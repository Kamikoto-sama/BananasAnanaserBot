using BananasAnanaserBot.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VkNet;
using VkNet.Abstractions;
using VkNet.Model;

namespace BananasAnanaserBot
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			// var connectionString = Configuration.GetConnectionString("MySql" + Env.EnvironmentName);
			// services.AddSingleton<DataBaseContext>(new MySqlDataBase(connectionString));
			
			var api = new VkApi();
			var authParams = new ApiAuthParams{ AccessToken = Configuration["AccessToken"] };
			api.Authorize(authParams);
			services.AddSingleton<IVkApi>(api);

			services.AddSingleton<VkEventHandler>();
			services.AddSingleton<SessionsContainer>();
			
			services.AddControllers().AddNewtonsoftJson();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseExceptionHandler("/handleError");
			app.UseHttpsRedirection();

			app.UseRouting()
				.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
		}
	}
}