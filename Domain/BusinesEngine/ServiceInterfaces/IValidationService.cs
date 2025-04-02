using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.ServiceInterfaces
{
	public interface IValidationService
	{
		Task<string> SendValidationRequestAsync(object data);
		//Task<> GetValidationResultAsync(string requestId);
	}
}
