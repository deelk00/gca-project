using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utility.EFCore;
using Utility.Other;
using Utility.Other.Enums;
using Utility.Other.Extensions;

namespace Utility.Api.Controllers;

public abstract class CrudController<T> : Controller
    where T : class
{
    protected int pageSize = 50;
    protected readonly DbContext Context;
    
    public CrudController(DbContext context)
    {
        Context = context;
    }

    [HttpGet]
    [Route("{id:guid?}")]
    public virtual async Task<ActionResult<T>> Get([FromRoute] Guid id, [FromQuery] bool includeComplexTypes = false) 
    {
        var options = ToResponseDictOptions.EntitiesToIds;
        if (includeComplexTypes)
        {
            options = ToResponseDictOptions.None;
        }

        var entity = (await Context.Set<T>()
            .IncludeComplexTypes()
            .FirstOrDefaultAsync(ExpressionBuilder.BuildIdExpression<T, Guid>(id)));
        return Ok(entity?
            .ToResponseDict(options) ?? new object()
        );
    }

    [HttpGet]
    public virtual async Task<ActionResult<T[]>> GetList([FromQuery] int page = 0, [FromQuery] bool includeComplexTypes = false)
    {
        var query = Context.Set<T>().IncludeComplexTypes();
        var options = ToResponseDictOptions.EntitiesToIds;
        if (includeComplexTypes)
        {
            options = ToResponseDictOptions.None;
        }

        var entities = await query
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(entities.Select(x => x.ToResponseDict(options)));
    }

    [HttpPost]
    public virtual async Task<ActionResult<T>> Post([FromBody] T entity)
    {
        var e = await Context.TransactionAsync(async transaction => await Context.AddAsync(entity));
        return Ok(e.ToResponseDict());
    }

    [HttpPut]
    public virtual async Task<ActionResult<T>> Put([FromBody] T entity)
    {
        var e = await Context.TransactionAsync(transaction => Context.Update(entity));
        return Ok(e.ToResponseDict());
    }

    [HttpDelete]
    public virtual async Task<ActionResult<T>> Delete([FromRoute] Guid id)
    {
        var entity = await Context.FindAsync<T>(id);
        
        if (entity == null) return BadRequest("No entity with this id found");
        entity = await Context.TransactionAsync(transaction => Context.Remove(entity));

        return Ok(entity.ToResponseDict());
    }
}