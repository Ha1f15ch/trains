using BusinesEngine.Events;
using BusinesEngine.Services;
using BusinesEngine.Services.ServiceInterfaces;
using DatabaseEngine.RepositoryStorage.Interfaces;
using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAppTrain.Controllers
{
	[Route("main/v1/")]
	[ApiController]
	[AllowAnonymous] //Временное решение, так как для реального рабочего Api необходимо настраивать авторизацию в заголовках 
	public class ObserverController : Controller
	{
		private readonly NewsPublisher _newsPublisher;
		private readonly LogSubscriber _logSubscriber;

		private readonly IUserRepository _userRepository;
		private readonly INewsChannelRepository _newsChannelRepository;
		private readonly INewsChannelsPostsRepository _newsChannelsPostsRepository;
		private readonly INewsChannelsSubscribersRepository _newsChannelsSubscribersRepository;

		private readonly EmailNotificationService _emailNotificationService;
		private readonly JsonStringHandlerService _jsonStringHandlerService;

		private readonly ILogService _logService;

		public ObserverController(
			NewsPublisher newsPublisher,
			LogSubscriber logSubscriber,
			IUserRepository userRepository, 
			INewsChannelRepository newsChannelRepository, 
			INewsChannelsPostsRepository newsChannelsPostsRepository, 
			INewsChannelsSubscribersRepository newsChannelsSubscribersRepository, 
			EmailNotificationService emailNotificationService, 
			JsonStringHandlerService jsonStringHandlerService,
			ILogService logService)
		{
			_newsPublisher = newsPublisher;
			_newsPublisher.Subscribe(logSubscriber);

			_userRepository = userRepository;
			_newsChannelRepository = newsChannelRepository;
			_newsChannelsPostsRepository = newsChannelsPostsRepository;
			_newsChannelsSubscribersRepository = newsChannelsSubscribersRepository;
			
			_logService = logService;
			_emailNotificationService = emailNotificationService;
			_jsonStringHandlerService = jsonStringHandlerService;
		}

		[HttpGet("channel/all")]
		public async Task<IActionResult> GetAllNewsChannels()
		{
			var result = await _newsChannelRepository.GetAllNewsChannels();

			// Пытаемся конвертировать данные в нормальный вид 
			var serializedResult = await _jsonStringHandlerService.SerializeList(result);

			return Ok(serializedResult);
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
		public async Task<IActionResult> CreateNewNewsChannel([FromBody] NewsChannelDto newsChannelDto)
		{
			_logService.LogInformation($"Создаем новостной канал");

			var newNewsChannel = await _newsChannelRepository.CreateNewNewsChannel(newsChannelDto);

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

		[HttpGet("channel/{channelId}/subscribers")]
		public async Task<IActionResult> GetNewsChannelSubscribers(int channelId)
		{
			var subscribers = await _newsChannelsSubscribersRepository.GetSubscribersByChannelId(channelId);

			return Ok(subscribers);
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

			var user = await _userRepository.GetUserById(userId);

			if(user != null && !string.IsNullOrEmpty(user.Email))
			{
				//отправить событие - в логи о том, что пользователь подписался на новостной канал
				await _newsPublisher.NotifySubscribers($"Пользователь {userId} подписался на канал - {channelId}");

				//Отправить письмо пользователю на почту о том, что он подписался
				await _emailNotificationService.SendEmailAsync(user.Email, "Добро пожаловать !!!", $"Рады видеть на Вашем канале. Надеемся Вы найдете у Нас много интересной и полезной информации. Дальнейшая отправка новостей и оповещений будет выполняться на данную почту - {user.Email}");
			}

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

			// Получаем email
			var subscribers = await _newsChannelsSubscribersRepository.GetSubscribersByChannelId(channelId);
			
			var subscribersEmailForLog = string.Join(", ", subscribers.Select(s => s.Email));

			// Уведомляем в логах 
			await _newsPublisher.NotifySubscribers($"новостной канал выпустил новый пост, - {channelId}. Заголовок - {title}. Список Email адресов пользователей - {subscribersEmailForLog}");

			if(subscribers is not null)
			{
				foreach(var user in subscribers)
				{
					if(!string.IsNullOrEmpty(user.Email))
					{
						_logService.LogInformation($"выполняется оповещение пользователей через Email - {user.Email}");
						await _emailNotificationService.SendEmailAsync(user.Email, "Новый пост в канале", $"Новостной канал с id = {channelId} опубликовал пост: \"{title}\".\n\n{bodyPost}");
					}
				}
			}

			return Ok(resultByCreate);
		}
	}
}
