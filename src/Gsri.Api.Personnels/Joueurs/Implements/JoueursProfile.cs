using AutoMapper;
using Gsri.Api.Personnels.Database.Models;
using Gsri.Api.Personnels.Joueurs.Interfaces;

namespace Gsri.Api.Personnels.Joueurs.Implements;

public class JoueursProfile : Profile
{
    public JoueursProfile()
    {
        CreateMap<JoueurEntity, JoueurDetails>();
        CreateMap<AddJoueurRequest, JoueurEntity>();
    }
}
