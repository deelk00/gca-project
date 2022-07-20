namespace CartService.Services
{
    public class RemoteCheckoutService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;
        private readonly string checkoutServiceUrl;
        public RemoteCheckoutService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.checkoutServiceUrl = string.Join("/", configuration.GetConnectionString("checkout-service").Split('/').Append("orders"));
        }

        public async Task<bool> OrderExists(Guid orderId)
        {
            var client = httpClientFactory.CreateClient();
            var response = await client.GetAsync(Path.Join(checkoutServiceUrl, orderId.ToString()));
            Console.WriteLine(Path.Join(checkoutServiceUrl, orderId.ToString()));
            Console.WriteLine(response.ReasonPhrase);
            return response.IsSuccessStatusCode;
        }
    }
}
