using BusinesEngine.MediatorInstruction.Commands.ChannelCommand;
using BusinesEngine.MediatorInstruction.Commands.ChannelCommand.Queries;
using BusinesEngine.MediatorInstruction.Commands.ChannelPost;
using BusinesEngine.MediatorInstruction.Commands.ChannelPost.Queries;
using BusinesEngine.MediatorInstruction.Commands.NewsChannelSubscribersCommand;
using BusinesEngine.MediatorInstruction.Commands.UsersCommand.Queries;
using DTOModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAppTrain.Controllers
{
	[Route("main/v1/")]
	[ApiController]
	[AllowAnonymous] //Временное решение, так как для реального рабочего Api необходимо настраивать авторизацию в заголовках 
	public class ObserverController : Controller
	{
		private readonly IMediator _mediator;

		public ObserverController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet("channels/all")]
		public async Task<IActionResult> GetAllNewsChannels()
		{
			var command = new GetAllNewsChannelsQuery();

			var result = await _mediator.Send(command);

			return Ok(result);
		}

		[HttpGet("channel/{channelId}")]
		public async Task<IActionResult> GetNewsChannelById(int channelId)
		{
			var command = new GetNewsChannelByIdCommand { NewsChannelId = channelId };

			var result = await _mediator.Send(command);

			return Ok(result);
		}

		[HttpPost("channel/find-by-partName/{partName}")]
		public async Task<IActionResult> GetNewsChannelsByPartName(string partName)
		{
			var command = new GetNewsChannelsByPartNameCommand { PartChannelName = partName };

			var result = await _mediator.Send(command);

			return Ok(result);
		}

		[HttpPost("channel/find-by-name/{channelName}")]
		public async Task<IActionResult> GetNewsChannelsByName(string channelName)
		{
			var command = new GetNewsChannelByNameCommand { FullNewsChannelName = channelName };

			var result = await _mediator.Send(command);

			return Ok(result);
		}

		[HttpPost("channel/create-new")]
		public async Task<IActionResult> CreateNewNewsChannel([FromBody] NewsChannelDto newsChannelDto)
		{
			var command = new CreateNewNewsChannelCommand { NewsChannel = newsChannelDto };

			var result = await _mediator.Send(command);

			return Ok(result);
		}

		[HttpGet("channel/{channelId}/all-posts")]
		public async Task<IActionResult> GetAllPostsByChannelId(int channelId)
		{
			var command = new GetAllPostsByNewsChannelIdQuery { NewsChannelId = channelId };

			var result = await _mediator.Send(command);

			return Ok(result);
		}

		[HttpGet("channel/{channelId}/subscribers")]
		public async Task<IActionResult> GetNewsChannelSubscribers(int channelId)
		{
			var command = new GetSubscribersByChannelIdQuery { NewsChannelId = channelId };

			var result = await _mediator.Send(command);

			return Ok(result);
		}

		[HttpPost("channel/{channelId}/subscribe")]
		public async Task<IActionResult> SubscribeToNewsChannel(int userId, int channelId)
		{
			var command = new SubscribeUserToNewsChannelCommand { UserId = userId, NewsChannelId= channelId };

			var result = await _mediator.Send(command);

			return Ok(result);
		}

		[HttpPost("posts/find-by-partTitle/{partTitle}")]
		public async Task<IActionResult> GetPostsByTitlePart(string partTitle)
		{
			var command = new GetAllPostsByPartTitleQuery { PartNewsChannelPostsName = partTitle };

			var result = await _mediator.Send(command);

			return Ok(result);
		}

		[HttpPost("channel/{channelId}/create-post")]
		public async Task<IActionResult> CreateNewNewsChannelPost(int channelId, string title, string bodyPost, string? footerPost, string authorPost, string? sourceImage)
		{
			var command = new CreateNewNewsChannelsPostCommand
			{
				NewsChannelId = channelId,
				TitlePost = title,
				BodyPost = bodyPost,
				FooterPost = footerPost,
				AauthorPost = authorPost,
				SourceImage = sourceImage
			};

			var result = await _mediator.Send(command);

			return Ok(result);
		}
	}
}
