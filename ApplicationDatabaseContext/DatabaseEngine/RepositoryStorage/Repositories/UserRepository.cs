using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Dapper;
using System.Data;
using Microsoft.Extensions.Logging;

namespace DatabaseEngine.RepositoryStorage.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(AppDbContext appDbContext, ILogger<UserRepository> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        // использовалась параметризированная хранимая процедура, написанная в pg admin 4
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
                _logger.LogInformation($"Выполняем хранимую процедуру - insertuserdata");

                await _appDbContext.Database.ExecuteSqlRawAsync("CALL insertuserdata(@value_Name, @value_Email, @value_Password, @value_IsActive, @value_DCreate, @value_DUpdate, @value_DDelete)", parameters);

                var newUser = await _appDbContext.Users.Where(u => u.Email == email).FirstOrDefaultAsync();

                _logger.LogInformation($"New user: {newUser.Id}");

                if (newUser == null)
                {
                    _logger.LogWarning($"Пользователя не удалось найти.");
                    throw new NullReferenceException("Не найдено значение в таблице User !!!"); 
                }

                return newUser;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"User is not created. Error - {ex.Message}");
                throw;
            }
        }

        // использовался Dapper
        public async Task<List<User>> GetAllUsers()
        {
            try
            {
                const string sqlQuery = "SELECT * FROM dbo.\"User\"";

                _logger.LogInformation($"Creating query to database{sqlQuery}");

                using var connection = _appDbContext.Database.GetDbConnection();

                await connection.OpenAsync();
                
                _logger.LogInformation($"Connection is opened");

                var users = await connection.QueryAsync<User?>(sqlQuery);

                _logger.LogInformation($"Result is {users.ToList().Count} users");

                return users.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Возникла ошибка при получении списка пользователей - {ex.Message}");
                throw;
            }
        }

        // использовался Dapper
        public async Task<User?> GetUserById(int userId)
        {
            try
            {
                const string sqlQuery = "SELECT * FROM dbo.\"User\" WHERE \"Id\" = @Id";

                _logger.LogInformation($"Create a query to database - {userId}. Will be use Dapper to get the result.");
				
				using var connection = _appDbContext.Database.GetDbConnection();

                _logger.LogInformation($"Connect to database is created");
				
				await connection.OpenAsync();

                _logger.LogInformation($"Connection to database is opened");

				var userById = await connection.QuerySingleOrDefaultAsync<User?>(sqlQuery, new { Id = userId });

                _logger.LogInformation($"User is founded by userId = {userId}. UserName = {userById?.Name}");

                if (userById == null)
                {
                    _logger.LogWarning($"User is not founded . . . ");
                    return null;
                }

                return userById;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while processing the request to search for a user by userId = {userId}. Error = {ex.Message}");
                throw;
            }
        }

		public async Task<List<string>> GetAllUsersEmail()
        {
            _logger.LogInformation("Query to get all user emails");

            var usersEmail = await _appDbContext.Users.Select(s => s.Email).ToListAsync();

            _logger.LogInformation($"{usersEmail.Count} user email adresses received");

            return usersEmail;
		}
	}
}
