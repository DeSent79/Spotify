using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spotify.DAL;
using Spotify.DAL.Init;
using Spotify.Domain.Entities.Identity;

namespace Spotify
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();
            CreateDbIfNotExists(host);
            host.Run();
        }

		public static void CreateDbIfNotExists(IHost host)
		{
			using (IServiceScope scope = host.Services.CreateScope())
			{
				IServiceProvider services = scope.ServiceProvider;

				try
				{
					var context = services.GetRequiredService<SpotifyDbContext>();
					var storage = services.GetRequiredService<IFileStorage<int>>();
					var userManager = services.GetRequiredService<UserManager<User>>();
					var signInManager = services.GetRequiredService<SignInManager<User>>();
					context.Database.EnsureCreated();
					DbInitializer.Initialize(context);
					DbInitializer.GetCustomUser(userManager, signInManager, context);
					storage.Init(context);
					DbInitializer.WriteFiles(storage);
				}
				catch (Exception ex)
				{
					var logger = services.GetRequiredService<ILogger<Program>>();
					logger.LogError(ex, "An error occurred creating the DB.");
				}
			}
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
