using CommonDtoModels;
using DbEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbEngine.RepositoryInterface
{
	public interface IBookRepository
	{
		public Task<Book?> AddBookAsync(Book book);
		public Task<Book?> UpdateBookAsync(UpdatedBook book);
		public Task<bool> DeleteBookAsync(int bookId);
		public Task<Book?> GetBookByName(string bookName);
	}
}
