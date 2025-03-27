using CommonDtoModels;
using DbEngine.Models;
using DbEngine.RepositoryInterface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbEngine.RepositoryStorage
{
	public class BookRepository : IBookRepository
	{
		private readonly AppDbContext _appDbContext;

		public BookRepository(AppDbContext context)
		{
			_appDbContext = context;
		}

		public async Task<Book?> AddBookAsync(Book book)
		{
			try
			{
				//ищем такую книгу
				var existedBook = await GetBookByName(book.Name);

				//Если такая есть
				if(existedBook is not null)
				{
					Console.WriteLine($"WARN такая книгу уже есть, возвращаем найденную - {existedBook.Id} - {existedBook.Name}");
				}
				else
				{
					Console.WriteLine($"INFO такой книги нет, создаем новую");
				}

				await _appDbContext.Books.AddAsync(book);
				await _appDbContext.SaveChangesAsync();

				Console.WriteLine($"INFO Книга успешно создана, присвоен id = {book.Id}");

				return book;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"ошибка при создании новой книги - {ex.Message}");
				return null;
			}
		}

		public async Task<bool> DeleteBookAsync(int bookId)
		{
			try
			{
				//находим книгу перед удалением 
				var bookForDelete = await _appDbContext.Books.FindAsync(bookId);

				if(bookForDelete is not null)
				{
					Console.WriteLine($"Книга найдена удаляем . . .");

					_appDbContext.Remove(bookForDelete);
					await _appDbContext.SaveChangesAsync();

					Console.WriteLine($"Книга успешно удалена");
					
					return true;
				}
				else
				{
					Console.WriteLine($"WARN Не удалось найти книгу по заданному id = {bookId}");
					return false;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"WARN Не удалось удалить книгу id = {bookId}. ошибка - {ex.Message}");
				return false;
			}
		}

		public async Task<Book?> UpdateBookAsync(UpdatedBook book)
		{
			try
			{

			}
			catch(Exception ex)
			{
				Console.WriteLine($"WARN Возникла ошибка при обновлении данных книги id = {book.Id}. Ошибка - {ex.Message}");
				return null;
			}
		}

		public async Task<Book?> GetBookByName(string bookName)
		{
			try
			{
				var findBook = await _appDbContext.Books.SingleOrDefaultAsync(el => el.Name == bookName);
				if(findBook is not null)
				{
					Console.WriteLine($"INFO Книга найдена - {findBook.Id} - {findBook.Name}");
					return findBook;
				}
				else
				{
					Console.WriteLine($"INFO Книга не найдена");
					return findBook;
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine($"Ошибка при поиске книги по названию - {bookName}. Ошибка - {ex.Message}");
				return null;
			}
		}
	}
}
