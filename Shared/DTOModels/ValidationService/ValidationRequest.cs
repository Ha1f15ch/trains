using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModels.ValidationService
{
	public class ValidationRequest
	{
		public string RequestId { get; set; }
		public NewUserData Data { get; set; }
	}
}
