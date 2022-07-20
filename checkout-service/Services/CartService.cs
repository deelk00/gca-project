using CheckoutService.Model.Types;

namespace CheckoutService.Services
{
    public class RemoteCartService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly string _cartServiceUrl;
        public RemoteCartService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _cartServiceUrl = string.Join("/", configuration.GetConnectionString("cart-service").Split('/').Append("carts"));
        }

        public async Task<ShoppingCart?> GetShoppingCart(Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(Path.Join(_cartServiceUrl, id.ToString()));

            if (!response.IsSuccessStatusCode) return null;

            var cart = await response.Content.ReadFromJsonAsync<ShoppingCart?>();

            return cart;
        }

        public async Task<ShoppingCart?> AddOrderToCart(Guid cartId, Guid orderId)
        {
            var client = _httpClientFactory.CreateClient();
            var content = JsonContent.Create(new { orderId = orderId });

            var response = await client.PostAsync(Path.Join(_cartServiceUrl, cartId.ToString(), "add-order"), content);

            if (!response.IsSuccessStatusCode) return null;

            var cart = await response.Content.ReadFromJsonAsync<ShoppingCart?>();

            return cart;
        }
    }
}
