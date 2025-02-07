using DatabaseEngine.RepositoryStorage.Interfaces;
using Microsoft.AspNetCore.Mvc;
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

		[HttpGet("all-channels")]
		public async Task<IActionResult> GetAllnewsChannels()
		{
			var result = await _newsChannelRepository.GetAllNewsChannels();

			return Ok(result);
		}
	}
}
