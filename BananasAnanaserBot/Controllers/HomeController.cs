using Microsoft.AspNetCore.Mvc;

namespace BananasAnanaserBot.Controllers
{
	[Route("")]
	public class HomeController : Controller
	{
		[HttpGet]
		public ActionResult Home()
		{
			return Ok("Well, it works!");
		}
	}
}