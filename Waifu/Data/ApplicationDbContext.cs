using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Serilog;
using Waifu.Models;

namespace Waifu.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<ChatChannel> ChatChannels { get; set; }

    public DbSet<ChatMessage> ChatMessages { get; set; }

    public DbSet<RoleplayCharacter> RoleplayCharacters { get; set; }

    public DbSet<Waifu.Models.Settings> Settings { get; set; }

    public DbSet<Hotkey> Hotkeys { get; set; }

    public DbSet<PersonaSingle> Personas { get; set; }

    public bool IsBusy { get; private set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Configure SQLite as the database provider
        optionsBuilder.UseSqlite($"Data Source={Constants.DatabasePath}");
        optionsBuilder.AddInterceptors(new DbContextInterceptors(this));
    }

    private static readonly SemaphoreSlim SaveChangesSemaphore = new SemaphoreSlim(1, 1);

    private class DbContextInterceptors : DbCommandInterceptor
    {
        private readonly ApplicationDbContext _context;

        public DbContextInterceptors(ApplicationDbContext context)
        {
            _context = context;
        }

        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData,
            InterceptionResult<DbDataReader> result)
        {
            _context.IsBusy = true;
            return base.ReaderExecuting(command, eventData, result);
        }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command,
            CommandEventData eventData, InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            _context.IsBusy = true;
            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command,
            CommandExecutedEventData eventData, DbDataReader result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            _context.IsBusy = false;
            return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
        }

        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData,
            DbDataReader result)
        {
            _context.IsBusy = false;
            return base.ReaderExecuted(command, eventData, result);
        }

        public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData,
            InterceptionResult<object> result)
        {
            _context.IsBusy = true;
            return base.ScalarExecuting(command, eventData, result);
        }

        public override object? ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object? result)
        {
            _context.IsBusy = false;
            return base.ScalarExecuted(command, eventData, result);
        }

        public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command,
            CommandEventData eventData, InterceptionResult<object> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            _context.IsBusy = true;
            return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override ValueTask<object?> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData,
            object? result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            _context.IsBusy = false;
            return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
        }

        public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command,
            CommandEventData eventData, InterceptionResult<int> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            _context.IsBusy = true;
            return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData,
            int result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            _context.IsBusy = false;
            return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
        }
    }

    public override int SaveChanges()
    {
        SaveChangesSemaphore.Wait();

        IsBusy = true;

        var result = base.SaveChanges();

        IsBusy = false;

        SaveChangesSemaphore.Release();

        return result;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await SaveChangesSemaphore.WaitAsync(cancellationToken);

        IsBusy = true;

        var result = await base.SaveChangesAsync(cancellationToken);

        IsBusy = false;

        SaveChangesSemaphore.Release();

        return result;
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}