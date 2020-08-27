using BotServicesLibrary.Abstractions;
using System;
using System.Collections.Generic;

namespace BotServicesLibrary
{
    /// <summary>
    /// Session storage for each user.
    /// </summary>
    public interface ISessionManager
    {
        public Session GetSessionFromId(GlobalId userId);
        public Session this[GlobalId id] => GetSessionFromId(id);
        public TSession GetSessionFromId<TSession>(GlobalId userId)
            where TSession : notnull, Session;

        public bool TryGetSessionFromId(GlobalId userId, out Session session);
        public bool TryGetSessionFromId<TSession>(GlobalId userId, out TSession session)
            where TSession : Session;

        public bool IsUserHaveActiveSession(GlobalId id);
        public Type GetUserActiveSessionType(GlobalId id);

        public Session InitSession(GlobalId userId, Type sessionType,
            IReadOnlyList<IExternalContentProvider> providers = null, params object[] args);

        public TSession InitSession<TSession>(GlobalId userId, 
            IReadOnlyList<IExternalContentProvider> providers = null, params object[] args)
            where TSession : notnull, Session;

        public bool CloseUserSession(GlobalId userId);
    }
}