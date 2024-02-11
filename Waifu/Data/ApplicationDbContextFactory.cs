using Autofac;
using Microsoft.Extensions.Logging;

namespace Waifu.Data;

// for multithreading...
public class ApplicationDbContextFactory
{
    private readonly ILifetimeScope _lifetimeScope;
    private readonly ILogger<ApplicationDbContextFactory> _logger;

    public ApplicationDbContextFactory(ILifetimeScope lifetimeScope, ILogger<ApplicationDbContextFactory> logger)
    {
        _lifetimeScope = lifetimeScope;
        _logger = logger;

        // add 5 in the pool
        for (int i = 0; i < 5; i++)
            ApplicationDbContexts.Add(_lifetimeScope.Resolve<ApplicationDbContext>());
    }

    public List<ApplicationDbContext> ApplicationDbContexts { get; set; } = new();

    public ApplicationDbContext GetDbContext()
    {
        // WHY ISNT DBCONTEXT MULTITHREAD SAFE??!?!?!? WTF MICROSOFT
        
        // var freeDbContext =
        //     ApplicationDbContexts
        //         .Where(x => x.Database.CurrentTransaction is null)
        //         .OrderBy(x => Random.Shared.Next()) // get a random one
        //         .FirstOrDefault(x => !x.IsBusy);
        //
        // if (freeDbContext is not null)
        //     return freeDbContext;

        // no free dbcontext, create one
        var dbContext = _lifetimeScope.Resolve<ApplicationDbContext>();

        // ApplicationDbContexts.Add(dbContext);

        _logger.LogInformation($"Create new DbContext {dbContext.GetHashCode()}");

        return dbContext;
    }
}