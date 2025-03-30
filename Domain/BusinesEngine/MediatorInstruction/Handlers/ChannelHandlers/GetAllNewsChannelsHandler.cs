using BusinesEngine.MediatorInstruction.Commands.ChannelCommand.Queries;
using BusinesEngine.Services;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using MediatR;

namespace BusinesEngine.MediatorInstruction.Handlers.ChannelHandlers
{
	public class GetAllNewsChannelsHandler : IRequestHandler<GetAllNewsChannelsQuery, string>
	{
		private readonly INewsChannelRepository _newsChannelRepository;
		private readonly JsonStringHandlerService _jsonStringHandlerService;

		public GetAllNewsChannelsHandler(INewsChannelRepository newsChannelRepository, JsonStringHandlerService jsonStringHandlerService)
		{
			_newsChannelRepository = newsChannelRepository;
			_jsonStringHandlerService = jsonStringHandlerService;
		}

		public async Task<string> Handle(GetAllNewsChannelsQuery request, CancellationToken cancellationToken)
		{
			var listUsers = await _newsChannelRepository.GetAllNewsChannels();
			return await _jsonStringHandlerService.SerializeList(listUsers);
		}
	}
}
