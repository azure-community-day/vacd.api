using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;

namespace luis.azure.api
{
	public class Program
	{
		public static void Main(string[] args)
		{
			BuildWebHost(args).Run();
		}

		public static IWebHost BuildWebHost(string[] args)
		{
			var webHost = new WebHostBuilder();
			var environment = webHost.GetSetting("environment");

			var configBuilder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: true)
				//.AddJsonFile("appsettings.{environment}.json", optional: true)
				.AddJsonFile("hosting.json", optional: true)
				.AddCommandLine(args);

			if (environment == "Development")
				configBuilder.AddUserSecrets<Startup>();

			var config = configBuilder.Build();

			return WebHost.CreateDefaultBuilder(args)
				   .UseConfiguration(config)
				   .UseStartup<Startup>()
				   .Build();
		}
	}
}