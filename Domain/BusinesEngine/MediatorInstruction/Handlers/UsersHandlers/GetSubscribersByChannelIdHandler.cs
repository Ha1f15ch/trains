using BusinesEngine.MediatorInstruction.Commands.UsersCommand.Queries;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.MediatorInstruction.Handlers.UsersHandlers
{
	public class GetSubscribersByChannelIdHandler : IRequestHandler<GetSubscribersByChannelIdQuery, List<User>>
	{
		private readonly INewsChannelsSubscribersRepository _newsChannelsSubscribersRepository;

		public GetSubscribersByChannelIdHandler(INewsChannelsSubscribersRepository newsChannelsSubscribersRepository)
		{
			_newsChannelsSubscribersRepository = newsChannelsSubscribersRepository;
		}

		public async Task<List<User>> Handle(GetSubscribersByChannelIdQuery request, CancellationToken cancellationToken)
		{
			return await _newsChannelsSubscribersRepository.GetSubscribersByChannelId(request.NewsChannelId);
		}
	}
}
