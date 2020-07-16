using System;
using System.IO;
using System.Linq;
using System.Text;
using IronPython.Hosting;

namespace PythonIntegration
{
	class Program
	{
		static void Main(string[] args)
		{
			var a = TimeSpan.FromMinutes(1).Milliseconds;
			Console.WriteLine("HGello" + a);
			return;
			var engine = Python.CreateEngine();
			var scope = engine.CreateScope();
			Console.WriteLine("Python 2.7");
			while (true)
			{
				Console.Write(">>> ");
				var expression = Console.ReadLine();
				if (expression == "r")
				{
					expression = "def a():\n_return 1".Replace('_', '\t');
					Console.WriteLine(expression);
				}
				var stream = new MemoryStream();
				engine.Runtime.IO.SetOutput(stream, Encoding.ASCII);
				try
				{
					var result = engine.Execute(expression, scope);
					var message = string.Join("", stream.ToArray().Select(b => (char) b));
					if (message.Length > 0)
						Console.WriteLine(message.Trim());
					stream.Close();
					if (result != null)
						Console.WriteLine(result);
				}
				catch (Exception e)
				{
					Console.WriteLine("Error: " + e.Message);
				}
			}
		}
	}
}