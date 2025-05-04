using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CartService.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        [JsonIgnore]
        public Cart Cart { get; set; } // Navigation property to the cart
        public int ServiceId { get; set; } // ID of the product
        public Service Service { get; set; }
        public decimal Price { get; set; }
    }

}