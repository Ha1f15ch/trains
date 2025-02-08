using BusinesEngine.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.Events
{
	public class NewsPublisher : INewsPublisher
	{
		private readonly List<INewsSubscriber> _subscribers = new();

		public async Task NotifySubscribers(string message)
		{
			await Task.Run(() => 
			{
				foreach (INewsSubscriber subscriber in _subscribers)
				{
					subscriber.Update(message);
				}
			});
		}

		public async Task Subscribe(INewsSubscriber subscriber)
		{
			await Task.Run(() =>
			{
				_subscribers.Add(subscriber);
			});
		}

		public async Task Unsubscribe(INewsSubscriber subscriber)
		{
			await Task.Run(() =>
			{
				_subscribers.Remove(subscriber);
			});
		}
	}
}
