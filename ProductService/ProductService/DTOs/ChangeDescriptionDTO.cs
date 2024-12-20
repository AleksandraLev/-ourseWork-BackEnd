using System.ComponentModel.DataAnnotations;

namespace ProductsService.DTOs
{
    public class ChangeDescriptionDTO
    {
        [Key]
        [Range(0, int.MaxValue, ErrorMessage = "Значение должно быть от 0 до максимального значения int.")]
        public int Id { get; set; }

        [MaxLength(512)]
        [Range(0, int.MaxValue, ErrorMessage = "Значение должно быть от 0 до максимального значения int.")]
        public string Description { get; set; }
    }
}
