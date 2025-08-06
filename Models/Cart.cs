using Microsoft.AspNetCore.Identity;

namespace easy_shop.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;

        public decimal TotalPrice { get; set; }

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        public IdentityUser? User { get; set; } // AspNetUsers bilan bog'lanish
    }

}
