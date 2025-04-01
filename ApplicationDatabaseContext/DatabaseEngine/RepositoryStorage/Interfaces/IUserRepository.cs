using DatabaseEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseEngine.RepositoryStorage.Interfaces
{
    public interface IUserRepository
    {
        public Task<User> CreateNewUser(string name, string email, string password, bool isActive, DateTime dateCreate, DateTime dateUpdate, DateTime dateDelete);
        public Task<List<User>> GetAllUsers();
        public Task<User?> GetUserById(int userId);
        public Task<List<string>> GetAllUsersEmail();
    }
}
