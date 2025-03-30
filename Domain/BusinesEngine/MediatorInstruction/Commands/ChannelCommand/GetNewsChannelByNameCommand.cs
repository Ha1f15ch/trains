using DatabaseEngine.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.MediatorInstruction.Commands.ChannelCommand
{
	public class GetNewsChannelByNameCommand : IRequest<NewsChannel?>
	{
		required public string FullNewsChannelName { get; set; }
	}
}
