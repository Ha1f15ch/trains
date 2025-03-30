using BusinesEngine.MediatorInstruction.Commands.ChannelPost;
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
	public class CreateNewNewsChannelsPostHandler : IRequestHandler<CreateNewNewsChannelsPostCommand, NewsChannelsPosts?>
	{
		private readonly INewsChannelsPostsRepository _newsChannelsPostsRepository;

		public CreateNewNewsChannelsPostHandler(INewsChannelsPostsRepository newsChannelsPostsRepository)
		{
			_newsChannelsPostsRepository = newsChannelsPostsRepository;
		}

		public async Task<NewsChannelsPosts?> Handle(CreateNewNewsChannelsPostCommand request, CancellationToken cancellationToken)
		{
			return await _newsChannelsPostsRepository.CreateNewNewsChannelsPost(request.NewChannelId, request.TitlePost, request.BodyPost, request.FooterPost, request.AauthorPost, request.SourceImage);
		}
	}
}
