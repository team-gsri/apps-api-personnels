using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Gsri.Api.Personnels.Tests.Utils.Executors;

public abstract class TestExecutorStringBase<TEntryPoint, TContext, TEntity> : TestExecutorBase<TEntryPoint, TContext, TEntity, string>
    where TEntryPoint : class
    where TEntity : class
    where TContext : DbContext
{
    protected override string GenerateKey() => string.Join(string.Empty, Enumerable.Range(1, 32).Select(i => string.Format("{0:x2}", Random.Shared.Next(256))));

    protected override Expression<Func<string, bool>> GetPredicate(string value) => key => key == value;
}
