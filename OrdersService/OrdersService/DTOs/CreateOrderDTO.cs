using OrdersService.Model;
using System.ComponentModel.DataAnnotations;

namespace OrdersService.DTOs
{
    public class CreateOrderDTO
    {
        [Required]
        [MaxLength(512)]
        public string ShippingAddress { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "Order must contain at least one item.")]
        public List<CreateOrderItemDTO> Items { get; set; }
    }
}
