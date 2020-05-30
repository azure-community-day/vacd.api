using IAC.AZ.Tools.ServicePrincipalManager;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace IAC.AZ.Tools.ServicePrincipalManager.Tests
{
	public class IntegrationTestsFixture : IDisposable
	{
		private readonly TestServer _testServer;
		public HttpClient Client { get; }
		public IConfiguration Configuration { get; }

		public IntegrationTestsFixture()
		{
			Configuration = new ConfigurationBuilder()
				.SetBasePath(GetContentRootPath())
				.AddJsonFile("appsettings.json", optional: true)
				.AddJsonFile("appsettings.Development.json", optional: true)
				.AddUserSecrets<Startup>()
				.Build();

			var authority = $"{Configuration["AzureAd:Instance"].ToString()}{Configuration["AzureAd:TenantId"].ToString()}";

			var builder = new WebHostBuilder()
				   .UseContentRoot(GetContentRootPath())
				   .UseEnvironment("Test")
				   .ConfigureAppConfiguration(ctx =>
				   {
					   var keyVaultEndpoint = Configuration["KeyVault:EndPoint"].ToString();
					   if (!string.IsNullOrEmpty(keyVaultEndpoint))
					   {
						   bool.TryParse(Environment.GetEnvironmentVariable("AZURE_USE_MSI", EnvironmentVariableTarget.Process), out var useAzureMSI);
						   if (!useAzureMSI)
						   {
							   var clientCredential = new ClientCredential(Configuration["MsiOnPrem:ClientId"].ToString(), Configuration["MsiOnPrem:ClientSecret"].ToString());
							   AuthenticationContext context = new AuthenticationContext(authority, false);
							   AuthenticationResult authenticationResult = context.AcquireTokenAsync(
								   "https://vault.azure.net",
								   clientCredential).Result;

							   var keyVaultClient = new KeyVaultClient((auth, resource, scope) => Task.FromResult(authenticationResult.AccessToken));

							   ctx.AddAzureKeyVault(
								   keyVaultEndpoint, keyVaultClient, new DefaultKeyVaultSecretManager());

						   }
						   else
						   {
							   var azureServiceTokenProvider = new AzureServiceTokenProvider();
							   var keyVaultClient = new KeyVaultClient(
								   new KeyVaultClient.AuthenticationCallback(
									   azureServiceTokenProvider.KeyVaultTokenCallback));
							   ctx.AddAzureKeyVault(
								   keyVaultEndpoint, keyVaultClient, new DefaultKeyVaultSecretManager());
						   }
					   }
				   })
				   .UseStartup<Startup>();  // Uses Start up class from your API Host project to configure the test server

			_testServer = new TestServer(builder);

			Client = _testServer.CreateClient();
			Client.BaseAddress = new Uri("http://localhost:27984");
			Client.DefaultRequestHeaders.Accept.Clear();
			Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}

		private string GetContentRootPath()
		{
			var testProjectPath = PlatformServices.Default.Application.ApplicationBasePath;
			var relativePathToHostProject = @"..\..\..\..\src";
			return Path.Combine(testProjectPath, relativePathToHostProject);
		}

		public void Dispose()
		{
			Client.Dispose();
			_testServer.Dispose();
		}
	}
}
