using FluentAssertions;
using Gsri.Api.Personnels.Tests.Utils.Handlers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Json;

namespace Gsri.Api.Personnels.Tests.Utils;

public abstract class ControllerTestBase<TEntryPoint, TContext, TEntity> : WebApiTestBase<TEntryPoint, TContext>
    where TEntryPoint : class
    where TEntity : class
    where TContext : DbContext
{
    protected abstract string BaseUrl { get; }

    protected abstract Expression<Func<TEntity, string>> KeySelector { get; }

    public async Task Execute(HttpVerbs verb, bool existing, HttpStatusCode expected)
    {
        await SeedDatabaseAsync(Context).ConfigureAwait(false);
        var key = await GenerateKeyAsync(existing, Context).ConfigureAwait(false);
        var content = GenerateJsonContent(verb, key);
        var handler = CreateHandler(verb);
        var response = await handler.Handle(Client, BaseUrl, Convert.ToString(key), content);
        response.StatusCode.Should().Be(expected);
    }

    protected abstract JsonContent GeneratePostPayload(string key);

    protected abstract JsonContent GeneratePutPayload(string key);

    protected abstract Task SeedDatabaseAsync(TContext context);

    private ITestHandler CreateHandler(HttpVerbs verb) => new Dictionary<HttpVerbs, ITestHandler>
    {
        { HttpVerbs.Get, new GetHandler() },
        { HttpVerbs.Delete, new DeleteHandler() },
        { HttpVerbs.Post, new PostHandler() },
        { HttpVerbs.Put, new PutHandler() },
    }[verb];

    private JsonContent GenerateJsonContent(HttpVerbs verb, string key) => new Dictionary<HttpVerbs, Func<string, JsonContent>>
    {
        { HttpVerbs.Get, key => JsonContent.Create(string.Empty) },
        { HttpVerbs.Delete, key => JsonContent.Create(string.Empty) },
        { HttpVerbs.Post, GeneratePostPayload },
        { HttpVerbs.Put, GeneratePutPayload },
    }[verb](key);

    private Task<string> GenerateKeyAsync(bool existing, DbContext context) => existing
        ? GenrateExistingKeyAsync(context)
        : GenerateNonExistingKeyAsync(context);

    private async Task<string> GenerateNonExistingKeyAsync(DbContext context)
    {
        string generated;
        do
        {
            generated = Random.Shared.Next().ToString();
        }
        while (await context.Set<TEntity>().Select(KeySelector).AnyAsync(key => key == generated));
        return generated;
    }

    private async Task<string> GenrateExistingKeyAsync(DbContext context)
    {
        return await context.Set<TEntity>()
            .Select(KeySelector)
            .FirstAsync()
            .ConfigureAwait(false);
    }

    public enum HttpVerbs

    { Get, Post, Put, Delete };
}
