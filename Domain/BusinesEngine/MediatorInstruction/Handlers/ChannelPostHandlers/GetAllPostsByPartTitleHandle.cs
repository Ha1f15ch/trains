using BusinesEngine.MediatorInstruction.Commands.ChannelPost.Queries;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.MediatorInstruction.Handlers.ChannelPostHandlers
{
	public class GetAllPostsByPartTitleHandle : IRequestHandler<GetAllPostsByPartTitleQuery, List<NewsChannelsPosts>>
	{
		private readonly INewsChannelsPostsRepository _newsChannelsPostsRepository;

		public GetAllPostsByPartTitleHandle(INewsChannelsPostsRepository newsChannelsPostsRepository)
		{
			_newsChannelsPostsRepository = newsChannelsPostsRepository;
		}

		public async Task<List<NewsChannelsPosts>> Handle(GetAllPostsByPartTitleQuery request, CancellationToken cancellationToken)
		{
			return await _newsChannelsPostsRepository.GetAllPostsByPartTitle(request.PartNewsChannelPostsName);
		}
	}
}
