using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Threading.Tasks;

namespace luis.azure.api.Services
{
	public class TokenBuilder : ITokenBuilder
	{
        private readonly IConfiguration _configuration;

        public TokenBuilder(IConfiguration configuration)
		{
            _configuration = configuration;
        }


        /// <summary>
        /// Generates a Bearer Token against Azure Management API.
        /// </summary>
        /// <returns>A <see cref="System.Threading.Tasks.Task"/> object that represents the asynchronous operation.</returns>
        public async Task<string> getToken()
        {

            var ClientId = _configuration["MasterSPN:ClientId"];
            var ClientSecret = _configuration["MasterSPN:ClientSecret"];
            ClientCredential cc = new ClientCredential(ClientId, ClientSecret);
            var context = new AuthenticationContext("https://login.windows.net/" + _configuration["AzureAd:TenantId2"]);
            var result = await context.AcquireTokenAsync("https://management.azure.com/", cc);
            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            return result.AccessToken;
        }
    }
}
