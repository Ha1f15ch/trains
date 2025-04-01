using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace DatabaseEngine.RepositoryStorage.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<BookRepository> _logger;

		public BookRepository(AppDbContext context, ILogger<BookRepository> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task<Book?> CreateNewBook(string title, bool isActive, string? description, string? author, int? countList, string? createdAt, DateTime updateDate)
        {
            try
            {
				_logger.LogInformation("Попытка создания новой книги с названием {Title}", title);

				if (string.IsNullOrEmpty(title))
				{
					_logger.LogError("Неверное значение параметра title: {Title}. Значение не может быть null или пустым.", title);
					throw new ArgumentNullException($"Параметр title не может быть null");
				}

				if (updateDate == default)
				{
					_logger.LogError("Неверное значение параметра updateDate: {UpdateDate}. Дата обновления должна быть указана.", updateDate);
					throw new ArgumentNullException($"Параметр updateDate должен быть указан");
				}

				var existedBook = await _context.Books.FirstOrDefaultAsync(x => x.Title == title);

                if (existedBook is not null)
				{
					_logger.LogWarning("Книга с названием {Title} уже существует. Возвращаем существующую книгу.", title);
					return existedBook;
				}

				var newBook = new Book
                {
                    Title = title,
                    IsActive = isActive,
                    Description = description,
                    Author = author,
                    CountLists = countList,
                    CreatedAt = createdAt,
                    UpdatedAt = updateDate
                };

                await _context.Books.AddAsync(newBook);
                await _context.SaveChangesAsync();

				_logger.LogInformation("Книга с названием {Title} успешно создана", title);
				return newBook;
			}
            catch (Exception ex)
            {
				_logger.LogError(ex, "Ошибка при создании книги с названием {Title}", title);
				throw;
			}
        }

        public async Task<List<Book>> GetAllBooks()
        {
			try
			{
				_logger.LogInformation("Попытка получения всех книг");

				var books = await _context.Books.ToListAsync();

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

        public async Task<Book?> GetBookById(int id)
        {
			try
			{
				_logger.LogInformation("Попытка получения книги по ID {Id}", id);

				if (id <= 0)
				{
					_logger.LogError("Неверное значение параметра id: {Id}. Значение должно быть больше 0.", id);
					throw new ArgumentException($"Для поиска книги id должен быть > 0");
				}

				var book = await _context.Books.FindAsync(id);

				if (book is null)
				{
					_logger.LogWarning("Книга с ID {Id} не найдена", id);
					return null;
				}

				_logger.LogInformation("Книга с ID {Id} успешно найдена. Название: {Title}", id, book.Title);
				return book;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при получении книги по ID {Id}", id);
				throw;
			}
		}

        public async Task<List<Book>> GetBooksByName(string name)
        {
			try
			{
				_logger.LogInformation("Попытка поиска книг по названию {Name}", name);

				if (string.IsNullOrEmpty(name))
				{
					_logger.LogError("Неверное значение параметра name: {Name}. Значение не может быть null или пустым.", name);
					throw new NullReferenceException($"Параметр name не может быть равен null");
				}

				var books = await _context.Books.Where(el => el.Title.Contains(name)).ToListAsync();

				if (!books.Any())
				{
					_logger.LogWarning("Книги с названием {Name} не найдены", name);
				}
				else
				{
					_logger.LogInformation("Найдено {Count} книг с названием {Name}", books.Count, name);
				}

				return books;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при поиске книг по названию {Name}", name);
				throw;
			}
		}
    }
}
