using System.ComponentModel.DataAnnotations;

namespace ClothingStore.DTOs
{
    public class ChangeEmailDTO
    {
        [Required]
        [MaxLength(128)]
        [OptionalEmailAddress]
        public string Email { get; set; }
    }
}
