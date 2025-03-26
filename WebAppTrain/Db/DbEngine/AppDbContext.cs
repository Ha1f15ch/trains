using Microsoft.EntityFrameworkCore;

namespace DbEngine
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
		
	}
}
