using Microsoft.EntityFrameworkCore;

namespace Waifu.Data;

public class Characters
{
    private readonly ApplicationDbContext _applicationDbContext;

    public Characters(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<int> CountCharactersAsync() => await _applicationDbContext.RoleplayCharacters.CountAsync();
}