using DatabaseEngine.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.Commands.UsersCommand
{
	public class UserSubscriptionToBookCommand : IRequest<Subscription>
	{
		public int UserId { get; set; }
		public int BookId { get; set; }
	}
}
