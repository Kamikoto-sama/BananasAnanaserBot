using System.Collections.Generic;
using BananasAnanaserBot.Models;

namespace BananasAnanaserBot.Services
{
	public class Session
	{
		public string Id { get; }
		public ContextType ContextType { get; set; }
		public PythonInterpreter PythonInterpreter { get; set; }

		public Session(string id)
		{
			Id = id;
		}

		public string HandleContextAction(string messageText)
		{
			switch (ContextType)
			{
				case ContextType.Python:
					messageText = messageText.Replace('_', '\t');
					return PythonInterpreter.ExecuteExpression(messageText);
			}

			return "Unknown action";
		}
	}
}