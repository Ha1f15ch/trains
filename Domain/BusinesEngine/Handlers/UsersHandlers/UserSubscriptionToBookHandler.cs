using BusinesEngine.Commands.UsersCommand;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.Handlers.UsersHandlers
{
	public class UserSubscriptionToBookHandler : IRequestHandler<UserSubscriptionToBookCommand, Subscription>
	{
		private readonly ISubscriptionRepository _subscriptionRepository;

		public UserSubscriptionToBookHandler(ISubscriptionRepository subscriptionRepository)
		{
			_subscriptionRepository = subscriptionRepository;
		}

		public async Task<Subscription> Handle(UserSubscriptionToBookCommand request, CancellationToken cancellationToken)
		{
			return await _subscriptionRepository.SubscribeUserToBook(request.UserId, request.BookId);
		}
	}
}
