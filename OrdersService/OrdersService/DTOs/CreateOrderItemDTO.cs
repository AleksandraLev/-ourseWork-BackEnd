using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OrdersService.DTOs
{
    public class CreateOrderItemDTO
    {
        private decimal price;
        
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price
        {
            get => Math.Round(price, 2);
            set => price = value;
        }
    }
}
