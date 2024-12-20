using System.ComponentModel.DataAnnotations;

namespace ProductsService.DTOs
{
    public class ProductInfoDTO
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
    }
}
