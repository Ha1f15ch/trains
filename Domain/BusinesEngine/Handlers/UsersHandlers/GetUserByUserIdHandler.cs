using BusinesEngine.Commands.UsersCommand;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.Handlers.UsersHandlers
{
	public class GetUserByUserIdHandler : IRequestHandler<GetUserByIdCommand, User?>
	{
		private readonly IUserRepository _userRepository;

		public GetUserByUserIdHandler(IUserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		public async Task<User?> Handle(GetUserByIdCommand request, CancellationToken cancellationToken)
		{
			return await _userRepository.GetUserById(request.UserId);
		}
	}
}
