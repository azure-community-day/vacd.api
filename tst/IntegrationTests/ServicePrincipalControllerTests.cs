using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace IAC.AZ.Tools.ServicePrincipalManager.Tests.IntegrationTests
{
    public class ServicePrincipalControllerTests : IClassFixture<IntegrationTestsFixture>
	{
		private readonly IntegrationTestsFixture _fixture;

		public ServicePrincipalControllerTests(IntegrationTestsFixture fixture)
		{
			_fixture = fixture;
		}

		[Fact]
		public async Task Get_Service_Principal()
		{
            // Arrange
            var authority = $"{_fixture.Configuration["AzureAd:Instance"].ToString()}{_fixture.Configuration["AzureAd:TenantId"].ToString()}";

            //var clientId = "abb3296c-6c13-495c-9475-ad08fdfefde6"; //mastersp
			//var clientSecret = "=98mOjH.+6ci]r6L5bwOFm.9[NovNwe=";
			var clientId = "0c2b4150-2c4f-46cd-a25d-c841a758b25c"; //Bambi
			var clientSecret = "05f63009b0f39c69c80ba8a80c0ac26c7097c4beca3ac58201c3a55cc00b962c";
			
			var resource = _fixture.Configuration["ServicePrincipalManagerApp:APIId"];

			var clientCredential = new ClientCredential(clientId, clientSecret);
			AuthenticationContext context = new AuthenticationContext(authority, false);
			AuthenticationResult authenticationResult = context.AcquireTokenAsync(resource, clientCredential).Result;
			_fixture.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);

			// Act
			var response = await _fixture.Client.GetAsync($"/api/serviceprincipals");
			response.EnsureSuccessStatusCode();

			var responseStrong = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
		}
	}
}