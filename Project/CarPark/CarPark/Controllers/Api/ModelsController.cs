using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarPark.Data;
using CarPark.Models;
using Microsoft.AspNetCore.Http;

namespace CarPark.Controllers.Api;

public class ModelsController : ApiBaseController
{
    private readonly ApplicationDbContext _context;

    public ModelsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Models
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Model>>> GetModels()
    {
        return await _context.Models.ToListAsync();
    }

    // GET: api/Models/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<Model>> GetModel(int id)
    {
        var model = await _context.Models.FindAsync(id);

        if (model == null)
        {
            return NotFound();
        }

        return model;
    }

    // PUT: api/Models/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> PutModel(int id, Model model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        _context.Entry(model).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ModelExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Models
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<Model>> PostModel(Model model)
    {
        _context.Models.Add(model);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetModel", new { id = model.Id }, model);
    }

    // DELETE: api/Models/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> DeleteModel(int id)
    {
        var model = await _context.Models.FindAsync(id);
        if (model == null)
        {
            return NotFound();
        }

        _context.Models.Remove(model);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ModelExists(int id)
    {
        return _context.Models.Any(e => e.Id == id);
    }
}