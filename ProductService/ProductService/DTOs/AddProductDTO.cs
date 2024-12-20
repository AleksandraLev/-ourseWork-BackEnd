using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProductsService.DTOs
{
    public class AddProductDTO
    {
        private decimal _price;
        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        [Required]
        public decimal Price
        {
            get => Math.Round(_price, 2); // Округляем значение до двух знаков после запятой
            set => _price = value; // Устанавливаем значение без изменений
        }
        [MaxLength(512)]
        public string Description { get; set; }

        [Required]
        [MaxLength(128)]
        public string CategoryName { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Значение должно быть от 0 до максимального значения int.")]
        public int StockQuantity { get; set; }
    }
}
