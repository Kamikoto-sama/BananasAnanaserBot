using BotServicesLibrary.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace BotServicesLibrary
{
    /// <summary>
    /// Extension of the bot service with the implemented logic of commands,
    /// with custom search for commands in the message and associations for bot commands.
    /// </summary>
    /// <typeparam name="TUserId">User unique identifier type.</typeparam>
    public abstract class BotServiceWithCommands<TUserId> : BotService<TUserId>
    {
        ///<summary>
        /// Collection with Bot commands with text associations in lower case
        /// </summary>
        protected abstract IReadOnlyDictionary<string, ProcessMessageAndReturn> BotCommands { get; }
        ///<summary>
        /// Text associations with specific command from <see cref="BotCommands"/>
        /// </summary>
        protected abstract IReadOnlyDictionary<string, string> CommandsAssociations { get; }


        ///<summary>
        /// Methods to parse command from specific types of user message
        /// </summary>
        protected abstract IReadOnlyDictionary<Type, TryGetCommandFromMessage> CommandsGetters { get; }
        


        protected BotServiceWithCommands(ISessionManager sessionManager,
            IEnumerable<Type> acceptedMessageTypes, int attemptsCountForSendMessage, ILogger logger = null)
            : base(sessionManager, acceptedMessageTypes, attemptsCountForSendMessage, logger)
        { }

        protected override bool TryCustomMessageProcessing<TMessage>(TUserId userId, TMessage message, 
            out IEnumerable<MessageAbstraction> messagesToSend)
        {
            messagesToSend = null;

            if (!CommandsGetters[typeof(TMessage)].Invoke(message, out var command)) 
                return false;

            if (BotCommands.TryGetValue(command, out var action))
            {
                messagesToSend = action(message, userId);
                Logger?.LogInformation($"Accepted command \"{command}\" from user {userId}");
            }
            else messagesToSend = OnBadCommand(command, userId);

            return true;
        }

        /// <summary>
        /// Try find <see cref="text"/> in <see cref="BotCommands"/> and <see cref="CommandsAssociations"/>.
        /// </summary>
        /// <param name="text">Text of not accepted command.</param>
        /// <param name="command">User unique identifier.</param>
        protected bool IsTextContainCommand(string text, out string command)
        {
            if (CommandsAssociations.ContainsKey(text))
            {
                command = CommandsAssociations[text];
                return true;
            }

            var lowerText = text.ToLower();
            if (BotCommands.ContainsKey(lowerText))
            {
                command = lowerText;
                return true;
            }

            command = null;
            return false;
        }

        /// <summary>
        /// Invoke when service can't find <see cref="badCommand"/> in <see cref="BotCommands"/> or <see cref="CommandsAssociations"/>.
        /// </summary>
        /// <param name="badCommand">Text of not accepted command.</param>
        /// <param name="userId">User unique identifier.</param>
        protected abstract IEnumerable<MessageAbstraction> OnBadCommand(string badCommand, TUserId userId);



        /// <summary>
        /// Try parse or find command in accepted user message.
        /// </summary>
        /// <param name="message">Message with specific type in object form.</param>
        /// <param name="command">Parsed command from message.</param>
        protected delegate bool TryGetCommandFromMessage(object message, out string command);
    }
}