using System.ComponentModel.DataAnnotations;

namespace Gsri.Api.Personnels.Joueurs.Interfaces;

public record AddJoueurRequest
{
    [Required]
    public string Pseudonyme { get; set; } = string.Empty;
}
