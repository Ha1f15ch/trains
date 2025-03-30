using BusinesEngine.MediatorInstruction.Commands.BookCommand.Queries;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.MediatorInstruction.Handlers.BookHandlers
{
	public class GetAllBooksHandler : IRequestHandler<GetAllBooksQuery, List<Book?>>
	{
		private readonly IBookRepository _bookRepository;

		public GetAllBooksHandler(IBookRepository bookRepository)
		{
			_bookRepository = bookRepository;
		}

		public async Task<List<Book?>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
		{
			return await _bookRepository.GetAllBooks();
		}
	}
}
