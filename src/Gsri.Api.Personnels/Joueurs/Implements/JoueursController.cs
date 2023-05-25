using Asp.Versioning;
using Gsri.Api.Personnels.Joueurs.Interfaces;
using Gsri.Api.Personnels.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Gsri.Api.Personnels.Joueurs.Implements;

[ApiController, ApiVersion("1.0"), Route("v{version:apiVersion}/[controller]")]
public class JoueursController : ControllerBase
{
    private const string CreatedRouteName = "Created";

    private readonly JoueursAdapter adapter;

    public JoueursController(JoueursAdapter adapter)
    {
        ArgumentNullException.ThrowIfNull(adapter);

        this.adapter = adapter;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(JoueurDetails))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddAsync([FromBody] AddJoueurRequest payload)
    {
        try
        {
            return CreatedAtRoute(CreatedRouteName, new { id = payload.Pseudonyme },
                await adapter.AddAsync(payload).ConfigureAwait(false));
        }
        catch (BusinessException<JoueursErrors> ex) when (ex.Error == JoueursErrors.AlreadyExists)
        {
            return Conflict("Ce joueur existe déjà");
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        try
        {
            await adapter.DeleteAsync(id).ConfigureAwait(false);
            return NoContent();
        }
        catch (BusinessException<JoueursErrors> ex) when (ex.Error == JoueursErrors.NotFound)
        {
            return NotFound("Ce joueur n'existe pas");
        }
    }

    [HttpGet("{id}", Name = CreatedRouteName)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(JoueurDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FindAsync(string id)
    {
        try
        {
            return Ok(await adapter.FindAsync(id).ConfigureAwait(false));
        }
        catch (BusinessException<JoueursErrors> ex) when (ex.Error == JoueursErrors.NotFound)
        {
            return NotFound("Ce joueur n'existe pas");
        }
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(JoueurDetails[]))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListAsync()
    {
        return Ok(await adapter.GetAllAsync().ConfigureAwait(false));
    }
}
