using BotServicesLibrary.Abstractions;
using System;
using System.Collections.Generic;

namespace BotServicesLibrary
{
    /// <summary>
    /// Base class for interacting with the user when performing more complex tasks.
    /// </summary>
    public abstract class Session : IDisposable
    {
        public readonly GlobalId OwnerId;

        public abstract bool CanReset { get; }

        protected IReadOnlyList<IExternalContentProvider> Providers { get; }

        protected Session(GlobalId userOwnerId, IReadOnlyList<IExternalContentProvider> providers)
        {
            OwnerId = userOwnerId;
            Providers = providers ?? new IExternalContentProvider[0];
            foreach (var provider in Providers)
                provider.Init();
        }

        /// <summary>
        /// Messages for send to user after session initializing.
        /// </summary>
        public abstract IEnumerable<MessageAbstraction> MessagesAfterInit { get; }

        /// <summary>
        /// The main method of the session with the bot service.
        /// Called when a message is received from the client and this session is initialized.
        /// </summary>
        public abstract SessionResult ApplyMessageText(string text);

        /// <summary>
        /// Reset session progress to init state.
        /// </summary>
        public abstract void Reset(object[] args);

        public virtual void Dispose()
        {
            foreach (var provider in Providers)
                provider.Dispose();
        }

        /// <summary>
        /// Throws <see cref="SessionWorkException"/> with template text.
        /// Template - "Can't init {SessionName} session without: {missingArgsNames}"
        /// </summary>
        /// <param name="missingArgsNames">Names of missing arguments in constructor</param>
        protected void ThrowExceptionWithMissingArgsInConstructor(params string[] missingArgsNames)
        {
            throw new SessionWorkException(
                $"Can't init {GetType().Name} session without: {string.Join(", ", missingArgsNames)}");
        }
    }
}