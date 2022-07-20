using CartService.Database.Model;
using CartService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using Utility.EFCore;
using Utility.Other.Enums;
using Utility.Other.Extensions;

namespace CartService.Controllers
{
    [ApiController]
    [Route("carts")]
    public class CartController : Controller
    {
        private readonly DbContext _context;
        private readonly RemoteCheckoutService checkoutService;
        public CartController(DbContext context, RemoteCheckoutService checkoutService)
        {
            _context = context;
            this.checkoutService = checkoutService;
        }

        [HttpGet("{id:guid}")]
        public async Task<object> Get([FromRoute] Guid id)
        {
            var cart = await _context.Set<ShoppingCart>()
                .Include(x => x.CartItems)
                .FirstOrDefaultAsync(x => x.Id == id);

            return Ok(cart?.ToResponseDict(ToResponseDictOptions.None));
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<object> GetList([FromRoute] Guid userId)
        {
            var carts = await _context.Set<ShoppingCart>()
                .Include(x => x.CartItems)
                .Where(x => x.UserId == userId)
                .ToListAsync();

            return Ok(carts.Select(x => x.ToResponseDict(ToResponseDictOptions.None)));
        }

        [HttpPost("create/{userId:guid}")]
        public async Task<object> CreateCart([FromRoute] Guid userId)
        {
            var cart = new ShoppingCart()
            {
                UserId = userId,
            };

            cart = await _context.TransactionAsync(x => _context.AddAsync(cart));

            return Ok(cart.ToResponseDict());
        }

        [HttpPut("{cartId:guid}")]
        public async Task<object> AddToCart([FromRoute] Guid cartId, [FromBody] List<CartItem> cartItems)
        {
            var cart = await _context.Set<ShoppingCart>()
                .Include(x => x.CartItems)
                .FirstOrDefaultAsync(x => x.Id == cartId);

            if (cart == null) return BadRequest("cart not found");

            cart.CartItems = cartItems;

            cart = await _context.TransactionAsync(x => _context.Update(cart));

            return Ok(cart.ToResponseDict(ToResponseDictOptions.None));
        }

        [HttpPost("{cartId:guid}/add-order")]
        public async Task<object> AddOrderToCart([FromRoute] Guid cartId, [FromBody] Guid orderId)
        {
            var cart = await _context.Set<ShoppingCart>()
                .Include(x => x.CartItems)
                .FirstOrDefaultAsync(x => x.Id == cartId);

            if (cart == null) return BadRequest("cart not found");
            if (cart.OrderId != null) return BadRequest("cart is already used in an order");

            if(!(await checkoutService.OrderExists(orderId)))
            {
                return BadRequest("order could not be found");
            }

            cart.OrderId = orderId;

            await _context.SaveChangesAsync();

            return Ok(cart.ToResponseDict(ToResponseDictOptions.None));
        }
    }
}
