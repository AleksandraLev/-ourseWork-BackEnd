using System.ComponentModel.DataAnnotations;

namespace ProductsService.DTOs
{
    public class ChangePriceDTO
    {

        private decimal price;

        [Key]
        [Range(0, int.MaxValue, ErrorMessage = "Значение должно быть от 0 до максимального значения int.")]
        public int Id { get; set; }

        [Required]
        public decimal Price
        {
            get => Math.Round(price, 2); // Округляем значение до двух знаков после запятой
            set => price = value; // Устанавливаем значение без изменений
        }
    }
}
