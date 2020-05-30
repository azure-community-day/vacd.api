using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Rest;
using System;
using System.Threading.Tasks;

namespace luis.azure.api.Services
{
    public class ResourceBuilder : IResourceBuilder
    {
		private readonly IConfiguration _config;

		public ResourceBuilder(IConfiguration config)
		{
			_config = config;
		}

        /// <summary>
        /// Creates a resource group.
        /// </summary>
        /// <param name="token">The brearer token acquired againts Azure Management API to use when creating the resource group.</param>
        /// <param name="subscriptionId">The subscription id under which the resource group will be created.</param>
        /// <param name="resourceGroupName">The name that will be given to the new resource group.</param>
        /// <param name="resourceGroupLocation">The location where the resource group will be created.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task"/> object that represents the asynchronous operation.</returns>
        public async Task<ResourceGroup> CreateResourceGroup(
            string token,
            string subscriptionId,
            string resourceGroupName,
            string resourceGroupLocation)
        {
            ServiceClientCredentials creds = new TokenCredentials(token);
            var resourceGroup = new ResourceGroup();
            using (ResourceManagementClient resourceManagementClient = new ResourceManagementClient(creds))
            {
                resourceManagementClient.SubscriptionId = subscriptionId;
                bool? existsResult = await resourceManagementClient.ResourceGroups.CheckExistenceAsync(resourceGroupName);
                if (existsResult == null || !existsResult.Value)
                {
                    Console.WriteLine("Creating resource group {0}", resourceGroupName);
                    resourceGroup = await resourceManagementClient.ResourceGroups.CreateOrUpdateAsync(resourceGroupName, new ResourceGroup(resourceGroupLocation));
                    Console.WriteLine("Resource group created");
                    Console.WriteLine();
                }
            }
            
            return resourceGroup;
        }
    }
}