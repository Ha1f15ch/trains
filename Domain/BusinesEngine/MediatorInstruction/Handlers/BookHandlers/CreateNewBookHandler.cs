using BusinesEngine.MediatorInstruction.Commands.BookCommand;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusinesEngine.MediatorInstruction.Handlers.BookHandlers
{
	public class CreateNewBookHandler : IRequestHandler<CreateNewBookCommand, Book?>
	{
		private readonly IBookRepository _bookRepository;
		private readonly ILogger<CreateNewBookHandler> _logger;

		public CreateNewBookHandler(IBookRepository bookRepository, ILogger<CreateNewBookHandler> logger)
		{
			_bookRepository = bookRepository;
			_logger = logger;
		}

		public async Task<Book?> Handle(CreateNewBookCommand request, CancellationToken cancellationToken)
		{
			try
			{
				_logger.LogInformation("Попытка создания новой книги. Параметры: Title = {Title}, IsActive = {IsActive}, Description = {Description}, Author = {Author}, CountList = {CountList}, CreatedAt = {CreatedAt}, UpdateDate = {UpdateDate}",
					request.Title, request.IsActive, request.Description, request.Author, request.CountList, request.CreatedAt, request.UpdateDate);

				var book = await _bookRepository.CreateNewBook(request.Title, request.IsActive, request.Description, request.Author, request.CountList, request.CreatedAt, request.UpdateDate);

				if (book is null)
				{
					_logger.LogWarning("Создание книги не удалось. Параметры: Title = {Title}", request.Title);
				}
				else
				{
					_logger.LogInformation("Книга успешно создана. ID: {BookId}, Title: {Title}", book.Id, book.Title);
				}

				return book;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при создании книги. Параметры: Title = {Title}, IsActive = {IsActive}, Description = {Description}, Author = {Author}, CountList = {CountList}, CreatedAt = {CreatedAt}, UpdateDate = {UpdateDate}",
					request.Title, request.IsActive, request.Description, request.Author, request.CountList, request.CreatedAt, request.UpdateDate);
				throw;
			}
		}
	}
}
