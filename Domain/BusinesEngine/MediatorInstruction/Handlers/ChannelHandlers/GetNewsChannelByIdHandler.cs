using BusinesEngine.MediatorInstruction.Commands.ChannelCommand;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.MediatorInstruction.Handlers.ChannelHandlers
{
	public class GetNewsChannelByIdHandler : IRequestHandler<GetNewsChannelByIdCommand, NewsChannel?>
	{
		private readonly INewsChannelRepository _channelRepository;

		public GetNewsChannelByIdHandler(INewsChannelRepository newsChannelRepository)
		{
			_channelRepository = newsChannelRepository;
		}

		public async Task<NewsChannel?> Handle(GetNewsChannelByIdCommand request, CancellationToken cancellationToken)
		{
			return await _channelRepository.GetNewsChannelById(request.NewsChannelId);
		}
	}
}
