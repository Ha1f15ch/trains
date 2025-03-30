using DatabaseEngine.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.MediatorInstruction.Commands.ChannelCommand
{
	public class GetNewsChannelsByPartNameCommand : IRequest<List<NewsChannel>>
	{
		required public string PartChannelName { get; set; }
	}
}
