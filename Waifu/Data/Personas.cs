using Waifu.Models;

namespace Waifu.Data;

public class Personas
{
    private readonly ApplicationDbContextFactory _applicationDbContext;

    public Personas(ApplicationDbContextFactory applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<PersonaSingle> GetOrCreatePersona()
    {
        var dbContext = _applicationDbContext.GetDbContext();
        
        var persona = new PersonaSingle()
        {
        };

        dbContext.Personas.Add(persona);

        await dbContext.SaveChangesAsync();

        return persona;
    }
}