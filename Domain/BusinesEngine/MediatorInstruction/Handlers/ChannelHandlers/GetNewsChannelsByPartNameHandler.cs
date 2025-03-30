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
	public class GetNewsChannelsByPartNameHandler : IRequestHandler<GetNewsChannelsByPartNameCommand, List<NewsChannel>>
	{
		private readonly INewsChannelRepository _channelRepository;

		public GetNewsChannelsByPartNameHandler(INewsChannelRepository newsChannelRepository)
		{
			_channelRepository = newsChannelRepository;
		}

		public async Task<List<NewsChannel>> Handle(GetNewsChannelsByPartNameCommand request, CancellationToken cancellationToken)
		{
			return await _channelRepository.GetNewsChannelsByPartName(request.PartChannelName);
		}
	}
}
