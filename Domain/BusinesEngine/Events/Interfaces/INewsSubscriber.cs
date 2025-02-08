using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.Events.Interfaces
{
	public interface INewsSubscriber
	{
		Task Update(string message);
	}
}
