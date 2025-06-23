using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BoostOrder.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public string Type { get; set; }

        [JsonPropertyName("stock_quantity")]
        public int StockQuantity { get; set; }

        [JsonPropertyName("regular_price")]
        public double RegularPrice { get; set; }
        public ICollection<ProductImage> Images { get; set; }
        public ICollection<ProductVariation> Variations { get; set; }
    }
}
