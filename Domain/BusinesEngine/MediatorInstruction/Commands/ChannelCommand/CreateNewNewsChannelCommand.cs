using DatabaseEngine.Models;
using DTOModels;
using MediatR;

namespace BusinesEngine.MediatorInstruction.Commands.ChannelCommand
{
	public class CreateNewNewsChannelCommand : IRequest<NewsChannel?>
	{
		required public NewsChannelDto NewsChannel { get; set; }
	}
}
