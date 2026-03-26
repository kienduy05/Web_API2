using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using QuanLyBanSach.Services;
using QuanLyBanSach.Models;

namespace QuanLyBanSach.Controllers
{
    public class CartController : Controller
    {
        private const string CartSessionKey = "Cart";
        private readonly BookAPI _bookAPI;

        public CartController()
        {
            _bookAPI = new BookAPI();
        }

        public async Task<IActionResult> Index()
        {
            // Get cart from session
            var cart = GetCart();

            // Get book details for each cart item
            var cartItems = new List<CartItemDetail>();
            foreach (var item in cart)
            {
                var book = await _bookAPI.GetById(item.BookId);
                if (book != null)
                {
                    cartItems.Add(new CartItemDetail
                    {
                        BookId = item.BookId,
                        BookName = book.BookName,
                        BookPrice = book.BookPrice,
                        BookImage = book.BookImage,
                        PublisherName = book.PublisherName,
                        Quantity = item.Quantity
                    });
                }
            }

            return View(cartItems);
        }

        [HttpPost]
        public IActionResult AddToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                // Validate input
                if (request.BookId <= 0 || request.Quantity <= 0)
                {
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                // Get cart from session
                var cart = GetCart();

                // Check if item already in cart
                var existingItem = cart.FirstOrDefault(c => c.BookId == request.BookId);

                if (existingItem != null)
                {
                    existingItem.Quantity += request.Quantity;
                }
                else
                {
                    cart.Add(new CartItem 
                    { 
                        BookId = request.BookId, 
                        Quantity = request.Quantity 
                    });
                }

                // Save cart to session
                SaveCart(cart);

                return Json(new { success = true, message = "Thêm vào giỏ hàng thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        private List<CartItem> GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItem>();
            }
            return JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CartSessionKey, cartJson);
        }
    }

    public class AddToCartRequest
    {
        public int BookId { get; set; }
        public int Quantity { get; set; }
    }

    public class CartItem
    {
        public int BookId { get; set; }
        public int Quantity { get; set; }
    }

    public class CartItemDetail
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public decimal BookPrice { get; set; }
        public string BookImage { get; set; }
        public string PublisherName { get; set; }
        public int Quantity { get; set; }
    }
}
