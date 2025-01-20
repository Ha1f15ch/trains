using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseEngine.RepositoryStorage.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(AppDbContext appDbContext
                              , ILogger<UserRepository> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public async Task<User> CreateNewUser(string name, string email, string password, bool isActive, DateTime dateCreate, DateTime dateUpdate, DateTime dateDelete)
        {
            var parameters = new[]
            {
                new Npgsql.NpgsqlParameter("@value_Name", name),
                new Npgsql.NpgsqlParameter("@value_Email", email),
                new Npgsql.NpgsqlParameter("@value_Password", password),
                new Npgsql.NpgsqlParameter("@value_IsActove", isActive),
                new Npgsql.NpgsqlParameter("@value_DCreate", dateCreate),
                new Npgsql.NpgsqlParameter("@value_DUpdate", dateUpdate),
                new Npgsql.NpgsqlParameter("@value_DDelete", dateDelete),
            };

            try
            {
                _logger.LogInformation("Выполнение ХП для создания записи User, вызван метод {CreateNewUser}", nameof(CreateNewUser));

                await _appDbContext.Database.ExecuteSqlRawAsync("CALL insertUserData(@value_Name, @value_Email, @value_Password, @value_IsActive, @value_DCreate, @value_DUpdate, @value_DDelete)", parameters);

                var newUser = await _appDbContext.Users.Where(u => u.Email == email).FirstOrDefaultAsync();

                if (newUser == null) throw new NullReferenceException("Не найдено значение в таблице User !!!");

                return newUser;
            }
            catch(Exception ex)
            {
                _logger.LogError($"при выполнении метода - {CreateNewUser} возникла ошибка {ex.Message}", nameof(CreateNewUser));
                throw;
            }
            
        }
    }
}
