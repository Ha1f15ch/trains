using DatabaseEngine.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.MediatorInstruction.Commands.ChannelCommand
{
	public class GetNewsChannelByIdCommand : IRequest<NewsChannel?>
	{
		public int NewsChannelId { get; set; }
	}
}
