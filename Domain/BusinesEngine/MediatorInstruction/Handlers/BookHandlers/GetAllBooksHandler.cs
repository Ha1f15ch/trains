using BusinesEngine.MediatorInstruction.Commands.BookCommand.Queries;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusinesEngine.MediatorInstruction.Handlers.BookHandlers
{
	public class GetAllBooksHandler : IRequestHandler<GetAllBooksQuery, List<Book>>
	{
		private readonly IBookRepository _bookRepository;
		private readonly ILogger<GetAllBooksHandler> _logger;

		public GetAllBooksHandler(IBookRepository bookRepository, ILogger<GetAllBooksHandler> logger)
		{
			_bookRepository = bookRepository;
			_logger = logger;
		}

		public async Task<List<Book>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
		{
			try
			{
				_logger.LogInformation("Попытка получения всех книг");

				var books = await _bookRepository.GetAllBooks();

				if (!books.Any())
				{
					_logger.LogWarning("Книги в базе данных отсутствуют");
				}
				else
				{
					_logger.LogInformation("Успешно получено {Count} книг", books.Count);
				}

				return books;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при получении всех книг");
				throw;
			}
		}
	}
}
