using CartService.Interface;
using CartService.Models;
using CatalogService.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CartService.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly CartDbContext dbContext;

        public CartRepository(CartDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task ClearCartAsync(int userId)
        {
            var cart = await dbContext.Carts
                 .Include(c => c.Items)
                 .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                dbContext.CartItems.RemoveRange(cart.Items); // Remove all items
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<CartItem>> GetCartItems(int userId)
        {
            var cart = await dbContext.Carts
                 .Include(c => c.Items)
                 .ThenInclude(ci => ci.Service)
                 .FirstOrDefaultAsync(c => c.UserId == userId);

            return cart?.Items.ToList() ?? new List<CartItem>();
        }

        public async Task AddToCart(int userId, int productId, int quantity)
        {
            //var userID = GetUserId();

            var cart = await dbContext.Carts
                 .Include(c => c.Items)
                 .FirstOrDefaultAsync(c => c.UserId == userId);

            var product = await productRepository.GetByIdAsync(productId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId, Items = new List<CartItem>() };
                dbContext.Carts.Add(cart);
            }

            var cartItem = cart.Items.FirstOrDefault(ci => ci.ProductId == productId);

            //var total = cart.Items.Select(c => c.Product.Price * c.Quantity).Sum();
            if (cartItem == null)
            {
                cartItem = new CartItem { ProductId = productId, Quantity = quantity, Product = product, Price = product.Price };
                cart.Items.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity; // Increase quantity
            }

            dbContext.Entry(product).State = EntityState.Unchanged;
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateCartItem(int userId, int cartItemId, int quantity)
        {
            var cart = await dbContext.Carts
                       .Include(c => c.Items)
                       .FirstOrDefaultAsync(c => c.UserId == userId);

            var cartItems = await this.GetCartItems(userId);
            var item = cartItems.FirstOrDefault(ci => ci.CartItemId == cartItemId);

            var cartItem = cart?.Items.FirstOrDefault(ci => ci.CartItemId == cartItemId);
            if (item != null)
            {
                item.Quantity = quantity;
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task RemoveFromCart(int userId, int serviceId)
        {
            var cart = await dbContext.Carts
                        .Include(c => c.Items)
                        .FirstOrDefaultAsync(c => c.UserId == userId);

            var cartItem = dbContext.CartItems.FirstOrDefault(ci => ci.ServiceId == serviceId);
            if (cartItem != null)
            {
                cart.Items.Remove(cartItem);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<decimal> GetShoppingCartTotal(int userId)
        {
            var cart = await dbContext.Carts
                  .Include(c => c.Items)
                  .FirstOrDefaultAsync(c => c.UserId == userId);

            var total = cart.Items.Select(c => c.Product.Price * c.Quantity).Sum();
            return total;
        }

        public async Task<int> GetNumberOfCartItem(int userId)
        {
            var cart = await dbContext.Carts
                 .Include(c => c.Items)
                 .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || cart.Items == null)
            {
                return 0; // Return 0 if the cart or items are not found
            }

            var number = cart.Items.Sum(c => c.Quantity);
            return number;
        }
    }
}
