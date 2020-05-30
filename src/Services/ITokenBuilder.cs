using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Graph.RBAC.Fluent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace luis.azure.api.Services
{
    public interface ITokenBuilder
	{

        Task<string> getToken();
	}
}
