using CheckoutService.Model.Database;
using CheckoutService.Model.Database.Types;
using CheckoutService.Model.Requests;
using CheckoutService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utility.EFCore;
using Utility.Other.Enums;
using Utility.Other.Extensions;

namespace CheckoutService.Controllers
{
    [ApiController]
    [Route("orders")]
    public class OrderController : Controller
    {
        private readonly DbContext _context;
        private readonly RemoteCartService _remoteCartService;
        public OrderController(DbContext context, RemoteCartService remoteCartService)
        {
            _remoteCartService = remoteCartService;
            _context = context;
        }

        [HttpGet("{id:guid}")]
        public async Task<object> GetOrder([FromRoute] Guid id)
        {
            var order = await _context.Set<Order>()
                .Include(x => x.Payment)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (order == null) return BadRequest("order not found");
            return Ok(order?.ToResponseDict(ToResponseDictOptions.None));
        }

        [HttpGet("from-user/{userId:guid}")]
        public async Task<object> GetOrdersFromUser([FromRoute] Guid userId)
        {
            var response = (await _context.Set<Order>().Where(x => x.UserId == userId).ToListAsync());
            
            return Ok(response.Select(x => x.ToResponseDict(ToResponseDictOptions.None)));
        }
        
        [HttpPost("{cartId:guid}")]
        public async Task<object> PostOrder([FromRoute] Guid cartId)
        {
            var order = new Order()
            {
                OrderStatus = Model.Database.Enums.OrderStatus.Pending,
                CartId = cartId,
            };

            var cart = await _remoteCartService.GetShoppingCart(cartId);

            if (cart == null) return BadRequest("the cart could not be found");
            try
            {
                order.UserId = cart.UserId;
                order = await _context.TransactionAsync(x => _context.AddAsync(order));
            }
            catch(Exception ex)
            {
                
            }
            
            cart = await _remoteCartService.AddOrderToCart(cart.Id, order.Id);

            if (cart == null) return BadRequest("error while processing order");

            return Ok(order.ToResponseDict());
        }

        [HttpPost("pay-for/{id:guid}")]
        public async Task<object> PayForOrder([FromRoute] Guid id, [FromBody] CreditCard card)
        {
            var order = await _context.Set<Order>()
                .Include(x => x.Payment)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (order == null) return BadRequest("Order not found");
            if (!card.IsCreditCardInfoValid()) return BadRequest("Creditcard-Info is wrong");

            var payment = new Payment()
            { 
                CreditCardNumber = card.CardNumber, 
                CreditCardExpirationDate = card.CardExpirationDate, 
                Pin = card.Pin
            };

            order.Payment = payment;
            order.OrderStatus = Model.Database.Enums.OrderStatus.Paid;

            order = await _context.TransactionAsync(x => _context.Update(order));

            return Ok(order.ToResponseDict(ToResponseDictOptions.None));
        }
    }
}
