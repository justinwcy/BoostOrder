using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BoostOrder.Models
{
    public class ProductVariation
    {
        [Key] 
        [JsonIgnore] 
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Sku { get; set; }

        [JsonPropertyName("regular_price")]
        public double RegularPrice { get; set; }

        [JsonPropertyName("stock_quantity")]
        public int StockQuantity { get; set; }

        public string UOM { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }
    }
}
