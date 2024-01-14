using Microsoft.EntityFrameworkCore;
using Waifu.Models;

namespace Waifu.Data;

public class Characters
{
    private readonly ApplicationDbContext _applicationDbContext;

    public Characters(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<int> CountCharactersAsync() => await _applicationDbContext.RoleplayCharacters.CountAsync();

    public async Task AddCharacterAsync(RoleplayCharacter roleplayCharacter)
    {
        _applicationDbContext.RoleplayCharacters.Add(roleplayCharacter);

        await _applicationDbContext.SaveChangesAsync();
    }
}