using BotServicesLibrary.Abstractions;
using System;
using System.Collections.Generic;

namespace BotServicesLibrary
{
    public class SessionResult
    {
        public bool SessionEnd { get; }

        public IReadOnlyCollection<MessageAbstraction> Messages { get; }

        public SessionResult(IReadOnlyCollection<MessageAbstraction> messages, bool isSessionEnd = false)
        {
            Messages = messages ?? throw new ArgumentNullException(nameof(messages));
            SessionEnd = isSessionEnd;
        }

        public SessionResult(bool isSessionEnd, params MessageAbstraction[] messages)
        : this(messages, isSessionEnd)
        { }

        public SessionResult(params MessageAbstraction[] messages)
        : this(messages, false)
        { }

        public static implicit operator SessionResult(MessageAbstraction message) => new SessionResult(message);
    }
}