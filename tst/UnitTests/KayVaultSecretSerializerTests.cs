using IAC.AZ.Tools.ServicePrincipalManager.Models;
using IAC.AZ.Tools.ServicePrincipalManager.Services;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xunit;

namespace IAC.AZ.Tools.ServicePrincipalManager.Tests.UnitTests
{
    public class KayVaultSecretSerializerTests
	{
		[Fact]
		public void Service_Principal_Is_Serialized_To_Base64()
		{
			// Arrange
			var s = new KeyVaultSecretSerializer();
			var sp = new ServicePrincipal("65e318d4-d615-482c-a18b-203418d1d278", "Ft*v2xQpC1Ib3+iUoKswYGjUWj4]T_1a");
			var expected = "eyJDbGllbnRJZCI6IjY1ZTMxOGQ0LWQ2MTUtNDgyYy1hMThiLTIwMzQxOGQxZDI3OCIsIkNsaWVudFNlY3JldCI6IkZ0KnYyeFFwQzFJYjMraVVvS3N3WUdqVVdqNF1UXzFhIn0=";

			// Act
			var actual = s.Serialize(sp);

			// Assert
			Assert.Equal(expected, actual);
		}

        [Fact]
        public void Service_Principal_Is_Serialized_To_Base64_SPMaster_L_Subscription()
        {
            // Arrange
            var s = new KeyVaultSecretSerializer();
            var sp = new ServicePrincipal("abb3296c-6c13-495c-9475-ad08fdfefde6", "=98mOjH.+6ci]r6L5bwOFm.9[NovNwe=");
            var expected = "eyJDbGllbnRJZCI6ImFiYjMyOTZjLTZjMTMtNDk1Yy05NDc1LWFkMDhmZGZlZmRlNiIsIkNsaWVudFNlY3JldCI6Ij05OG1PakguKzZjaV1yNkw1YndPRm0uOVtOb3ZOd2U9In0=";

            // Act
            var actual = s.Serialize(sp);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Service_Principal_Is_Deserialized_From_Base64()
        {
            // Arrange
            var s = new KeyVaultSecretSerializer();
            var base64sp = "eyJDbGllbnRJZCI6IjY1ZTMxOGQ0LWQ2MTUtNDgyYy1hMThiLTIwMzQxOGQxZDI3OCIsIkNsaWVudFNlY3JldCI6IkZ0KnYyeFFwQzFJYjMraVVvS3N3WUdqVVdqNF1UXzFhIn0=";
            var expected = new ServicePrincipal("65e318d4-d615-482c-a18b-203418d1d278", "Ft*v2xQpC1Ib3+iUoKswYGjUWj4]T_1a");

            // Act
            var actual = s.Deserialize<ServicePrincipal>(base64sp);

            // Assert
            Assert.Equal(expected.ClientId, actual.ClientId);
            Assert.Equal(expected.ClientSecret, actual.ClientSecret);
        }

        [Fact]
        public void Enrollment_Account_Info_Serialize_To_JSON()
        {
            // Arrange
            var s = new KeyVaultSecretSerializer();
            var enrollmentsInfo = new List<EnrollmentAccountInfo>();
            enrollmentsInfo.Add(new EnrollmentAccountInfo()
            {
                EnrollmentAccountId = "f97e4d24-15e3-4e46-bb21-e3e8314c7ff9",
                KeyVaultName = "sppoccomkvt001",
                SecretName = "sp1clientsecret"
            });

            var expected = "W3siRW5yb2xsbWVudEFjY291bnRJZCI6ImY5N2U0ZDI0LTE1ZTMtNGU0Ni1iYjIxLWUzZTgzMTRjN2ZmOSIsIktleVZhdWx0TmFtZSI6InNwcG9jY29ta3Z0MDAxIiwiU2VjcmV0TmFtZSI6InNwMWNsaWVudHNlY3JldCJ9XQ=="; 

            // Act
            var actual = s.Serialize(enrollmentsInfo);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}