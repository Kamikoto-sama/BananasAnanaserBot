using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using BananasAnanaserBot.Models;
using Microsoft.Extensions.Configuration;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;
using VkNet.Utils;

namespace BananasAnanaserBot.Services
{
	public class VkEventHandler
	{
		private const char CommandSymbol = '%';
		private readonly IConfiguration configuration;
		private readonly IVkApi vkApi;
		private readonly SessionsContainer sessionsContainer;

		public VkEventHandler(IConfiguration configuration, IVkApi vkApi, SessionsContainer sessionsContainer)
		{
			this.configuration = configuration;
			this.vkApi = vkApi;
			this.sessionsContainer = sessionsContainer;
		}
		
		public async Task MessageNew(VkResponse vkResponse, long groupId)
		{
			var message = Message.FromJson(vkResponse);
			var peerId = message.PeerId;
			var messageText = message.Text;
			var errorMessage = configuration["ErrorMessage"];
			var session = sessionsContainer.GetOrCreateSession(message.FromId.ToString());

			if (session.ContextType != ContextType.None)
			 	messageText = session.HandleContextAction(message.Text);
			if (message.Text.StartsWith(CommandSymbol))
				HandleCommand(message, session)
					.Then(result => messageText = result)
					.OnFail(errMsg => messageText = errorMessage + errMsg);
			
			await SendMessage(peerId, messageText);
		}

		private Result<string> HandleCommand(Message messageText, Session session)
		{
			var commandParams = messageText.Text
				.TrimStart(CommandSymbol, ' ', '\n', '\t')
				.Split(' ');
			if (commandParams.Length < 1)
				return Result.Fail<string>("Missing command name");
			var commandName = commandParams.First();
			if (!Enum.TryParse(commandName, true, out Command command))
				return Result.Fail<string>("Unknown command");

			switch (command)
			{
				case Command.Python:
					session.ContextType = ContextType.Python;
					session.PythonInterpreter ??= PythonInterpreter.Run();
					return Result.Ok("Current context => Python 2.7 interpreter");
				case Command.Exit:
					session.ContextType = ContextType.None;
					return Result.Ok("Current context => None");
				case Command.Timer:
					return SetTimer(commandParams.Skip(1), messageText.PeerId);
			}
			return Result.Fail<string>("Unknown command");
		}

		private Result<string> SetTimer(IEnumerable<string> args, long? peerId)
		{
			var intervalString = args.FirstOrDefault();
			if (intervalString == null)
				return Result.Ok("Not enough arguments: minutes interval missing");
			if (!double.TryParse(intervalString, out var interval))
				return Result.Ok("Invalid syntax");
			interval = TimeSpan.FromMinutes(interval).TotalMilliseconds;
			var timer = new Timer(interval) {AutoReset = false};
			timer.Elapsed += async (sender, eventArgs) => await SendMessage(peerId, "Timer elapsed");
			timer.Start();
			return "Timer started";
		}

		private async Task SendMessage(long? peerId, string messageText, MessageKeyboard keyboard = null)
		{
			var messageParams = new MessagesSendParams
			{
				RandomId = DateTime.Now.Millisecond,
				PeerId = peerId,
				Message = messageText,
				Keyboard = keyboard
			};
			await vkApi.Messages.SendAsync(messageParams);
		}
	}
}