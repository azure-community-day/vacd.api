using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace luis.azure.api.Features.Resources
{
    /// <summary>
    /// API Controller for resources operations
    /// </summary>
    [Route("api/resources")]
    [ApiController]
    public class ResourcesController : ControllerBase
    {
		private readonly IMediator _mediator;

		public ResourcesController(IMediator mediator)
		{
			_mediator = mediator;
		}

        /// <summary>
        /// <summary>
        /// Create a new ResourceGroup
        /// </summary>
        /// <param name="request">Resource Group info</param>
        /// <returns>Info about created Resource Group</returns>
        // POST api/resources/resourceGroup
        [AllowAnonymous]
        [HttpPost("resourceGroup")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ResourceCreateResponse))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(ExceptionProblemDetails))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ExceptionProblemDetails))]
        public async Task<IActionResult> Create([FromBody] ResourceGroupCreateRequest request)
        {
            var result = await _mediator.Send(request);
            return new OkObjectResult(result);
        }
    }
}