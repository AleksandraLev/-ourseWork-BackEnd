using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProductsService.DTOs
{
    public class ChangeStockQuantityDTO
    {
        [Key]
        [Range(0, int.MaxValue, ErrorMessage = "Значение должно быть от 0 до максимального значения int.")]
        public int Id { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Значение должно быть от 0 до максимального значения int.")]
        public int StockQuantity { get; set; }
    }
}
