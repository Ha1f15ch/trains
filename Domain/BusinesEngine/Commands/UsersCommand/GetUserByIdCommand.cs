using DatabaseEngine.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.Commands.UsersCommand
{
	public class GetUserByIdCommand : IRequest<User?>
	{
		public int UserId { get; set; }
	}
}
