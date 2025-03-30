using DatabaseEngine.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.MediatorInstruction.Commands.UsersCommand.Queries
{
	public class GetSubscribersByChannelIdQuery : IRequest<List<User>>
	{
		public int NewsChannelId { get; set; }
	}
}
