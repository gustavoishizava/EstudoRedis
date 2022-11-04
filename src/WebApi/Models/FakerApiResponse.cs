namespace WebApi.Models
{
    public class FakerApiResponse
    {
        public int Total { get; set; }
        public List<Product> Data { get; set; } = new();
    }
}