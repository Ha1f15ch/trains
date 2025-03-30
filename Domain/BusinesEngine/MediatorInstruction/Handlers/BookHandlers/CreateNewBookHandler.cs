using BusinesEngine.MediatorInstruction.Commands.BookCommand;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using MediatR;

namespace BusinesEngine.MediatorInstruction.Handlers.BookHandlers
{
	public class CreateNewBookHandler : IRequestHandler<CreateNewBookCommand, Book?>
	{
		private readonly IBookRepository _bookRepository;

		public CreateNewBookHandler(IBookRepository bookRepository)
		{
			_bookRepository = bookRepository;
		}

		public async Task<Book?> Handle(CreateNewBookCommand request, CancellationToken cancellationToken)
		{
			return await _bookRepository.CreateNewBook(request.Title, request.IsActive, request.Description, request.Author, request.CountList, request.CreatedAt, request.UpdateDate);
		}
	}
}
