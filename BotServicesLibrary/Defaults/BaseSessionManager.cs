using BotServicesLibrary.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BotServicesLibrary.Defaults
{
    /// <summary>
    /// Basic implementation of <see cref="ISessionManager"/> with logging and certain types of exceptions.
    /// </summary>
    /// <typeparam name="TDictionaryType"></typeparam>
    /// <exception cref="SessionManagerWorkException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public class BaseSessionManager<TDictionaryType> : ISessionManager
        where TDictionaryType : IDictionary<GlobalId, Session>, new()
    {
        protected readonly ILogger Logger;

        protected readonly IDictionary<GlobalId, Session> Sessions;

        public BaseSessionManager(ILogger logger = null)
            : this(new TDictionaryType(), logger)
        { }

        public BaseSessionManager(TDictionaryType dictionary, ILogger logger = null)
        {
            Sessions = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            
            Logger = logger ?? NullLoggerFactory.Instance.CreateLogger(GetType().Name);
        }

        public Session GetSessionFromId(GlobalId userId)
        {
            if (!Sessions.ContainsKey(userId))
                throw new SessionManagerWorkException($"Don't have session at id {userId}");

            return Sessions[userId];
        }

        public TSession GetSessionFromId<TSession>(GlobalId userId)
            where TSession : notnull, Session
        {
            var client = GetSessionFromId(userId);
            return client as TSession
                   ?? throw new SessionManagerWorkException(
                       $"Can't get session with type {typeof(TSession).Name}, current type {client.GetType().Name}");
        }

        public bool TryGetSessionFromId(GlobalId userId, out Session session)
        {
            session = null;

            if (Sessions.ContainsKey(userId))
                session = Sessions[userId];

            return session != null;
        }

        public bool TryGetSessionFromId<TSession>(GlobalId userId, out TSession session)
            where TSession : Session
        {
            var userHaveActiveSession = TryGetSessionFromId(userId, out var resSession);

            session = resSession as TSession;
            return userHaveActiveSession && session != null;
        }

        public Session this[GlobalId id] => GetSessionFromId(id);

        public bool IsUserHaveActiveSession(GlobalId id) => Sessions.ContainsKey(id);
        public Type GetUserActiveSessionType(GlobalId id) => IsUserHaveActiveSession(id) ? Sessions[id].GetType() : null;

        public TSession InitSession<TSession>(GlobalId userId,
            IReadOnlyList<IExternalContentProvider> providers, params object[] args)
            where TSession : notnull, Session
        {
            var session = InitSession(userId, typeof(TSession), providers, args);
            return session as TSession;
        }

        public Session InitSession(GlobalId userId, Type sessionType,
            IReadOnlyList<IExternalContentProvider> providers, params object[] args)
        {
            if (!sessionType.IsSubclassOf(typeof(Session)))
                throw new SessionManagerWorkException(
                    $"Incorrect type for init session, type need be subclass of {typeof(Session)}");

            if (Sessions.ContainsKey(userId))
            {
                var current = Sessions[userId];
                if (!current.CanReset)
                    throw new SessionManagerWorkException(
                        $"Can't init session type {sessionType.Name} when working type {current.GetType().Name}");
                
                if (current.GetType() == sessionType)
                {
                    current.Reset(args);
                    Logger?.LogInformation($"Reset session \"{sessionType.Name}\" of user {userId}");
                    return current;
                }
            }

            var session = CreateSession(userId, sessionType, providers, args);
            
            if (Sessions.ContainsKey(userId))
                Sessions[userId] = session;
            else
                Sessions.Add(userId, session);

            Logger?.LogInformation($"Created session \"{sessionType.Name}\" of user {userId}");
            return session;
        }

        protected virtual Session CreateSession(GlobalId userId, Type sessionType,
            IReadOnlyList<IExternalContentProvider> providers, object[] args)
        {
            var arguments = new object[] { userId, providers }.Concat(args).ToArray();
            return (Session) Activator.CreateInstance(sessionType, arguments);
        }

        public bool CloseUserSession(GlobalId userId)
        {
            if (!Sessions.ContainsKey(userId)) return false;

            var result =  Sessions.Remove(userId, out var value);
            value?.Dispose();
            Logger?.LogInformation($"Closed session \"{value?.GetType().Name}\" of user {userId}");

            return result;
        }
    }
}