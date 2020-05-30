using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace luis.azure.api.Features.Health
{
	/// <summary>
	/// API Controller for Health operations
	/// </summary>
	[Route("api/health")]
    [ApiController]
    public class HealthController : ControllerBase
    {
		/// <summary>
		/// Health Probe
		/// </summary>
		/// <returns>"pong" as a health probe</returns>
		// POST api/health
		[HttpGet]
		[AllowAnonymous]
		[ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(string))]
		public IActionResult Ping()
		{
			return new OkObjectResult("pong");
		}
	}
}