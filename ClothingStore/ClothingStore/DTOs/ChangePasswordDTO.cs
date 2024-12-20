using System.ComponentModel.DataAnnotations;

namespace ClothingStore.DTOs
{
    public class ChangePasswordDTO
    {
        [Required]
        [MaxLength(255)]
        public string Password { get; set; }
    }
}
