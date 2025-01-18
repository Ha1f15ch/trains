using Microsoft.EntityFrameworkCore;

namespace DatabaseEngine
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        
    }
}
