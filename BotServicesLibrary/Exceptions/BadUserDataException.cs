#nullable enable
using System;

namespace BotServicesLibrary
{
    /// <summary>
    /// Exception when user input incorrect data to session.
    /// Exception contain <see cref="MessageToUser"/>, it can be appear to user.
    /// </summary>
    public class BadUserDataException : ArgumentException
    {
        public string MessageToUser { get; }

        public BadUserDataException(string messageToUser, string? message) : base(message)
        {
            MessageToUser = messageToUser;
        }

        public BadUserDataException(string messageToUser, string? message, Exception? innerException) : base(message, innerException)
        {
            MessageToUser = messageToUser;
        }

        public BadUserDataException(string messageToUser, string? message, string? paramName) : base(message, paramName)
        {
            MessageToUser = messageToUser;
        }

        public BadUserDataException(string messageToUser, string? message, string? paramName, Exception? innerException) : base(message, paramName, innerException)
        {
            MessageToUser = messageToUser;
        }
    }
}