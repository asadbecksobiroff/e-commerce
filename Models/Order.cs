using Microsoft.AspNetCore.Identity;

namespace easy_shop.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;

        public decimal TotalPrice { get; set; }

        public string Status { get; set; } = null!;
        
        public string? Location { get; set; }

        public DateTime OrderDate { get; set; }
        public string? PaymentStatus { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public IdentityUser? User { get; set; }
    }

}
