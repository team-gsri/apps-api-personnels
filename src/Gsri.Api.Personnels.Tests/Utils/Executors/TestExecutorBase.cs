using FluentAssertions;
using Gsri.Api.Personnels.Tests.Utils.Handlers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Gsri.Api.Personnels.Tests.Utils.Executors;

public abstract class TestExecutorBase<TEntryPoint, TContext, TEntity, TKey> : IClassFixture<WebApplicationFactory<TEntryPoint>>
    where TEntryPoint : class
    where TEntity : class
    where TContext : DbContext
{
    protected abstract string BaseUrl { get; }

    protected abstract Expression<Func<TEntity, TKey>> KeySelector { get; }

    public async Task Execute(WebApplicationFactory<TEntryPoint> factory, HttpVerbs verb, bool existing, HttpStatusCode expected)
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();
        using var transaction = context.Database.BeginTransaction();
        var key = await GenerateKeyAsync(existing, context).ConfigureAwait(false);
        var content = GenerateContent(verb, key);
        var handler = CreateHandler(verb);
        var client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(context);
            });
        }).CreateClient();
        var response = await handler.Handle(client, BaseUrl, Convert.ToString(key), content);
        response.StatusCode.Should().Be(expected);
    }

    protected abstract TKey GenerateKey();

    protected abstract JsonContent GeneratePostPayload(TKey key);

    protected abstract JsonContent GeneratePutPayload(TKey key);

    protected abstract Expression<Func<TKey, bool>> GetPredicate(TKey value);

    private ITestHandler CreateHandler(HttpVerbs verb) => new Dictionary<HttpVerbs, ITestHandler>
    {
        { HttpVerbs.Get, new GetHandler() },
        { HttpVerbs.Delete, new DeleteHandler() },
        { HttpVerbs.Post, new PostHandler() },
        { HttpVerbs.Put, new PutHandler() },
    }[verb];

    private JsonContent GenerateContent(HttpVerbs verb, TKey key) => new Dictionary<HttpVerbs, Func<TKey, JsonContent>>
    {
        { HttpVerbs.Get, key => JsonContent.Create(string.Empty) },
        { HttpVerbs.Delete, key => JsonContent.Create(string.Empty) },
        { HttpVerbs.Post, GeneratePostPayload },
        { HttpVerbs.Put, GeneratePutPayload },
    }[verb](key);

    private Task<TKey> GenerateKeyAsync(bool existing, DbContext context) => existing
        ? GenrateExistingKeyAsync(context)
        : GenerateNonExistingKeyAsync(context);

    private async Task<TKey> GenerateNonExistingKeyAsync(DbContext context)
    {
        TKey generated;
        do
        {
            generated = GenerateKey();
        }
        while (await context.Set<TEntity>().Select(KeySelector).AnyAsync(GetPredicate(generated)));
        return generated;
    }

    private async Task<TKey> GenrateExistingKeyAsync(DbContext context)
    {
        return await context.Set<TEntity>()
            .Select(KeySelector)
            .FirstAsync()
            .ConfigureAwait(false);
    }

    public enum HttpVerbs

    { Get, Post, Put, Delete };
}
