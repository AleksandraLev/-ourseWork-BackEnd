using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductsService.Model
{
    [Table("Category")]
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        // Навигационное свойство для связи "один ко многим"
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
