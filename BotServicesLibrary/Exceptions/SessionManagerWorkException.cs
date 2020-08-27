#nullable enable
using System;
using System.Runtime.Serialization;

namespace BotServicesLibrary
{
    /// <summary>
    /// Thrown on incorrect work in <see cref="ISessionManager"/>
    /// </summary>
    public class SessionManagerWorkException : ServiceWorkException
    {
        public SessionManagerWorkException()
        { }

        protected SessionManagerWorkException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        { }

        public SessionManagerWorkException(string? message) 
            : base(message)
        { }

        public SessionManagerWorkException(string? message, Exception? innerException) 
            : base(message, innerException)
        { }
    }
}