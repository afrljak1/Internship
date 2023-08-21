using Microsoft.EntityFrameworkCore;

namespace Movie_Api.Models
{
    public class APIDbContext : DbContext
    {
        public APIDbContext(DbContextOptions<APIDbContext> options) : base(options)
        { }


        public DbSet<Movie> Movies { get; set; }

    }
    }
