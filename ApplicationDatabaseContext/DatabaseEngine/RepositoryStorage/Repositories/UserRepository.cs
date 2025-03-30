using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Dapper;
using System.Data;

namespace DatabaseEngine.RepositoryStorage.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _appDbContext;

        public UserRepository(
            AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
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
                await _appDbContext.Database.ExecuteSqlRawAsync("CALL insertuserdata(@value_Name, @value_Email, @value_Password, @value_IsActive, @value_DCreate, @value_DUpdate, @value_DDelete)", parameters);

                var newUser = await _appDbContext.Users.Where(u => u.Email == email).FirstOrDefaultAsync();

                if (newUser == null)
                {
                    throw new NullReferenceException("Не найдено значение в таблице User !!!"); 
                }

                return newUser;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        // использовался Dapper
        public async Task<List<User?>> GetAllUsers()
        {
            try
            {
                const string sqlQuery = "SELECT * FROM dbo.\"User\"";

                //Создаем соединение с БД
                using var connection = _appDbContext.Database.GetDbConnection();

                //Открываем соединение
                await connection.OpenAsync();

                var users = await connection.QueryAsync<User?>(sqlQuery);
                
                return users.ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // использовался Dapper
        public async Task<User?> GetUserById(int userId)
        {
            try
            {
                const string sqlQuery = "SELECT * FROM dbo.\"User\" WHERE \"Id\" = @Id";

				//Создаем соединение с БД
				using var connection = _appDbContext.Database.GetDbConnection();

				//Открываем соединение
				await connection.OpenAsync();

				var userById = await connection.QuerySingleOrDefaultAsync<User?>(sqlQuery, new { Id = userId });

                if (userById == null)
                {
                    return null;
                }

                return userById;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

		public async Task<List<string>> GetAllUsersEmail()
        {
            var usersEmail = await _appDbContext.Users.Select(s => s.Email).ToListAsync();

            return usersEmail;
		}
	}
}
