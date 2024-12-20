using System.ComponentModel.DataAnnotations;

namespace ClothingStore.DTOs
{
    public class SignInDTO
    {
        [Required]
        [Phone]
        [MinLength(10)]
        [MaxLength(18)]
        public string PhoneNumber { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string Password { get; set; }
        
        [Required]
        public string DeviceId { get; set; }
    }
}
