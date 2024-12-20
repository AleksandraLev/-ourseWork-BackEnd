using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using OrdersService.Model;

namespace OrdersService.DTOs
{
    public class AllOrderInfoDTO
    {
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
        public List<OrderItem> AllItems { get; set; }
        public Dictionary<string, string> GetOrderInfo()
        {
            Dictionary<string, string> orderInfo = new Dictionary<string, string>
            {
                { "OrderId", Id.ToString() },
                { "UserId", UserId.ToString() },
                { "OrderDate", OrderDate.ToString() },
                { "OrderStatus", Status },
                { "ShippingAddress", ShippingAddress },
                { "AllItems", GetOrderItemsSummary(AllItems)}
            };
            return orderInfo;
        }
        public string GetOrderItemsSummary(List<OrderItem> allItems)
        {
            if (allItems == null || allItems.Count == 0)
            {
                return "No items in the order.";
            }

            var sb = new StringBuilder();

            foreach (var item in allItems)
            {
                sb.AppendLine($"\nProductId: {item.ProductId}\n, Quantity: {item.Quantity}\n, Price: {item.Price:C2}\n");
            }

            return sb.ToString();
        }
    }
}
