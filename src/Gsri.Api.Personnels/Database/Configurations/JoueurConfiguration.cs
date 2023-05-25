using Gsri.Api.Personnels.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gsri.Api.Personnels.Database.Configurations;

public class JoueurConfiguration : IEntityTypeConfiguration<JoueurEntity>
{
    public void Configure(EntityTypeBuilder<JoueurEntity> builder)
    {
        builder.HasIndex(joueur => joueur.Pseudonyme).IsUnique();
    }
}
