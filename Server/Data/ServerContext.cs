using Microsoft.EntityFrameworkCore;
using SharedLibrary.Models;

namespace Server.Data
{
    public class ServerContext : DbContext
    {
        public ServerContext(DbContextOptions<ServerContext> opp) : base(opp) { }

        public virtual DbSet<Employee>Employees { get; set; }
        public virtual DbSet<Experience> Experiences { get; set; }
        public virtual DbSet<ExperienceTitle> ExperiencesTitles { get; set; }

    }
}
