using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utility.EFCore;
using Utility.Other;
using Utility.Other.Extensions;

namespace Utility.Api.Controllers;

public abstract class ManyToManyRelationShipController<TFirst, TSecond> : Controller
    where TFirst: class
    where TSecond: class
{
    private readonly string _firstPropName;
    private readonly string _secondPropName;
    
    private readonly DbContext _context;
    
    public ManyToManyRelationShipController(
        DbContext context, 
        string firstPropName,
        string secondPropName
        )
    {
        _context = context;
        _firstPropName = firstPropName;
        _secondPropName = secondPropName;
    }
    
    [HttpPost]
    [Route("{firstId:guid}/{secondId:guid}")]
    public async Task<ActionResult> Post([FromRoute] Guid firstId, [FromRoute] Guid secondId)
    {
        var first = await _context.Set<TFirst>()
            .Include(_firstPropName)
            .FirstOrDefaultAsync(ExpressionBuilder.BuildIdExpression<TFirst, Guid>(firstId));
        
        var second = await _context.Set<TSecond>()
            .Include(_secondPropName)
            .FirstOrDefaultAsync(ExpressionBuilder.BuildIdExpression<TSecond, Guid>(secondId));
        
        if (first == null || second == null)
        {
            if (first == null ^ second == null)
            {
                return BadRequest("id not found");
            }
            
            first = await _context.Set<TFirst>()
                .Include(_secondPropName)
                .FirstOrDefaultAsync(ExpressionBuilder.BuildIdExpression<TFirst, Guid>(secondId));
            second = await _context.Set<TSecond>()
                .Include(_firstPropName)
                .FirstOrDefaultAsync(ExpressionBuilder.BuildIdExpression<TSecond, Guid>(firstId));

            if (first == null || second == null)
            {
                return BadRequest("id not found");
            }
        }

        var firstSecondProp = first.AccessProperty<TFirst, List<TSecond>>(_firstPropName);
        firstSecondProp.Add(second);

        var secondFirstProp = second.AccessProperty<TSecond, List<TFirst>>(_secondPropName);
        secondFirstProp.Add(first);

        first = await _context.TransactionAsync(x => _context.Update(first));
        second = await _context.TransactionAsync(x => _context.Update(second));

        return Ok(new 
        {
            first = first.ToResponseDict(),
            second = second.ToResponseDict()
        });
    }
}