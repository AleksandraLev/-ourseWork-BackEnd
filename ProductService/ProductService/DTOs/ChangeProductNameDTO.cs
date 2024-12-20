using System.ComponentModel.DataAnnotations;

namespace ProductsService.DTOs
{
    public class ChangeProductNameDTO
    {
        [Key]
        [Range(0, int.MaxValue, ErrorMessage = "Значение должно быть от 0 до максимального значения int.")]
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }
    }
}
