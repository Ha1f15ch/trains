using BusinesEngine.Commands.UsersCommand.Queries;
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
	public class GetUserSubscriptionsHandler : IRequestHandler<GetUserSubscriptionsQuery, List<Subscription?>>
	{
		private readonly ISubscriptionRepository _subscriptionRepository;

		public GetUserSubscriptionsHandler(ISubscriptionRepository subscriptionRepository)
		{
			_subscriptionRepository = subscriptionRepository;
		}

		public async Task<List<Subscription?>> Handle(GetUserSubscriptionsQuery request, CancellationToken cancellationToken)
		{
			return await _subscriptionRepository.GetUserSubscriptions(request.UserId);
		}
	}
}
