using System.ComponentModel.DataAnnotations;

namespace ClothingStore.DTOs
{
    public class ChangePhoneNumberDTO
    {
        [Required]
        [Phone]
        [MinLength(10)]
        [MaxLength(18)]
        public string PhoneNumberNew { get; set; }
    }
}
