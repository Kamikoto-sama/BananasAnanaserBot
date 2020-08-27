#nullable enable
using System;
using System.Runtime.Serialization;

namespace BotServicesLibrary
{
    /// <summary>
    /// Thrown on incorrect work in <see cref="Session"/>
    /// </summary>
    public class SessionWorkException : Exception
    {
        public SessionWorkException()
        {
        }

        protected SessionWorkException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public SessionWorkException(string? message) : base(message)
        {
        }

        public SessionWorkException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}