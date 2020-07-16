using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BananasAnanaserBot.Services
{
	public class SessionsContainer
	{
		private readonly ConcurrentDictionary<string, Session> sessions = 
			new ConcurrentDictionary<string, Session>();

		public Session GetOrCreateSession(string sessionId)
		{
			if (sessions.TryGetValue(sessionId, out var session))
				return session;
			if (sessions.ContainsKey(sessionId))
				throw new ArgumentException("Session with such id already exists.");
			session = new Session(sessionId);
			sessions[sessionId] = session;
			return session;
		}
	}
}