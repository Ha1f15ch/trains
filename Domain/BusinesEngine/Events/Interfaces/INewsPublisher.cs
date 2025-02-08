using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.Events.Interfaces
{
	public interface INewsPublisher
	{
		Task Subscribe(INewsSubscriber subscriber);
		Task Unsubscribe(INewsSubscriber subscriber);
		Task NotifySubscribers(string message);
	}
}
