using BusinesEngine.Events;
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
		private readonly NewsPublisher _newsPublisher = new();
		private readonly LogSubscriber _logSubscriber;

		private readonly IUserRepository _userRepository;
		private readonly INewsChannelRepository _newsChannelRepository;
		private readonly INewsChannelsPostsRepository _newsChannelsPostsRepository;
		private readonly INewsChannelsSubscribersRepository _newsChannelsSubscribersRepository;
		private readonly LogService _logService;

		public ObserverController(IUserRepository userRepository, INewsChannelRepository newsChannelRepository, INewsChannelsPostsRepository newsChannelsPostsRepository, INewsChannelsSubscribersRepository newsChannelsSubscribersRepository, LogService logService, ILogger<LogSubscriber> logger)
		{
			_logService = logService;
			_userRepository = userRepository;
			_newsChannelRepository = newsChannelRepository;
			_newsChannelsPostsRepository = newsChannelsPostsRepository;
			_newsChannelsSubscribersRepository = newsChannelsSubscribersRepository;
			_logSubscriber = new LogSubscriber(logger);
			_newsPublisher.Subscribe(_logSubscriber);
		}

		[HttpGet("channel/all")]
		public async Task<IActionResult> GetAllNewsChannels()
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
			var foundedNewsChannels = await _newsChannelRepository.GetNewsChannelsByPartName(partName);

			return Ok(foundedNewsChannels);
		}

		[HttpPost("channel/find-by-name/{channelName}")]
		public async Task<IActionResult> GetNewsChannelsByName(string channelName)
		{
			var foundedNewsChannels = await _newsChannelRepository.GetNewsChannelByName(channelName);

			return Ok(foundedNewsChannels);
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

		[HttpPost("channel/{channelId}/subscribe")]
		public async Task<IActionResult> SubscribeToNewsChannel(int userId, int channelId)
		{
			_logService.LogInformation($"User with Id = {userId} подписывается на новостной канал с Id = {channelId}");

			var resultSubscription = await _newsChannelsSubscribersRepository.SubscribeUserToNewsChannel(userId, channelId);

			if(resultSubscription is null)
			{
				_logService.LogError($"{SubscribeToNewsChannel} - {nameof(SubscribeToNewsChannel)} - Подписка не была выполнена успешно. resultSubscription = {resultSubscription}");

				return BadRequest(404);
			}

			//отправить событие - Вы подписались
			await _newsPublisher.NotifySubscribers($"Пользователь {userId} подписался на канал - {channelId}");

			return Ok(resultSubscription);
		}

		[HttpPost("posts-by-title/{partTitle}")]
		public async Task<IActionResult> GetPostsByTitlePart(string partTitle)
		{
			var postsByPart = await _newsChannelsPostsRepository.GetAllPostsByPartTitle(partTitle);
			
			return Ok(postsByPart);
		}

		[HttpPost("channel/{channelId}/create-post")]
		public async Task<IActionResult> CreateNewNewsChannelPost(int channelId, string title, string bodyPost, string? footerPost, string authorPost, string? sourceImage)
		{
			_logService.LogInformation($"{CreateNewNewsChannelPost} - {nameof(CreateNewNewsChannelPost)} - Создаем запись для новостного канала");

			var resultByCreate = await _newsChannelsPostsRepository.CreateNewNewsChannelsPost(channelId, title, bodyPost, footerPost, authorPost, sourceImage);

			if(resultByCreate is null)
			{
				return BadRequest(404);
			}
			// Уведомляем подписчиков 
			await _newsPublisher.NotifySubscribers($"новостной канал выпустил новый пост, скорее посмотрите. Канал - {channelId}. Заголовок - {title}");

			return Ok(resultByCreate);
		}
	}
}
