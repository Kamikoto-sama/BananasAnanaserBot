using Microsoft.AspNetCore.Mvc;

namespace BananasAnanaserBot.Controllers
{
	[ApiController]
	[ApiExplorerSettings(IgnoreApi = true)]
	public class ErrorController : ControllerBase
	{
		[Route("/handleError")]
		public IActionResult HandleError() => Ok("ok");
	}
}