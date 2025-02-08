using DatabaseEngine.RepositoryStorage.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Channels;
using WebApiApp.LogInfrastructure;

namespace WebAppTrain.Controllers
{
	[ApiController]
	[Route("main/v1/")]
	public class ObserverController : Controller
	{
		private readonly IUserRepository _userRepository;
		private readonly INewsChannelRepository _newsChannelRepository;
		private readonly INewsChannelsPostsRepository _newsChannelsPostsRepository;
		private readonly INewsChannelsSubscribersRepository _newsChannelsSubscribersRepository;
		private readonly LogService _logService;

		public ObserverController(IUserRepository userRepository, INewsChannelRepository newsChannelRepository, INewsChannelsPostsRepository newsChannelsPostsRepository, INewsChannelsSubscribersRepository newsChannelsSubscribersRepository, LogService logService)
		{
			_logService = logService;
			_userRepository = userRepository;
			_newsChannelRepository = newsChannelRepository;
			_newsChannelsPostsRepository = newsChannelsPostsRepository;
			_newsChannelsSubscribersRepository = newsChannelsSubscribersRepository;
		}

		[HttpGet("channel/all")]
		public async Task<IActionResult> GetAllnewsChannels()
		{
			var result = await _newsChannelRepository.GetAllNewsChannels();

			return Ok(result);
		}

		[HttpGet("channel/{channelId}")]
		public async Task<IActionResult> GetNewsChannelById(int channelId)
		{
			var result = await _newsChannelRepository.GetNewsChannelById(channelId);

			if(result is null)
			{
				return BadRequest(404);
			}

			return Ok(result);
		}

		[HttpPost("channel/find-by-partName/{partName}")]
		public async Task<IActionResult> GetNewsChannelsByPartName(string partName)
		{
			var findedNewsChannels = await _newsChannelRepository.GetNewsChannelsByPartName(partName);

			return Ok(findedNewsChannels);
		}

		[HttpPost("channel/find-by-name/{channelName}")]
		public async Task<IActionResult> GetNewsChannelsByName(string channelName)
		{
			var findedNewsChannels = await _newsChannelRepository.GetNewsChannelByName(channelName);

			return Ok(findedNewsChannels);
		}

		[HttpPost("channel/create-new")]
		public async Task<IActionResult> CreateNewNewsChannel(string name, string? description)
		{
			_logService.LogInformation($"Создаем новостной канал");

			var newNewsChannel = await _newsChannelRepository.CreateNewNewsChannel(name, description);

			if(newNewsChannel is null)
			{
				_logService.LogError($"Переданы некорректные данные для создания записи");

				return BadRequest(404);
			}

			_logService.LogInformation($"Успешно создано");

			return Ok(newNewsChannel);
		}

		[HttpGet("channel/{channelId}/all-posts")]
		public async Task<IActionResult> GetAllPostsByChannelId(int channelId)
		{
			var posts = await _newsChannelsPostsRepository.GetAllPostsByNewsChannelId(channelId);

			return Ok(posts);
		}

		[HttpPost("posts-by-title/{partTitle}")]
		public async Task<IActionResult> GetPostsByTitlePatr(string partTitle)
		{
			var postsByPart = await _newsChannelsPostsRepository.GetAllPostsByPartTitle(partTitle);
			
			return Ok(postsByPart);
		}

		[HttpPost("channel/{channelId}/create-post")]
		public async Task<IActionResult> CreateNewNewsChannelPost(int channelId, string title, string bodyPost, string? footerPost, string authorPost, string? sourceImage)
		{
			_logService.LogInformation($"Создлаем запись для новостного канала");

			var resultByCreate = await _newsChannelsPostsRepository.CreateNewNewsChannelsPost(channelId, title, bodyPost, footerPost, authorPost, sourceImage);

			if(resultByCreate is null)
			{
				return BadRequest(404);
			}

			return Ok(resultByCreate);
		}


	}
}
