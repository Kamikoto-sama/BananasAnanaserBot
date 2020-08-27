using System;

namespace BotServicesLibrary.Abstractions
{
    /// <summary>
    /// Special type for providing external content for session.
    /// It is initiated once with a session and doesn't change in <see cref="Session"/> reset method.
    /// Can use for getting text from external database, for getting pictures and ect. 
    /// </summary>
    public interface IExternalContentProvider : IDisposable
    {
        public void Init();
    }
}