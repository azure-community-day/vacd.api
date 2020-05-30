using System;
using IAC.AZ.Tools.ServicePrincipalManager.Models.AzureRestApi;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using IAC.AZ.Tools.ServicePrincipalManager.Extensions;
using IAC.AZ.Tools.ServicePrincipalManager.Models;
using Xunit;

namespace IAC.AZ.Tools.ServicePrincipalManager.Tests.IntegrationTests
{
    public class BillingAccountApiIntegrationTests
    {
        [Fact]
        public async Task Query_Billing_Account_API()
        {
            var servicePrincipal =
                new ServicePrincipal("04b40527-4ef6-40ba-9db4-f954ee905f92", "d77d75c3-7c1d-4351-8b6d-195b4d550c31");

            var accessToken = await servicePrincipal.AcquireTokenAsync(
                "https://login.microsoftonline.com/35595a02-4d6d-44ac-99e1-f9ab4cd872db",
                @"https://management.azure.com");

            var azureManagementBaseUrl = @"https://management.azure.com";
            var enrollmentAccountsBaseUrl = $"{azureManagementBaseUrl}/providers/Microsoft.Billing/enrollmentAccounts";
            var enrollmentAccountsApiUrl = $"{enrollmentAccountsBaseUrl}?api-version=2018-03-01-preview";

            var isOwner = false;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var enrollmentAccountsApiResponse = client.GetStringAsync($"{enrollmentAccountsApiUrl}").Result;
                var enrollmentAccountsList = JsonConvert.DeserializeObject<EnrollmentAccountsList>(enrollmentAccountsApiResponse);

                foreach (var enrollmentAccount in enrollmentAccountsList.Value)
                {
                    var roleAssignmentUrl = $"{enrollmentAccountsBaseUrl}/{enrollmentAccount.Name}/providers/Microsoft.Authorization/roleAssignments?api-version=2015-07-01";
                    var roleAssignmentApiResponse = client.GetStringAsync(roleAssignmentUrl).Result;

                    var roleAssignmentList = JsonConvert.DeserializeObject<RoleAssignmentList>(roleAssignmentApiResponse);

                    foreach (var roleAssignment in roleAssignmentList.Value)
                    {
                        if (roleAssignment.Properties.PrincipalId == "d2718f39-c0bd-4eb0-bedd-8abd08cc03ce")
                        {
                            var roleDefinitionApiResponse = 
                                client.GetStringAsync($"{azureManagementBaseUrl}/{roleAssignment.Properties.RoleDefinitionId}?api-version=2015-07-01").Result;

                            var role = JsonConvert.DeserializeObject<RoleDefinition>(roleDefinitionApiResponse);

                            if (role.Properties.RoleName == "Owner")
                            {
                                isOwner = true;
                                break;
                            }
                        }
                    }
                }
            }

            Assert.Equal(isOwner, true);
        }

        [Fact]
        public async Task Make_Enrollment_Account_Owner_and_Billing_Reader()
        {
            var servicePrincipal =
                new ServicePrincipal("04b40527-4ef6-40ba-9db4-f954ee905f92", "d77d75c3-7c1d-4351-8b6d-195b4d550c31");

            var accessToken = await servicePrincipal.AcquireTokenAsync(
                "https://login.microsoftonline.com/35595a02-4d6d-44ac-99e1-f9ab4cd872db",
                @"https://management.azure.com");

            var azureManagementBaseUrl = @"https://management.azure.com";
            var enrollmentAccountsBaseUrl = $"{azureManagementBaseUrl}/providers/Microsoft.Billing/enrollmentAccounts";

            var isOwner = false;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var roleAssignmentGuid = Guid.NewGuid().ToString();

                var enrollmentAccountName = "b27a179d-c43c-41ed-95d5-0601d81f4d07";
                var roleAssignmentUrl = 
                    $"{enrollmentAccountsBaseUrl}/{enrollmentAccountName}/providers/Microsoft.Authorization/roleAssignments/{roleAssignmentGuid}?api-version=2015-07-01";

                var enrollmentAccountRoleAssignments = new EnrollmentAccountRoleAssignments()
                {
                    Properties = new EnrollmentAccountRoleAssignmentsProperties()
                    {
                        PrincipalId = "d2718f39-c0bd-4eb0-bedd-8abd08cc03ce",
                        RoleDefinitionId = "/providers/Microsoft.Authorization/roleDefinitions/fa23ad8b-c56e-40d8-ac0c-ce449e1d2c64"
                    }
                };

                var json = JsonConvert.SerializeObject(enrollmentAccountRoleAssignments);

                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var enrollmentAccountsApiResponse = 
                    await client.PutAsync($"{roleAssignmentUrl}", httpContent);

                var response = enrollmentAccountsApiResponse.Content.ReadAsStringAsync();
            }

            Assert.Equal(isOwner, true);
        }

        [Fact]
        public async Task Check()
        {
            var clientCredential =
                new ClientCredential("04b40527-4ef6-40ba-9db4-f954ee905f92", "d77d75c3-7c1d-4351-8b6d-195b4d550c31");

            AuthenticationContext context =
                new AuthenticationContext("https://login.microsoftonline.com/35595a02-4d6d-44ac-99e1-f9ab4cd872db",
                    false);

            AuthenticationResult authenticationResult = context.AcquireTokenAsync(
                    "https://management.azure.com", clientCredential)
                .GetAwaiter()
                .GetResult();
            
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);

                var subscriptionId = "d16aad36-13af-4e8b-a522-f2701712b375";
                var subscriptionsUrl =
                    $"https://management.azure.com/subscriptions/{subscriptionId}/providers/Microsoft.Commerce/UsageAggregates?api-version=2019-06-01/Daily_BRSDF_20140501_0000";

                var subscriptionBilling = client.GetAsync(subscriptionsUrl).Result;
            }
        }
    }
}