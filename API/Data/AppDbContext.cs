using Microsoft.EntityFrameworkCore;
using BookService.Models;

namespace BookService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<MediaItem> MediaItems => Set<MediaItem>();
        public DbSet<MediaUnit> MediaUnits => Set<MediaUnit>();
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<Loan> Loans => Set<Loan>();
        public DbSet<User> Users => Set<User>();
    }
}
