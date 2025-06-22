using System.ComponentModel.DataAnnotations;

namespace BoostOrder.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public string Type { get; set; }
        public int StockQuantity { get; set; }
        public double RegularPrice { get; set; }
        public ICollection<ProductImage> Images { get; set; }
    }
}
