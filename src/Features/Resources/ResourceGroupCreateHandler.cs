using luis.azure.api.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace luis.azure.api.Features.Resources
{
    /// <summary>
    /// Create a Service Principal Level 1
    /// </summary>
    public class ResourceGroupCreateHandler : IRequestHandler<ResourceGroupCreateRequest, ResourceCreateResponse>
    {
        private readonly IConfiguration  _configuration;
        private readonly IResourceBuilder _resourceBuilder;
        private readonly ITokenBuilder _tokenBuilder;

        public ResourceGroupCreateHandler(
            IConfiguration configuration,
            IResourceBuilder resourceBuilder,
            ITokenBuilder tokenBuilder)
        {
            _configuration = configuration;
            _resourceBuilder = resourceBuilder;
            _tokenBuilder = tokenBuilder;
        }

        public async Task<ResourceCreateResponse> Handle(
            ResourceGroupCreateRequest request,
            CancellationToken cancellationToken)
        {

            var token = await _tokenBuilder.getToken();

            var result = await _resourceBuilder.CreateResourceGroup(
                token,
                request.SubscriptionId,
                request.ResourceGroupName,
                request.ResourceGroupLocation);

            return new ResourceCreateResponse
            {
                ResourceId = result.Id,
                ResourceName = result.Name
            };
        }
    }
}