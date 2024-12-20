using System.ComponentModel.DataAnnotations;

namespace ClothingStore.DTOs
{
    public class RegistrationDTO
    {
        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        [Required]
        [Phone]
        [MinLength(10)]
        [MaxLength(18)]
        public string PhoneNumber { get; set; }

        [MaxLength(128)]
        [OptionalEmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(255)]
        public string Password { get; set; }
    }
}
