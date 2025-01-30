using DatabaseEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseEngine.RepositoryStorage.Interfaces
{
    public interface IBookRepository
    {
        public Task<List<Book>> GetAllBooks();
        public Task<Book?> GetBookById(int id);
        public Task<List<Book>?> GetBookByName(string name);
        public Task<Book?> CreateNewBook(string title, bool isActive, string? description, string? author, int? countList, string? createdAt, DateTime updateDate);
    }
}
