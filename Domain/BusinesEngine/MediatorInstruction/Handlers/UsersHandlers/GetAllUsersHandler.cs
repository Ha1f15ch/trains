using BusinesEngine.MediatorInstruction.Commands.UsersCommand.Queries;
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
	public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, List<User?>>
	{
		private readonly IUserRepository _userRepository;

		public GetAllUsersHandler(IUserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		public async Task<List<User?>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
		{
			return await _userRepository.GetAllUsers();
		}
	}
}
