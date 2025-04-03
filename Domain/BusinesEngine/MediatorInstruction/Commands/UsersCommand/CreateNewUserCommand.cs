using MediatR;
using DatabaseEngine.Models;

namespace BusinesEngine.MediatorInstruction.Commands.UsersCommand
{
	public class CreateNewUserCommand : IRequest<string?>
	{
		public string Name { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public bool IsActive { get; set; }
		public DateTime DateCreate { get; set; }
		public DateTime DateUpdate { get; set; }
		public DateTime DateDelete { get; set; }
	}
}
