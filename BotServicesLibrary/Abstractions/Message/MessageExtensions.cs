using System.Collections.Generic;
using System.Linq;

namespace BotServicesLibrary.Abstractions
{
    public static class MessageExtensions
    {
        public static IEnumerable<MessageAbstraction> AttachMessage(this MessageAbstraction message,
            MessageAbstraction otherMessage) => new[] {message, otherMessage};

        public static IEnumerable<MessageAbstraction> AttachMessages(this MessageAbstraction message,
            IEnumerable<MessageAbstraction> otherMessages)
            => new[] {message}.AttachMessages(otherMessages);

        public static IEnumerable<MessageAbstraction> AttachMessages(this IEnumerable<MessageAbstraction> messages,
            IEnumerable<MessageAbstraction> otherMessages)
            => messages.Concat(otherMessages);

        public static IEnumerable<MessageAbstraction> AttachMessage(this IEnumerable<MessageAbstraction> messages,
            MessageAbstraction otherMessage)
            => messages.Append(otherMessage);
    }
}