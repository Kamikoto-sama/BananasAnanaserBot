using BotServicesLibrary.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;

namespace BotServicesLibrary
{
    /// <summary>
    /// Base class for creating a bot service.
    /// It has a guaranteed catch of errors, implementations for sending messages with attempts,
    /// sending text for a user session and processing the session result.
    /// </summary>
    /// <typeparam name="TUserId">User unique identifier type.</typeparam>
    /// <exception cref="ServiceWorkException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public abstract class BotService<TUserId>
        where TUserId : notnull
    {
        public virtual string ServiceId => GetType().Name;

        protected GlobalId GetUserGlobalId(TUserId id) =>
            new GlobalId(ServiceId.GetHashCode().ToString(), id.ToString());

        protected readonly ILogger Logger;
        
        protected readonly int AttemptsCountForSendMessage;
        private readonly HashSet<Type> _typesOfInputMessages;

        protected readonly ISessionManager SessionManager;

        ///<summary>
        /// Methods to get message text from specific types of user message
        /// </summary>
        protected abstract IReadOnlyDictionary<Type, GetTextFromMessage> MessageTextGetters { get; }

        ///<summary>
        /// Collection of messages presets for unrecognized message
        /// </summary>
        protected abstract IReadOnlyDictionary<Type, ProcessMessageAndReturn> UnrecognizedMessageSender { get; }

        protected BotService(ISessionManager sessionManager, IEnumerable<Type> acceptedMessageTypes, 
            int attemptsCountForSendMessage, ILogger logger = null)
        {
            Logger = logger ?? NullLoggerFactory.Instance.CreateLogger(GetType().Name);

            _typesOfInputMessages = new HashSet<Type>(acceptedMessageTypes);
            AttemptsCountForSendMessage = attemptsCountForSendMessage;

            SessionManager = sessionManager ?? throw new ArgumentNullException(nameof(sessionManager));
        }

        /// <summary>
        /// Processes the message and sends response messages to the user.
        /// </summary>
        /// <param name="message">Message from user, his type must be contained in an array of accepted types.</param>
        /// <param name="userId">User unique identifier.</param>
        /// <typeparam name="TMessage">Message type, it must be contained in an array of accepted types</typeparam>
        public void ApplyNewMessage<TMessage>(TMessage message, TUserId userId)
            where TMessage : notnull
        {
            IEnumerable<MessageAbstraction> messages;
            try
            {
                if (!_typesOfInputMessages.Contains(typeof(TMessage)))
                    throw new ServiceWorkException(
                        $"{GetType().Name} can't apply message with type {typeof(TMessage).Name}");

                messages = ProcessMessage(message, userId);
                Logger?.LogInformation($"Applied message from user {userId}");

            }
            catch (Exception e)
            {
                Logger?.LogError(e, $"Exception on applying message from user {userId}: ");
                messages = OnHandleException(e, userId);
            }

            if (messages == null) return;
                
            SendMessageWithAttempts(userId, messages);
        }

        private IEnumerable<MessageAbstraction> ProcessMessage<TMessage>(TMessage message, TUserId userId) 
            where TMessage : notnull
        {
            if (TryCustomMessageProcessing(userId, message, out var processedResult))
                return processedResult;

            var type = typeof(TMessage);

            var text = MessageTextGetters[type].Invoke(message);

            return TryApplyMessageTextToSession(userId, text, out var result)
                ? result
                : UnrecognizedMessageSender[type].Invoke(message, userId);
        }

        /// <summary>
        /// Sends messages to the user. If an error occurs when sending a message, it tries to send the <see cref="AttemptsCountForSendMessage"/> number of times
        /// </summary>
        /// <param name="userId">User unique identifier.</param>
        /// <param name="messages">Messages to user in abstract form.</param>
        protected void SendMessageWithAttempts(TUserId userId, IEnumerable<MessageAbstraction> messages)
        {
            foreach (var message in messages)
            {
                for(var i = AttemptsCountForSendMessage; i > 0 ; i--)
                {
                    try
                    {
                        SendMessageFromParams(userId, message);
                        Logger?.LogInformation($"Message send to user {userId}");
                        break;
                    }
                    catch (Exception e)
                    {
                        if (i == 1)
                            Logger?.LogError(e, $"Can't send message to user {userId}");
                        else
                            Logger?.LogWarning(e,
                                $"Exception on sending message to user {userId}, {i - 1} attempts left");
                    }
                }
            }
        }

        /// <summary>
        /// Try apply text from user message to current active session.
        /// </summary>
        /// <param name="userId">User unique identifier.</param>
        /// <param name="text">Text for applying to session.</param>
        /// <param name="messages">Messages to user in abstract form.</param>
        protected bool TryApplyMessageTextToSession(TUserId userId, string text,
            out IEnumerable<MessageAbstraction> messages)
        {
            if (SessionManager.TryGetSessionFromId(GetUserGlobalId(userId), out var session))
            {
                var result = session.ApplyMessageText(text);
                
                var sessionTypeName = session.GetType().Name;
                Logger?.LogInformation($"Session \"{sessionTypeName}\" applied message from user {userId}");

                if (result is SessionResultWithSaveData data)
                    SaveDataFromSession(userId, data.SaveData);

                ProcessCustomDataFromSessionResult(userId, result);

                messages = result.Messages;

                if (result.SessionEnd)
                {
                    SessionManager.CloseUserSession(GetUserGlobalId(userId));
                    messages = messages.AttachMessages(GetTemplateMessageAfterSessionEnd(userId));
                }

                return true;
            }

            messages = null;
            return false;
        }

        /// <summary>
        /// Helpful method for create <see cref="Session"/>
        /// and get its messages for user after initialization.
        /// </summary>
        /// <typeparam name="TSessionType">Type of session."/></typeparam>
        /// <param name="userId">User unique identifier.</param>
        /// <param name="providers">Necessary collection of <see cref="IExternalContentProvider"/> for that session type.</param>
        /// <param name="args">Custom arguments for that session type.</param>
        /// <returns></returns>
        protected IEnumerable<MessageAbstraction> InitSessionAndGetAfterInitMessages<TSessionType>(TUserId userId,
            IReadOnlyList<IExternalContentProvider> providers = null, params object[] args)
            where TSessionType : Session
        {
            var session = SessionManager.InitSession<TSessionType>(GetUserGlobalId(userId), providers, args);
            return session.MessagesAfterInit;
        }

        /// <summary>
        /// Custom processing of user <see cref="message"/>.
        /// </summary>
        /// <param name="userId">User unique identifier.</param>
        /// <param name="message">Message from user, his type must be contained in an array of accepted types.</param>
        /// <param name="messagesToSend">Result messages of process.</param>
        /// <typeparam name="TMessage">Message type, it must be contained in an array of accepted types</typeparam>
        /// <returns>Use custom processing or default.</returns>
        protected abstract bool TryCustomMessageProcessing<TMessage>(TUserId userId, TMessage message, out IEnumerable<MessageAbstraction> messagesToSend);

        /// <summary>
        /// Logic of sending message to user.
        /// </summary>
        /// <param name="userId">User unique identifier.</param>
        /// <param name="messageAbstraction">Message to user in abstract form.</param>
        protected abstract void SendMessageFromParams(TUserId userId, MessageAbstraction messageAbstraction);

        /// <summary>
        /// Need to save your special data from <see cref="SessionResultWithSaveData"/>.
        /// </summary>
        /// <param name="userId">User unique identifier.</param>
        /// <param name="saveData">Abstract type for data container.</param>
        protected abstract void SaveDataFromSession(TUserId userId, ISaveData saveData);

        /// <summary>
        /// Need to process custom data in children classes of <see cref="SessionResult"/> or <see cref="SessionResultWithSaveData"/>.
        /// </summary>
        /// <param name="userId">User unique identifier.</param>
        /// <param name="result">Messages to user in abstract form.</param>
        protected abstract void ProcessCustomDataFromSessionResult(TUserId userId, SessionResult result);

        /// <summary>
        /// Method invoke when throw exception while service processing user message.
        /// </summary>
        /// <param name="userId">User unique identifier.</param>
        /// <param name="exception">Thrown error while process message.</param>
        /// <returns>Messages about error for user.</returns>
        protected abstract IEnumerable<MessageAbstraction> OnHandleException(Exception exception, TUserId userId);

        /// <summary>
        /// Sends that messages to user additionally after any session close.
        /// </summary>
        protected abstract IEnumerable<MessageAbstraction> GetTemplateMessageAfterSessionEnd(TUserId userId);



        /// <summary>
        /// Make action with message and return messages for user.
        /// </summary>
        /// <param name="message">Message with specific type in object form.</param>
        /// <param name="userId">User unique identifier.</param>
        protected delegate IEnumerable<MessageAbstraction> ProcessMessageAndReturn(object message, TUserId userId);

        /// <summary>
        /// Get text from user message.
        /// </summary>
        /// <param name="message">Message with specific type in object form.</param>
        protected delegate string GetTextFromMessage(object message);
    }
}