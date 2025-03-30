using BusinesEngine.MediatorInstruction.Commands.UsersCommand;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.MediatorInstruction.Handlers.UsersHandlers
{
	public class CreateNewUserHandler : IRequestHandler<CreateNewUserCommand, User>
	{
		private readonly IUserRepository _userRepository;

		public CreateNewUserHandler(IUserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		public async Task<User> Handle(CreateNewUserCommand request, CancellationToken cancellationToken)
		{
			return await _userRepository.CreateNewUser(request.Name,
				request.Email,
				request.Password,
				request.IsActive,
				request.DateCreate,
				request.DateUpdate,
				request.DateDelete);
		}
	}
}
