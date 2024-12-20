using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProductsService.Model
{
    [Table("Product")]
    public class Product
    {
        private decimal price;

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        [Required]
        public decimal Price
        {
            get => Math.Round(price, 2); // Округляем значение до двух знаков после запятой
            set => price = value; // Устанавливаем значение без изменений
        }

        [MaxLength(512)]
        public string Description { get; set; }

        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Значение должно быть от 0 до максимального значения int.")]
        public int StockQuantity { get; set; }

        // Навигационное свойство для связи "один ко многим"
        public virtual Category Category { get; set; }
    }
}
