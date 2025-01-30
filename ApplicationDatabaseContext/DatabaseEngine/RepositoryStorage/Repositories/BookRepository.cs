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

        public Task<Book?> CreateNewBook(string title, bool isActive, string? description, string? author, int? countList, string? createdAt, DateTime updateDate)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Book>> GetAllBooks()
        {
            try
            {
                _logger.LogInformation($"Поиск всех книг, вызван метод {nameof(GetAllBooks)}");

                return await _context.Books.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"При выполнении метода {nameof(GetAllBooks)} возникла ошибка: {ex.Message}");
                throw;
            }
        }

        public async Task<Book?> GetBookById(int id)
        {
            try
            {
                if(id <= 0)
                {
                    _logger.LogWarning($"Некорректное значение id: {id}. Ожидается положительное число.");
                    return null;
                }

                _logger.LogInformation($"Поиск книги по id = {id}, вызван метод {nameof(GetBookById)}");

                var book = await _context.Books.FindAsync(id);

                if(book is null)
                {
                    _logger.LogWarning($"Книга с id = {id} не найдена.");
                }

                return book;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"При выполнении метода {nameof(GetBookById)} возникла ошибка: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Book>?> GetBookByName(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    _logger.LogWarning($"Некорректное значение name книги: {name}. Ожидается не null.");
                    return null;
                }

                _logger.LogInformation($"Поиск книги по названию = {name}, вызван метод {nameof(GetBookByName)}");

                var book = await _context.Books.Where(el => el.Title.Contains(name)).ToListAsync();

                if (book is null)
                {
                    _logger.LogWarning($"Книга с таким названием = {name} не найдена.");
                }

                return book;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"При выполнении метода {nameof(GetBookByName)} возникла ошибка: {ex.Message}");
                throw;
            }
        }
    }
}
