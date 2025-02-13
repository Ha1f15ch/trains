using BusinesEngine.Events.Interfaces;
using Microsoft.Extensions.Logging;
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
		private readonly List<IEmailSubscriber> _emailSubscribers = new();

		public async Task NotifySubscribers(string message)
		{
			await Task.Run(async () => 
			{
				foreach (var subscriber in _subscribers)
				{
					await subscriber.Update(message);
				}

				foreach(var emailSubscriber in _emailSubscribers)
				{
					await emailSubscriber.Update(message);
				}
			});
		}

		public async Task Subscribe(INewsSubscriber subscriber)
		{
			await Task.Run(() =>
			{
				if(subscriber is IEmailSubscriber emailSubscriber)
				{
					_emailSubscribers.Add(emailSubscriber);
				}
				else
				{
					_subscribers.Add(subscriber);
				}
			});
		}

		public async Task Unsubscribe(INewsSubscriber subscriber)
		{
			await Task.Run(() =>
			{
				if (subscriber is IEmailSubscriber emailSubscriber)
				{
					_emailSubscribers.Remove(emailSubscriber);
				}
				else
				{
					_subscribers.Remove(subscriber);
				}
			});
		}
	}
}
