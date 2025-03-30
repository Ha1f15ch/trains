using DatabaseEngine.Models;
using MediatR;

namespace BusinesEngine.MediatorInstruction.Commands.UsersCommand.Queries
{
	public class GetUserSubscriptionsQuery : IRequest<List<Subscription>>
	{
		public int UserId { get; set; }
	}
}
