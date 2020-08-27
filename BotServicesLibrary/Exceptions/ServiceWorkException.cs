#nullable enable
using System;
using System.Runtime.Serialization;

namespace BotServicesLibrary
{
    /// <summary>
    /// Thrown on incorrect work in <see cref="BotService{TUserId}"/>
    /// </summary>
    public class ServiceWorkException : Exception
    {
        public ServiceWorkException()
        {
        }

        protected ServiceWorkException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ServiceWorkException(string? message) : base(message)
        {
        }

        public ServiceWorkException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}