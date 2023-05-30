using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Gsri.Api.Personnels.Tests.Utils;

public abstract class WebApiTestBase<TEntryPoint, TContext> : IDisposable
    where TEntryPoint : class
    where TContext : DbContext
{
    private readonly IServiceScope scope;

    private readonly IDbContextTransaction transaction;

    private bool disposedValue;

    protected WebApiTestBase()
    {
        var factory = new WebApplicationFactory<TEntryPoint>();
        scope = factory.Services.CreateScope();
        Context = scope.ServiceProvider.GetRequiredService<TContext>();
        transaction = Context.Database.BeginTransaction();
        Client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(Context);
            });
        }).CreateClient();
    }

    protected HttpClient Client { get; }

    protected TContext Context { get; }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                transaction.Dispose();
                scope.Dispose();
            }
            disposedValue = true;
        }
    }
}
