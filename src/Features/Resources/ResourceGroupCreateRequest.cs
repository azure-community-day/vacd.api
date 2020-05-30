using FluentValidation;
using MediatR;

namespace luis.azure.api.Features.Resources
{
    /// <summary>
    /// Request for creating a Service Principal
    /// </summary>
    public class ResourceGroupCreateRequest : IRequest<ResourceCreateResponse>
    {
        /// <summary>
        /// The ID of the subscription where to create the resource
        /// </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// The friendly name of the resource group to be created
        /// </summary>
        public string ResourceGroupName { get; set; }

        /// <summary>
        /// The location of the resource group to be created
        /// </summary>
        public string ResourceGroupLocation { get; set; }

    }

    public class ResourceGroupCreateRequestValidator : AbstractValidator<ResourceGroupCreateRequest>
    {
        public ResourceGroupCreateRequestValidator()
        {
            RuleFor(m => m.SubscriptionId).NotEmpty().WithMessage($"The field {nameof(ResourceGroupCreateRequest.SubscriptionId)} can not be empty");
            RuleFor(m => m.ResourceGroupName).NotEmpty().WithMessage($"The field {nameof(ResourceGroupCreateRequest.ResourceGroupName)} can not be empty");
            RuleFor(m => m.ResourceGroupLocation).NotEmpty().WithMessage($"The field {nameof(ResourceGroupCreateRequest.ResourceGroupLocation)} can not be empty");

        }
    }
}