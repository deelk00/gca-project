using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utility.EFCore;
using Utility.Other;
using Utility.Other.Extensions;

namespace Utility.Api;

public abstract class CrudController<T> : Controller
    where T : class
{
    protected int pageSize = 50;
    protected readonly DbContext _context;
    
    public CrudController(DbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("{id:guid?}")]
    public virtual async Task<ActionResult<T>> Get([FromRoute] Guid id) 
    {
        return Ok(await _context.IncludeComplexTypes<T>()
            .FirstOrDefaultAsync(ExpressionBuilder.BuildIdExpression<T, Guid>(id)));
    }

    [HttpGet]
    public virtual async Task<ActionResult<T[]>> GetList([FromQuery] int page = 0)
    {
        return Ok(
            (await _context.Set<T>()
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync()
            ).Select(x => x.ToResponseDict())
        );
    }

    [HttpPost]
    public virtual async Task<ActionResult<T>> Post([FromBody] T entity)
    {
        var e = await _context.TransactionAsync(async transaction => await _context.AddAsync(entity));
        return Ok(e.ToResponseDict());
    }

    [HttpPut]
    public virtual async Task<ActionResult<T>> Put([FromBody] T entity)
    {
        var e = await _context.TransactionAsync(transaction => _context.Update(entity));
        return Ok(e.ToResponseDict());
    }

    [HttpDelete]
    public virtual async Task<ActionResult<T>> Delete([FromRoute] Guid id)
    {
        var entity = await _context.FindAsync<T>(id);
        
        if (entity == null) return BadRequest("No entity with this id found");
        entity = await _context.TransactionAsync(transaction => _context.Remove(entity));

        return Ok(entity.ToResponseDict());
    }
}