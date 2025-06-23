using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BoostOrder.Models
{
    public class ProductImage
    {
        [Key]
        public Guid Id { get; set; }

        [JsonPropertyName("src")]
        public string Src { get; set; }

        [JsonPropertyName("src_small")]
        public string SrcSmall { get; set; }

        [JsonPropertyName("src_medium")]
        public string SrcMedium { get; set; }

        [JsonPropertyName("src_large")]
        public string SrcLarge { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
