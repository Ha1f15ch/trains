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
	public class CreateNewNewsChannelHandler : IRequestHandler<CreateNewNewsChannelCommand, NewsChannel?>
	{
		private readonly INewsChannelRepository _newsChannelRepository;

		public CreateNewNewsChannelHandler(INewsChannelRepository newsChannelRepository)
		{
			_newsChannelRepository = newsChannelRepository;
		}

		public async Task<NewsChannel?> Handle(CreateNewNewsChannelCommand request, CancellationToken cancellationToken)
		{
			return await _newsChannelRepository.CreateNewNewsChannel(request.NewsChannel);
		}
	}
}
