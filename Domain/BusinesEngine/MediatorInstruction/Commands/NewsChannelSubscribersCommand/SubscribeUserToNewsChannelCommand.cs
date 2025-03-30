using DatabaseEngine.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.MediatorInstruction.Commands.NewsChannelSubscribersCommand
{
	public class SubscribeUserToNewsChannelCommand : IRequest<NewsChannelsSubscribers>
	{
		public int UserId { get; set; }
		public int NewsChannelId { get; set; }
	}
}
