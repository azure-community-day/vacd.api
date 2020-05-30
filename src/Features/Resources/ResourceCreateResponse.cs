using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace luis.azure.api.Features.Resources
{
    public class ResourceCreateResponse
    {
        /// <summary>
        /// The id of teh resource created
        /// </summary>
        public string ResourceId { get; set; }

        /// <summary>
        /// The name of the resource created
        /// </summary>
        public string ResourceName { get; set; }
    }
}
