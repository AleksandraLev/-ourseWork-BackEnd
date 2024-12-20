using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrdersService.Model
{
    [Table("Orders")]
    public class Order
    {
        public Order()
        {
            Status = Statuses[0];
        }
        public void ChangeStatus(int numStatus)
        {
            Status = Statuses[numStatus];
        }
        public void CanceleStatus()
        {
            Status = Statuses[4];
        }
        public void ProcessStatus()
        {
            Status = Statuses[1];
        }
        public void ShipStatus()
        {
            Status = Statuses[2];
        }
        public void CompleteStatus()
        {
            Status = Statuses[3];
        }
        public void CreateStatus()
        {
            Status = Statuses[0];
        }
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        [MaxLength(512)]
        public string ShippingAddress { get; set; }

        public virtual ICollection<OrderItem> Items { get; set; }

        public static readonly string[] Statuses =
        {
            "Created",
            "Processing",
            "Shipped",
            "Completed",
            "Canceled"
        };
    }
}
