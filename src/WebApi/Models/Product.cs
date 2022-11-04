namespace WebApi.Models
{
    public class Product
    {
        public int Id { get; init; }
        public string Name { get; set; }
        public string Ean { get; init; }
        public string Description { get; init; }

        public Product(int id, string name, string ean, string description)
        {
            Id = id;
            Name = name;
            Ean = ean;
            Description = description;
        }
    }
}