using Gsri.Api.Personnels.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Gsri.Api.Personnels.Database;

public class PersonnelsDbContext : DbContext
{
    public PersonnelsDbContext(DbContextOptions<PersonnelsDbContext> options) : base(options)
    { }

    public DbSet<JoueurEntity> Joueurs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
