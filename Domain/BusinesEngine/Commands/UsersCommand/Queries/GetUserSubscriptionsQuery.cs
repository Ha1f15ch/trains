using DatabaseEngine.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.Commands.UsersCommand.Queries
{
	public class GetUserSubscriptionsQuery : IRequest<List<Subscription?>>
	{
		public int UserId { get; set; }
	}
}
