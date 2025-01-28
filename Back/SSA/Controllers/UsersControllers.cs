using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("ApiUsers/Users")]
public class UsersControllers : ControllerBase
{
    private readonly UserContext _context;

    public UsersControllers(UserContext context)
    {
        _context = context;
    }

    // GET: api/users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
    {
        // Get items
        var users = _context.Users;
        return await users.ToListAsync();
    }

    // GET: api/user/2
    [HttpGet("GET")]
    public async Task<ActionResult<Users>> GetItem(int id)
    {
        // Find a specific item
        // SingleAsync() throws an exception if no item is found (which is possible, depending on id)
        // SingleOrDefaultAsync() is a safer choice here
        var user = await _context.Users.SingleOrDefaultAsync(t => t.Id == id);
        if (user == null)
            return NotFound();
        return user;
    }

    // POST: api/user
    [HttpPost]
    public async Task<ActionResult<Users>> PostItem(Users item)
    {
        _context.Users.Add(item);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
    }

    // PUT: api/user
    [HttpPut("PUT")]
    public async Task<IActionResult> PutItem(int id, Users item)
    {
        if (id != item.Id)
            return BadRequest();
        _context.Entry(item).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Users.Any(m => m.Id == id))
                return NotFound();
            else
                throw;
        }
        return NoContent();
    }

    // DELETE: api/item
    [HttpDelete("DELETE")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        var item = await _context.Users.FindAsync(id);
        if (item == null)
            return NotFound();
        _context.Users.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
