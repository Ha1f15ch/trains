using BusinesEngine.Services.ServiceInterfaces;
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
        private readonly ILogService _logService;


		public BookRepository(AppDbContext context, ILogService logService)
        {
            _context = context;
            _logService = logService;
        }

        public async Task<Book?> CreateNewBook(string title, bool isActive, string? description, string? author, int? countList, string? createdAt, DateTime updateDate)
        {
            try
            {
				_logService.LogInformation($"{nameof(CreateNewBook)} - Создание записи Book. Параметры: Title = {title}, IsActive = {isActive}, Description = {description}, Author = {author}, CountLists = {countList}, CreatedAt = {createdAt}, UpdatedAt = {updateDate}");

                if (string.IsNullOrEmpty(title))
                {
					_logService.LogWarning($"Название книги не может быть пустым. {nameof(title)}");
                    throw new ArgumentNullException($"Параметр title не может быть null");
                }

                if (updateDate == default)
                {
					_logService.LogWarning("Дата обновления не указана.");
                    throw new ArgumentNullException($"Параметр updateDate должен быть указан");
                }

                //проверить, есть ли уже такие записи по title
                var existedBook = await _context.Books.FirstOrDefaultAsync(x => x.Title == title);

                if (existedBook is not null) 
                {
					_logService.LogInformation($"Книга с названием '{title}' уже существует. Возвращена существующая книга.");
                    return existedBook;
                }

                //Создаем экземпляр класса книги для сохранения в контексте
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

				_logService.LogInformation($"Запись книги была создана. newBook = {nameof(newBook)}. Где newBook.Title = {newBook.Title}\nnewBook.IsActive = {newBook.IsActive}\nnewBook.Description = {newBook.Description}\nnewBook.Author = {newBook.Author}\nnewBook.CountLists = {newBook.CountLists}\nnewBook.CreatedAt = {newBook.CreatedAt}\nnewBook.UpdatedAt = {newBook.UpdatedAt}");

                return newBook;
            }
            catch (Exception ex)
            {
				_logService.LogError($"При выполнении метода {nameof(CreateNewBook)} возникла ошибка: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Book>> GetAllBooks()
        {
            try
            {
				_logService.LogInformation($"Поиск всех книг, вызван метод {nameof(GetAllBooks)}");

                var books = await _context.Books.ToListAsync();

				_logService.LogInformation($"Найдено {books.Count} книг");

				return books;
            }
            catch (Exception ex)
            {
				_logService.LogError($"При выполнении метода {nameof(GetAllBooks)} возникла ошибка: {ex.Message}");
                throw;
            }
        }

        public async Task<Book?> GetBookById(int id)
        {
            try
            {
                //Валидация входного параметра
                if(id <= 0)
                {
					_logService.LogWarning($"Некорректное значение id: {id}. Ожидается положительное число.");
                    throw new ArgumentException($"Для поиска книги id должен быть > 0");
                }

				_logService.LogInformation($"Поиск книги по id = {id}, вызван метод {nameof(GetBookById)}");

                //Выполняем поиск
                var book = await _context.Books.FindAsync(id);

                if(book is null)
                {
					_logService.LogWarning($"Книга с id = {id} не найдена.\n id = {book.Id}\n title = {book.Title}");
                }

                return book;
            }
            catch(Exception ex)
            {
				_logService.LogError($"При выполнении метода {nameof(GetBookById)} возникла ошибка: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Book>?> GetBookByName(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
					_logService.LogWarning($"Некорректное значение name книги: {name}. Ожидается не null.");
                    throw new NullReferenceException($"Параметр name не может быть равен null");
                }

				_logService.LogInformation($"Поиск книги по названию = {name}, вызван метод {nameof(GetBookByName)}");

                var book = await _context.Books.Where(el => el.Title.Contains(name)).ToListAsync();

                if (book is null)
                {
					_logService.LogWarning($"Книга с таким названием = {name} не найдена.");
                }

                return book;
            }
            catch (Exception ex)
            {
				_logService.LogError($"При выполнении метода {nameof(GetBookByName)} возникла ошибка: {ex.Message}.");
                throw;
            }
        }
    }
}
