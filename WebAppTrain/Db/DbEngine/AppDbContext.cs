using DbEngine.Models;
using Microsoft.EntityFrameworkCore;

namespace DbEngine
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
		
		public DbSet<User> Users { get; set; }
		public DbSet<Book> Books { get; set; }
		public DbSet<Genre> Genres { get; set; }
		public DbSet<BookGenre> BookGenres { get; set; }
	}
}
