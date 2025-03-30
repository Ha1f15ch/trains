using BusinesEngine.MediatorInstruction.Commands.NewsChannelSubscribersCommand;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.MediatorInstruction.Handlers.NewsChannelSubscribersHandlers
{
	public class SubscribeUserToNewsChannelHandler : IRequestHandler<SubscribeUserToNewsChannelCommand, NewsChannelsSubscribers>
	{
		private readonly INewsChannelsSubscribersRepository _newsChannelsSubscribersRepository;

		public SubscribeUserToNewsChannelHandler(INewsChannelsSubscribersRepository newsChannelsSubscribersRepository)
		{
			_newsChannelsSubscribersRepository = newsChannelsSubscribersRepository;
		}

		public async Task<NewsChannelsSubscribers> Handle(SubscribeUserToNewsChannelCommand request, CancellationToken cancellationToken)
		{
			return await _newsChannelsSubscribersRepository.SubscribeUserToNewsChannel(request.UserId, request.NewsChannelId);
		}
	}
}
