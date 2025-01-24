using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
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
                new NpgsqlParameter("@value_Name", NpgsqlTypes.NpgsqlDbType.Text) { Value = name },
                new NpgsqlParameter("@value_Email", NpgsqlTypes.NpgsqlDbType.Text) { Value = email },
                new NpgsqlParameter("@value_Password", NpgsqlTypes.NpgsqlDbType.Text) { Value = password },
                new NpgsqlParameter("@value_IsActive", NpgsqlTypes.NpgsqlDbType.Boolean) { Value = isActive },
                new NpgsqlParameter("@value_DCreate", NpgsqlTypes.NpgsqlDbType.Date) { Value = dateCreate },
                new NpgsqlParameter("@value_DUpdate", NpgsqlTypes.NpgsqlDbType.Date) { Value = dateUpdate },
                new NpgsqlParameter("@value_DDelete", NpgsqlTypes.NpgsqlDbType.Date) { Value = dateDelete }
            };

            try
            {
                _logger.LogInformation("Выполнение ХП для создания записи User, вызван метод {CreateNewUser}", nameof(CreateNewUser));

                await _appDbContext.Database.ExecuteSqlRawAsync("CALL insertuserdata(@value_Name, @value_Email, @value_Password, @value_IsActive, @value_DCreate, @value_DUpdate, @value_DDelete)", parameters);

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

        public async Task<List<User?>> GetAllUsers()
        {
            try
            {
                _logger.LogInformation($"Выполнение ХП для получения всех пользователей, вызван метод {nameof(GetAllUsers)}");

                const string sqlQuery = "SELECT * FROM dbo.\"User\"";

                using var connection = _appDbContext.Database.GetDbConnection();

                await connection.OpenAsync();

                var users = await connection.QueryAsync<User?>(sqlQuery);
                
                return users.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"При выполнении метода - {nameof(GetAllUsers)} возникла ошибка: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<User?> GetUserById(int userId)
        {
            try
            {
                _logger.LogInformation($"Выполнение ХП для получения всех пользователей, вызван метод {nameof(GetUserById)}");

                const string sqlQuery = "SELECT * FROM dbo.\"User\" WHERE \"Id\" = @Id";

                using var connection = _appDbContext.Database.GetDbConnection();

                await connection.OpenAsync();

                var userById = await connection.QuerySingleOrDefaultAsync<User?>(sqlQuery, new { Id = userId });

                return userById;
            }
            catch (Exception ex)
            {
                _logger.LogError($"При выполнении метода - {nameof(GetUserById)} возникла ошибка: {ex.Message}", ex);
                throw;
            }
        }
    }
}
