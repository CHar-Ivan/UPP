using Microsoft.EntityFrameworkCore;

namespace WebApplication1
{
    public class Datab : DbContext
    {
        string connect = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=MuseumDb;Integrated Security=True;Encrypt=False";

        public DbSet<User> Users { get; set; }
        public DbSet<Exhibit> Exhibits { get; set; }
        public DbSet<Exhibitor> Exhibitors { get; set; }
        public DbSet<ExhibitionNote> ExhibitionNotes { get; set; }
        public DbSet<Country> Countries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connect);
        }
    }
}