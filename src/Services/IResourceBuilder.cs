using Microsoft.Azure.Management.ResourceManager.Models;
using System.Threading.Tasks;

namespace luis.azure.api.Services
{
    public interface IResourceBuilder
	{

        Task<ResourceGroup> CreateResourceGroup(
            string token,
            string subscriptionId,
            string resourceGroupName,
            string resourceGroupLocation);

    }
}
