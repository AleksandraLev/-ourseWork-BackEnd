using System.ComponentModel.DataAnnotations;

namespace ProductsService.DTOs
{
    public class CategoryDTO
    {
        [Required]
        [MaxLength(128)]
        public string Name { get; set; }
    }
}
