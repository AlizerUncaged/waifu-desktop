using Waifu.Models;

namespace Waifu.Data;

public class Personas
{
    private readonly ApplicationDbContext _applicationDbContext;

    public Personas(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<PersonaSingle> GetOrCreatePersona()
    {
        var persona = new PersonaSingle()
        {
        };

        _applicationDbContext.Personas.Add(persona);

        await _applicationDbContext.SaveChangesAsync();
        
        return persona;
    }
}