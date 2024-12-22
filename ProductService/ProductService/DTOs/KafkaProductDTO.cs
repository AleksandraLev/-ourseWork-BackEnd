using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProductsService.DTOs
{
    public class KafkaProductDTO
    {
        [Key]
        public int OrderItemId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }
        [Required]
        public bool ProductExist { get; set; }

        [Required]
        public int Quantity { get; set; }
        [Required]
        public bool QuantityExist { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}
