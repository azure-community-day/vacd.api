using IAC.AZ.Tools.ServicePrincipalManager.Models;
using IAC.AZ.Tools.ServicePrincipalManager.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace IAC.AZ.Tools.ServicePrincipalManager.Tests.IntegrationTests
{
    public class RoleDefinitionTests : IClassFixture<MemoryCacheFixture>
    {
        private readonly IMemoryCache cache;
        private readonly Mock<IConfiguration> config;
        private readonly Mock<IConfigurationRoot> configRoot;

        public RoleDefinitionTests(MemoryCacheFixture memoryCache)
        {

            cache = memoryCache.Cache;

            config = new Mock<IConfiguration>();
            configRoot = new Mock<IConfigurationRoot>();

            var expected = new ServicePrincipal(
                "04b40527-4ef6-40ba-9db4-f954ee905f92",
                "d77d75c3-7c1d-4351-8b6d-195b4d550c31");

            var kvSerializer = new KeyVaultSecretSerializer();
            var serializedMasterSp = kvSerializer.Serialize(expected);

            config.SetupGet(x => x[It.Is<string>(s => s == "KeyVault:MasterSpKey")])
                .Returns("OptimumCCoERootSP");

            config.SetupGet(x => x[It.Is<string>(s => s == "OptimumCCoERootSP")])
                .Returns(serializedMasterSp);

            config.SetupGet(x => x[It.Is<string>(s => s == "AzureAd:TenantId")])
                .Returns("35595a02-4d6d-44ac-99e1-f9ab4cd872db");

            config.SetupGet(x => x[It.Is<string>(s => s == "AzureAd:Instance")])
                .Returns("https://login.microsoftonline.com/");

            configRoot.Setup(sb => sb[It.IsAny<string>()])
                .Returns((string key) => config.Object[key]);

        }

        [Theory]
        [InlineData("Owner")]
        [InlineData("Billing Reader")]
        [InlineData("owner")]
        public async Task Get_Role_Definition_By_Name(string roleName)
        {
            // Arrange
            var kvSerializer = new KeyVaultSecretSerializer();
            var kvConfig = new KeyVaultSecretsConfigurationProvider(configRoot.Object, kvSerializer);
            var roleDefinitionService = new RoleDefinitionService(cache, config.Object, kvConfig);

            // Act
            var roleDefinition = roleDefinitionService.GetRoleByName(roleName);

            // Assert
            Assert.Equal(roleName.ToLowerInvariant(), roleDefinition.Properties.RoleName.ToLowerInvariant());
        }
    }
}