using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace IAC.AZ.Tools.ServicePrincipalManager.Tests.IntegrationTests
{
    public class AuthenticationControllerTests : IClassFixture<IntegrationTestsFixture>
	{
		private readonly IntegrationTestsFixture _fixture;

		public AuthenticationControllerTests(IntegrationTestsFixture fixture)
		{
			_fixture = fixture;
		}

		[Fact]
		public async Task Get_Authentication_Returns_Token()
		{
			var aaa = _fixture.Configuration["ServicePrincipalManagerApp:APIId"];

			// Arrange
			var clientId = "0c2b4150-2c4f-46cd-a25d-c841a758b25c"; //Bambi
			var clientSecret = "05f63009b0f39c69c80ba8a80c0ac26c7097c4beca3ac58201c3a55cc00b962c";

			var response = await _fixture.Client.GetAsync($"/api/authentication/token?ClientId={clientId}&ClientSecret={clientSecret}");
			response.EnsureSuccessStatusCode();

			// Act
			var responseStrong = await response.Content.ReadAsStringAsync();

			var authority = $"{_fixture.Configuration["AzureAd:Instance"].ToString()}{_fixture.Configuration["AzureAd:TenantId"].ToString()}";
			IConfigurationManager<OpenIdConnectConfiguration> configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>($"{authority}/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever());
			OpenIdConnectConfiguration openIdConfig = await configurationManager.GetConfigurationAsync(CancellationToken.None);

			var token = responseStrong.Replace("\"", "");
			var audience = _fixture.Configuration["ServicePrincipalManagerApp:APIId"];

			TokenValidationParameters validationParameters =
				new TokenValidationParameters
				{
					ValidIssuer = authority,
					ValidAudiences = new[] { audience },
					IssuerSigningKeys = openIdConfig.SigningKeys
				};
			JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();			
			var user = handler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

			// Assert
			Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
			Assert.Equal(clientId, user.Claims.FirstOrDefault(c => c.Type == "appid")?.Value);
		}
	}
}
