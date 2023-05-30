using FluentAssertions;
using Gsri.Api.Personnels.Database;
using Gsri.Api.Personnels.Database.Models;
using Gsri.Api.Personnels.Joueurs.Interfaces;
using Gsri.Api.Personnels.Tests.Utils.Executors;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Gsri.Api.Personnels.Tests.IntegrationTests;

public class JoueursTests : TestExecutorStringBase<Program, PersonnelsDbContext, JoueurEntity>
{
    private readonly WebApplicationFactory<Program> factory;

    public JoueursTests(WebApplicationFactory<Program> factory)
    {
        this.factory = factory;
    }

    protected override string BaseUrl => "v1/Joueurs";

    protected override Expression<Func<JoueurEntity, string>> KeySelector => joueur => joueur.Pseudonyme;

    [Theory]
    [InlineData(true, HttpVerbs.Get, HttpStatusCode.OK)]
    [InlineData(true, HttpVerbs.Delete, HttpStatusCode.NoContent)]
    [InlineData(true, HttpVerbs.Post, HttpStatusCode.Conflict)]
    [InlineData(true, HttpVerbs.Put, HttpStatusCode.MethodNotAllowed)]
    [InlineData(false, HttpVerbs.Get, HttpStatusCode.NotFound)]
    [InlineData(false, HttpVerbs.Delete, HttpStatusCode.NotFound)]
    [InlineData(false, HttpVerbs.Post, HttpStatusCode.Created)]
    [InlineData(false, HttpVerbs.Put, HttpStatusCode.MethodNotAllowed)]
    public async Task JoueursController(bool entity_exists, HttpVerbs method, HttpStatusCode expected)
    {
        await Execute(factory, method, entity_exists, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task GetAll()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("v1/Joueurs").ConfigureAwait(false);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    protected override JsonContent GeneratePostPayload(string key) => JsonContent.Create(new AddJoueurRequest { Pseudonyme = key });

    protected override JsonContent GeneratePutPayload(string key) => JsonContent.Create(string.Empty);
}
