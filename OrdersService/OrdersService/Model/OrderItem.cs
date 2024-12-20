using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OrdersService.Model
{
    [Table("OrderItems")]
    public class OrderItem
    {
        private decimal price;
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price
        {
            get => Math.Round(price, 2); // Округляем значение до двух знаков после запятой
            set => price = value; // Устанавливаем значение без изменений
        }

        public virtual Order Order { get; set; }
    }
}
