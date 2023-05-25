using AutoMapper;
using AutoMapper.QueryableExtensions;
using Gsri.Api.Personnels.Database;
using Gsri.Api.Personnels.Database.Models;
using Gsri.Api.Personnels.Joueurs.Interfaces;
using Gsri.Api.Personnels.Utils;
using Microsoft.EntityFrameworkCore;

namespace Gsri.Api.Personnels.Joueurs.Implements;

public class JoueursAdapter
{
    private readonly PersonnelsDbContext database;

    private readonly IMapper mapper;

    public JoueursAdapter(PersonnelsDbContext database, IMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(database);
        ArgumentNullException.ThrowIfNull(mapper);

        this.database = database;
        this.mapper = mapper;
    }

    public async Task<JoueurDetails> AddAsync(AddJoueurRequest payload)
    {
        var query = database.Joueurs.Where(joueur => joueur.Pseudonyme == payload.Pseudonyme);
        if (await query.AnyAsync().ConfigureAwait(false))
        {
            throw new BusinessException<JoueursErrors>(JoueursErrors.AlreadyExists);
        }
        database.Joueurs.Add(mapper.Map<JoueurEntity>(payload));
        await database.SaveChangesAsync().ConfigureAwait(false);
        return await FindAsync(payload.Pseudonyme).ConfigureAwait(false);
    }

    public async Task DeleteAsync(string id)
    {
        var query = database.Joueurs.Where(joueur => joueur.Pseudonyme == id);
        if (!await query.AnyAsync().ConfigureAwait(false))
        {
            throw new BusinessException<JoueursErrors>(JoueursErrors.NotFound);
        }
        await query.ExecuteDeleteAsync().ConfigureAwait(false);
    }

    public async Task<JoueurDetails> FindAsync(string id)
    {
        return await database.Joueurs
            .Where(joueur => joueur.Pseudonyme == id)
            .ProjectTo<JoueurDetails>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false)
            ?? throw new BusinessException<JoueursErrors>(JoueursErrors.NotFound);
    }

    public async Task<JoueurDetails[]> GetAllAsync()
    {
        return await database.Joueurs
            .ProjectTo<JoueurDetails>(mapper.ConfigurationProvider)
            .ToArrayAsync()
            .ConfigureAwait(false);
    }
}
