using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Gsri.Api.Personnels.Tests.Utils.Executors;

internal abstract class TestExecutorIntBase<TEntryPoint, TContext, TEntity> : TestExecutorBase<TEntryPoint, TContext, TEntity, int>
    where TEntryPoint : class
    where TEntity : class
    where TContext : DbContext
{
    protected override int GenerateKey() => Random.Shared.Next();

    protected override Expression<Func<int, bool>> GetPredicate(int value) => key => key == value;
}
