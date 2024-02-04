using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Waifu.Data;

public class Models
{
    private readonly ILogger<Models> _logger;

    public Models(ILogger<Models> logger)
    {
        _logger = logger;
    }
    
}