using IAC.AZ.Tools.ServicePrincipalManager.Models;
using IAC.AZ.Tools.ServicePrincipalManager.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace IAC.AZ.Tools.ServicePrincipalManager.Tests.IntegrationTests
{
    public class KeyVaultSecretsConfigurationProviderTests
    {
        [Fact]
        public void KeyVaultSecretsConfigurationProvider_Returns_Master_SP()
        {
            // Arrange
            var expected = new ServicePrincipal(
                "abb3296c-6c13-495c-9475-ad08fdfefde6", 
                "=98mOjH.+6ci]r6L5bwOFm.9[NovNwe="); //mastersp

            var kvSerializer = new KeyVaultSecretSerializer();
            var serializedMasterSp = kvSerializer.Serialize(expected);

            var config = new Mock<IConfigurationRoot>();

            config.SetupGet(x => x[It.Is<string>(s => s == "KeyVault:MasterSpKey")])
                .Returns("OptimumCCoERootSP");

            config.SetupGet(x => x[It.Is<string>(s => s == "OptimumCCoERootSP")])
                .Returns(serializedMasterSp);

            config.SetupGet(x => x[It.Is<string>(s => s == "AzureAd:TenantId")])
                .Returns("000e9e6c-a02e-406e-a5ba-a077bf712dc1");

            config.SetupGet(x => x[It.Is<string>(s => s == "AzureAd:Instance")])
                .Returns("https://login.microsoftonline.com/");

            var kvConfig = new KeyVaultSecretsConfigurationProvider(config.Object, kvSerializer);

            // Act
            var actual = kvConfig.GetMasterServicePrincipal();

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(expected.ClientId, actual.ClientId);
            Assert.Equal(expected.ClientSecret, actual.ClientSecret);
        }

        [Fact]
        public void KeyVaultSecretsConfigurationProvider_Returns_SP0_List()
        {
            // Arrange
            var expected = new List<string>();
            expected.Add("abb3296c-6c13-495c-9475-ad08fdfefde6");

            var kvSerializer = new KeyVaultSecretSerializer();
            var serializedSp0List = kvSerializer.Serialize(expected);

            var config = new Mock<IConfigurationRoot>();

            config.SetupGet(x => x[It.Is<string>(s => s == "KeyVault:Sp0ListKey")])
                .Returns("OptimumCCoEGlobalAdmins");

            config.SetupGet(x => x[It.Is<string>(s => s == "OptimumCCoEGlobalAdmins")])
                .Returns(serializedSp0List);

            config.SetupGet(x => x[It.Is<string>(s => s == "AzureAd:TenantId")])
                .Returns("000e9e6c-a02e-406e-a5ba-a077bf712dc1");

            config.SetupGet(x => x[It.Is<string>(s => s == "AzureAd:Instance")])
                .Returns("https://login.microsoftonline.com/");

            var kvConfig = new KeyVaultSecretsConfigurationProvider(config.Object, kvSerializer);

            // Act
            var actual = kvConfig.GetSp0List();

            // Assert
            Assert.NotNull(actual);
            Assert.Contains(actual, f => f.Contains("abb3296c-6c13-495c-9475-ad08fdfefde6"));
        }

        [Fact]
        public void KeyVaultSecretsConfigurationProvider_Returns_SP1_List()
        {
            // Arrange
            var expected = new List<EnrollmentAccountInfo>();
            expected.Add(new EnrollmentAccountInfo()
            {
                EnrollmentAccountId = "04b40527-4ef6-40ba-9db4-f954ee905f92",
                KeyVaultName = "sppoccritkvt001",
                SecretName = "EnrollmentAccountSecret"
            });

            var kvSerializer = new KeyVaultSecretSerializer();
            var serializedEnrollmentAccounts = kvSerializer.Serialize(expected);

            var config = new Mock<IConfigurationRoot>();


            config.SetupGet(x => x[It.Is<string>(s => s == "KeyVault:Sp1ListKey")])
                .Returns("OptimumCCoEEnrollmentAccounts");

            config.SetupGet(x => x[It.Is<string>(s => s == "OptimumCCoEEnrollmentAccounts")])
                .Returns(serializedEnrollmentAccounts);

            config.SetupGet(x => x[It.Is<string>(s => s == "AzureAd:TenantId")])
                .Returns("000e9e6c-a02e-406e-a5ba-a077bf712dc1");

            config.SetupGet(x => x[It.Is<string>(s => s == "AzureAd:Instance")])
                .Returns("https://login.microsoftonline.com/");

            var kvConfig = new KeyVaultSecretsConfigurationProvider(config.Object, kvSerializer);

            // Act
            var actual = kvConfig.GetSp1List();

            // Assert
            Assert.NotNull(actual);
            Assert.Contains(actual, f => f.EnrollmentAccountId == "04b40527-4ef6-40ba-9db4-f954ee905f92");
        }

        [Fact]
        public void KeyVaultSecretsConfigurationProvider_Update_Sp0_and_Sp1_List()
        {
            // Arrange
            var expectedEnrollmentAccounts = new List<EnrollmentAccountInfo>();
            expectedEnrollmentAccounts.Add(new EnrollmentAccountInfo()
            {
                EnrollmentAccountId = "04b40527-4ef6-40ba-9db4-f954ee905f92",
                KeyVaultName = "sppoccritkvt001",
                SecretName = "EnrollmentAccountSecret"
            });

            var expectedGlobalAdmins = new List<string>() { "04b40527-4ef6-40ba-9db4-f954ee905f92" };

            var masterServicePrincipal = new ServicePrincipal(
                "abb3296c-6c13-495c-9475-ad08fdfefde6",
                "=98mOjH.+6ci]r6L5bwOFm.9[NovNwe="); //mastersp

            var kvSerializer = new KeyVaultSecretSerializer();
            var serializedEnrollmentAccounts = kvSerializer.Serialize(expectedEnrollmentAccounts);
            var serializedMasterSp = kvSerializer.Serialize(masterServicePrincipal);
            var serializedGlobalAdm = kvSerializer.Serialize(expectedGlobalAdmins);

            var config = new Mock<IConfigurationRoot>();

            config.SetupGet(x => x[It.Is<string>(s => s == "KeyVault:Sp0ListKey")])
                .Returns("OptimumCCoEGlobalAdmins");

            config.SetupGet(x => x[It.Is<string>(s => s == "KeyVault:Sp1ListKey")])
                .Returns("OptimumCCoEEnrollmentAccounts");

            config.SetupGet(x => x[It.Is<string>(s => s == "KeyVault:MasterSpKey")])
                .Returns("OptimumCCoERootSP");

            config.SetupGet(x => x[It.Is<string>(s => s == "AzureAd:TenantId")])
                .Returns("000e9e6c-a02e-406e-a5ba-a077bf712dc1");

            config.SetupGet(x => x[It.Is<string>(s => s == "AzureAd:Instance")])
                .Returns("https://login.microsoftonline.com/");

            config.SetupGet(x => x[It.Is<string>(s => s == "OptimumCCoERootSP")])
                .Returns(serializedMasterSp);

            config.SetupGet(x => x[It.Is<string>(s => s == "KeyVault:EndPoint")])
                .Returns("https://masterspkvt.vault.azure.net/");

            var kvConfig = new KeyVaultSecretsConfigurationProvider(config.Object, kvSerializer);

            // Act
            var actualSp0Update = kvConfig.UpdateSp0List(expectedGlobalAdmins).Result;
            var actualSp1Update = kvConfig.UpdateSp1List(expectedEnrollmentAccounts).Result;

            // Assert
            Assert.NotNull(actualSp0Update);
            Assert.NotNull(actualSp1Update);
            Assert.Equal(serializedGlobalAdm, actualSp0Update);
            Assert.Equal(serializedEnrollmentAccounts, actualSp1Update);
        }
    }
}