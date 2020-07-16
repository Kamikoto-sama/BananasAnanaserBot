using System;
using System.Threading.Tasks;
using BananasAnanaserBot.Models;
using BananasAnanaserBot.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using VkNet.Utils;

namespace BananasAnanaserBot.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class CallbackController : ControllerBase
	{
		private readonly IConfiguration configuration;
		private readonly VkEventHandler vkEventHandler;

		public CallbackController(IConfiguration configuration, VkEventHandler vkEventHandler)
		{
			this.configuration = configuration;
			this.vkEventHandler = vkEventHandler;
		}

		[HttpPost("Vk")]
		public async Task<OkObjectResult> VkCallback([FromBody] EventDto eventDto)
		{
			var vkResponse = new VkResponse(eventDto.Object);
			switch (eventDto.Type)
			{
				case "confirmation":
					return Ok(configuration["ConfirmationCode"]);
				case "message_new":
					await vkEventHandler.MessageNew(vkResponse, eventDto.GroupId);
					break;
			}
			return Ok("ok");
		}
	}
}