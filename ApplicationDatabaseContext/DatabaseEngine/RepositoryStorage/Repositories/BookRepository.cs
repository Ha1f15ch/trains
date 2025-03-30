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


		public BookRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Book?> CreateNewBook(string title, bool isActive, string? description, string? author, int? countList, string? createdAt, DateTime updateDate)
        {//Возможно после использовать autoMapper
            try
            {
                if (string.IsNullOrEmpty(title))
                {
                    throw new ArgumentNullException($"Параметр title не может быть null");
                }

                if (updateDate == default)
                {
                    throw new ArgumentNullException($"Параметр updateDate должен быть указан");
                }

                //проверить, есть ли уже такие записи по title
                var existedBook = await _context.Books.FirstOrDefaultAsync(x => x.Title == title);

                if (existedBook is not null) 
                {
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

                return newBook;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<Book?>> GetAllBooks()
        {
            try
            {
                var books = await _context.Books.ToListAsync();

				return books;
            }
            catch (Exception ex)
            {
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
                    throw new ArgumentException($"Для поиска книги id должен быть > 0");
                }

                //Выполняем поиск
                var book = await _context.Books.FindAsync(id);

                if(book is null)
                {
					Console.WriteLine($"Книга с id = {id} не найдена.\n id = {book.Id}\n title = {book.Title}");
                }

                return book;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<List<Book?>> GetBooksByName(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new NullReferenceException($"Параметр name не может быть равен null");
                }

                var book = await _context.Books.Where(el => el.Title.Contains(name)).ToListAsync();

                if (book is null)
                {
					Console.WriteLine($"Книга с таким названием = {name} не найдена.");
                }

                return book;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
