using BusinesEngine.Commands.BookCommand;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using MediatR;

namespace BusinesEngine.Handlers.BookHandlers
{
	public class GetBookByNameHandler : IRequestHandler<GetBookByNameCommand, List<Book?>>
	{
		private readonly IBookRepository _bookRepository;

		public GetBookByNameHandler(IBookRepository bookRepository)
		{
			_bookRepository = bookRepository;
		}

		public async Task<List<Book?>> Handle(GetBookByNameCommand request, CancellationToken cancellationToken)
		{
			return await _bookRepository.GetBooksByName(request.PartTitleName);
		}
	}
}
