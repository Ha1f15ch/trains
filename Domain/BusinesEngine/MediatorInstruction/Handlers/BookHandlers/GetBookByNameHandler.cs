using BusinesEngine.MediatorInstruction.Commands.BookCommand;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusinesEngine.MediatorInstruction.Handlers.BookHandlers
{
	public class GetBookByNameHandler : IRequestHandler<GetBookByNameCommand, List<Book>>
	{
		private readonly IBookRepository _bookRepository;
		private readonly ILogger<GetBookByNameHandler> _logger;

		public GetBookByNameHandler(IBookRepository bookRepository, ILogger<GetBookByNameHandler> logger)
		{
			_bookRepository = bookRepository;
			_logger = logger;
		}

		public async Task<List<Book>> Handle(GetBookByNameCommand request, CancellationToken cancellationToken)
		{
			try
			{
				_logger.LogInformation("Попытка поиска книг по части названия: {PartTitleName}", request.PartTitleName);

				var books = await _bookRepository.GetBooksByName(request.PartTitleName);

				if (!books.Any())
				{
					_logger.LogWarning("Книги с частью названия '{PartTitleName}' не найдены", request.PartTitleName);
				}
				else
				{
					_logger.LogInformation("Найдено {Count} книг с частью названия '{PartTitleName}'", books.Count, request.PartTitleName);
				}

				return books;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при поиске книг по части названия: {PartTitleName}", request.PartTitleName);
				throw;
			}
		}
	}
}
