using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IAC.AZ.Tools.ServicePrincipalManager.Tests.IntegrationTests
{
	public class HealthControllerTests : IClassFixture<IntegrationTestsFixture>
	{
		private readonly IntegrationTestsFixture _fixture;

		public HealthControllerTests(IntegrationTestsFixture fixture)
		{
			_fixture = fixture;
		}

		[Fact]
		public async Task Get_Health_Returns_Pong()
		{
			// Arrange
			var response = await _fixture.Client.GetAsync("/api/health");
			response.EnsureSuccessStatusCode();

			// Act
			var responseStrong = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
			Assert.Equal("\"pong\"", responseStrong);
		}
	}
}
