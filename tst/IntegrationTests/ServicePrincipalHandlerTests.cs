using IAC.AZ.Tools.ServicePrincipalManager.Features.ServicePrincipal;
using IAC.AZ.Tools.ServicePrincipalManager.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace IAC.AZ.Tools.ServicePrincipalManager.Tests.IntegrationTests
{
    public class ServicePrincipalHandlerTests
    {
        private readonly Mock<IConfiguration> config;
        private readonly Mock<IConfigurationRoot> configRoot;

        public ServicePrincipalHandlerTests()
        {
            config = new Mock<IConfiguration>();
            configRoot = new Mock<IConfigurationRoot>();

            config.SetupGet(x => x[It.Is<string>(s => s == "KeyVault:MasterSpKey")])
                .Returns("OptimumCCoERootSP");

            config.SetupGet(x => x[It.Is<string>(s => s == "OptimumCCoERootSP")])
                .Returns("eyJDbGllbnRJZCI6ImFiYjMyOTZjLTZjMTMtNDk1Yy05NDc1LWFkMDhmZGZlZmRlNiIsIkNsaWVudFNlY3JldCI6Ij05OG1PakguKzZjaV1yNkw1YndPRm0uOVtOb3ZOd2U9In0=");

            config.SetupGet(x => x[It.Is<string>(s => s == "AzureAd:TenantId")])
                .Returns("000e9e6c-a02e-406e-a5ba-a077bf712dc1");

            config.SetupGet(x => x[It.Is<string>(s => s == "AzureAd:Instance")])
                .Returns("https://login.microsoftonline.com/");

            config.SetupGet(x => x[It.Is<string>(s => s == "MsiOnPrem:ClientId")])
                .Returns("237adbda-c46b-4628-815c-dff2f8b50afa");

            config.SetupGet(x => x[It.Is<string>(s => s == "MsiOnPrem:ClientSecret")])
                .Returns("DWR@6hnyRHtC:Ww[k_m6usYKtB4vi16/");

            configRoot.Setup(sb => sb[It.IsAny<string>()])
                .Returns((string key) => config.Object[key]);
        }

        [Fact]
        public async Task Create_Service_Principal_RootSP()
        {
            // Arrange
            var servicePrincipalBuilder = new ServicePrincipalBuilder(config.Object);
            var keyVaultSecretProvider = new KeyVaultSecretsConfigurationProvider(configRoot.Object, new KeyVaultSecretSerializer());
            var handler = new ServicePrincipalLevel2CreateHandler(servicePrincipalBuilder, keyVaultSecretProvider);

            var request = new ServicePrincipalLevel2CreateRequest()
            {
                KeyVaultName = "sppoccomkvt001",
                ServicePrincipalName = "IntegrationTest-" + Guid.NewGuid(),
                SubscriptionId = "64a1a7cd-b532-4817-87c7-3a27c7ae9e1a"
            };

            // Act
            var result = await handler.Handle(request, new CancellationToken());

            // Assert
            Assert.Equal(1, 1);
            Assert.Equal(request.ServicePrincipalName, result.ServicePrincipalName);
        }
    }
}