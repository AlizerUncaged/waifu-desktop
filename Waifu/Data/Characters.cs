using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using Waifu.Models;

namespace Waifu.Data;

public class Characters
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ILogger<Characters> _logger;

    public Characters(ApplicationDbContext applicationDbContext, ILogger<Characters> logger)
    {
        _applicationDbContext = applicationDbContext;
        _logger = logger;
    }

    public async Task<int> CountCharactersAsync() => await _applicationDbContext.RoleplayCharacters.CountAsync();

    public async Task AddCharacterAsync(RoleplayCharacter roleplayCharacter)
    {
        _applicationDbContext.RoleplayCharacters.Add(roleplayCharacter);

        await _applicationDbContext.SaveChangesAsync();

        OnCharacterAdded?.Invoke(this, roleplayCharacter);
    }

    public async Task RemoveCharacterAsync(RoleplayCharacter roleplayCharacter)
    {
        _applicationDbContext.RoleplayCharacters.Remove(roleplayCharacter);

        await _applicationDbContext.SaveChangesAsync();

        OnCharacterRemoved?.Invoke(this, roleplayCharacter);
    }

    public async Task<IEnumerable<RoleplayCharacter>> GetAllRoleplayCharactersAsync()
    {
        _logger.LogDebug($"Fetching characters");

        var roleplayCharacters = await _applicationDbContext.RoleplayCharacters.ToListAsync();

        _logger.LogInformation(
            $"Fetched {roleplayCharacters.Count()} characters! Names: {string.Join(", ", roleplayCharacters.Select(x => x.CharacterName))}");

        return roleplayCharacters;
    }

    /// <summary>
    /// Gets called whenever a character gets added.
    /// </summary>
    public event EventHandler<RoleplayCharacter> OnCharacterAdded;

    public event EventHandler<RoleplayCharacter> OnCharacterRemoved;
}