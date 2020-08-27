using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace BotServicesLibrary.Defaults
{
    /// <summary>
    /// Default implementation of <see cref="ISessionManager"/> with concurrent support.
    /// </summary>
    /// <exception cref="SessionManagerWorkException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public class DefaultSessionManager
        : BaseSessionManager<ConcurrentDictionary<GlobalId, Session>>
    {
        public DefaultSessionManager()
        : base(null)
        { }

        public DefaultSessionManager(ILogger logger = null) 
            : base(logger)
        { }

        public DefaultSessionManager(ConcurrentDictionary<GlobalId, Session> dictionary, ILogger logger = null)
            : base(dictionary, logger)
        { }
    }
}