using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using WebApi.Models;

namespace WebApi.Services
{
    public class ProductService : IProductService
    {
        private readonly ILogger<ProductService> _logger;
        private readonly IDistributedCache _distributedCache;
        private readonly HttpClient _httpClient;

        public ProductService(ILogger<ProductService> logger, IDistributedCache distributedCache, HttpClient httpClient)
        {
            _logger = logger;
            _distributedCache = distributedCache;
            _httpClient = httpClient;
        }

        public async Task<Product> GetByNameAsync(string name)
        {
            var product = await GetCachedAsync(key: name);
            if (product is null)
                product = await GetApiAsync(name: name);

            return product;
        }

        private async Task<Product?> GetCachedAsync(string key)
        {
            var json = await _distributedCache.GetStringAsync(key);
            if (string.IsNullOrEmpty(json))
            {
                _logger.LogInformation("Product not found in cache.");
                return null;
            }

            _logger.LogInformation("Product found in cache.");

            return JsonSerializer.Deserialize<Product>(json, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });
        }

        private async Task AddCacheAsync(Product product)
        {
            _logger.LogInformation("Product added to cache.");

            var bytes = JsonSerializer.SerializeToUtf8Bytes(product);
            var options = new DistributedCacheEntryOptions();
            options.SetSlidingExpiration(TimeSpan.FromSeconds(15));

            await _distributedCache.SetAsync(key: product.Name, value: bytes, options: options);
        }

        private async Task<Product> GetApiAsync(string name)
        {
            _logger.LogInformation("Getting product in API.");

            var result = await _httpClient.GetAsync($"/api/v1/products?_quantity=1");
            var response = JsonSerializer.Deserialize<FakerApiResponse>(await result.Content.ReadAsStringAsync(), new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });

            var product = response!.Data.First();
            product.Name = name;

            await AddCacheAsync(product);

            return product;
        }
    }
}