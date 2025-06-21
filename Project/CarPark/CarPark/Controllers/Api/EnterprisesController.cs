using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarPark.Data;
using CarPark.Models;

namespace CarPark.Controllers.Api;

public class EnterprisesController : ApiBaseController
{
    private readonly ApplicationDbContext _context;

    public EnterprisesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Enterprises
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Enterprise>>> GetEnterprises()
    {
        return await _context.Enterprises.ToListAsync();
    }

    // GET: api/Enterprises/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<Enterprise>> GetEnterprise(int id)
    {
        var enterprise = await _context.Enterprises.FindAsync(id);

        if (enterprise == null)
        {
            return NotFound();
        }

        return enterprise;
    }
}