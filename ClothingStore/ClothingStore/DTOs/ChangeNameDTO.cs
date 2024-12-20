using System.ComponentModel.DataAnnotations;

namespace ClothingStore.DTOs
{
    public class ChangeNameDTO
    {
        [Required]
        [MaxLength(128)]
        public string Name { get; set; }
    }
}
