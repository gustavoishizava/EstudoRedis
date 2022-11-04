using WebApi.Models;

namespace WebApi.Services
{
    public interface IProductService
    {
        Task<Product> GetByNameAsync(string name);
    }
}