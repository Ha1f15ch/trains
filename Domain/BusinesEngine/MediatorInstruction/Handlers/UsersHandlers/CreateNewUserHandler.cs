using BusinesEngine.MediatorInstruction.Commands.UsersCommand;
using BusinesEngine.Services;
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
	public class CreateNewUserHandler : IRequestHandler<CreateNewUserCommand, string>
	{
		private readonly IUserRepository _userRepository;
		private readonly JsonStringHandlerService _jsonStringHandlerService;

		public CreateNewUserHandler(IUserRepository userRepository, JsonStringHandlerService jsonStringHandlerService)
		{
			_userRepository = userRepository;
			_jsonStringHandlerService = jsonStringHandlerService;
		}

		public async Task<string> Handle(CreateNewUserCommand request, CancellationToken cancellationToken)
		{
			var newUser = await _userRepository.CreateNewUser(request.Name,
				request.Email,
				request.Password,
				request.IsActive,
				request.DateCreate,
				request.DateUpdate,
				request.DateDelete);

			return await _jsonStringHandlerService.SerializeSingle(newUser);
		}
	}
}
