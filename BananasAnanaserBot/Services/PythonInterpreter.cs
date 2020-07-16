using System;
using System.IO;
using System.Linq;
using System.Text;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace BananasAnanaserBot.Services
{
	public class PythonInterpreter
	{
		private readonly ScriptEngine engine;
		private readonly ScriptScope scope;

		public PythonInterpreter(ScriptEngine engine, ScriptScope scope)
		{
			this.engine = engine;
			this.scope = scope;
		}
		
		public static PythonInterpreter Run()
		{
			var engine = Python.CreateEngine();
			var scope = engine.CreateScope();
			return new PythonInterpreter(engine, scope);
		}

		public string ExecuteExpression(string expression)
		{
			string output;
			using var stream = new MemoryStream();
			engine.Runtime.IO.SetOutput(stream, Encoding.ASCII);
			try
			{
				var result = engine.Execute(expression, scope);
				output = result?.ToString();
			}
			catch (Exception e)
			{
				output = "Error: " + e.Message;
			}

			if (stream.Length > 0)
				output = string.Join("", stream.ToArray().Select(b => (char) b)) + output;
			return output ?? "<No output>";
		}
	}
}