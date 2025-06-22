using System.ComponentModel.DataAnnotations;

namespace BoostOrder.Models
{
    public class ProductImage
    {
        [Key]
        public Guid Id { get; set; }
        public string Src { get; set; }
        public string Src_Small { get; set; }
        public string Src_Medium { get; set; }
        public string Src_Large { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
