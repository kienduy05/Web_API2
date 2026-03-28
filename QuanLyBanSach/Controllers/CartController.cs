using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using QuanLyBanSach.Services;
using QuanLyBanSach.Models;

namespace QuanLyBanSach.Controllers
{
    public class CartController : Controller
    {
        private const string CartSessionKey = "Cart";
        private readonly CartService _cartService;

        public CartController()
        {
            _cartService = new CartService();
        }

        public async Task<IActionResult> Index()
        {
            // Get cart from session
            var cart = GetCart();

            // Get book details for each cart item
            var cartItems = await _cartService.GetCartDetailsAsync(cart);

            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                var cart = GetCart();
                var result = await _cartService.AddToCartAsync(request.BookId, request.Quantity, cart);

                if (!result.success)
                {
                    return Json(new { success = false, message = result.message });
                }

                if (result.newItem != null)
                {
                    cart.Add(result.newItem);
                }

                SaveCart(cart);
                return Json(new { success = true, message = result.message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int id, [FromBody] UpdateQuantityRequest request)
        {
            try
            {
                if (request == null)
                {
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                var cart = GetCart();
                var result = await _cartService.UpdateQuantityAsync(id, request.Quantity, cart);

                if (!result.success)
                {
                    return Json(new { success = false, message = result.message });
                }

                SaveCart(cart);
                return Json(new { success = true, message = result.message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult RemoveFromCart([FromBody] RemoveCartRequest request)
        {
            try
            {
                if (request.BookId <= 0) return Json(new { success = false, message = "Dữ liệu không hợp lệ" });

                var cart = GetCart();
                var cartItem = cart.FirstOrDefault(c => c.BookId == request.BookId);

                if (cartItem != null)
                {
                    cart.Remove(cartItem);
                    SaveCart(cart);
                    return Json(new { success = true, message = "Xóa sản phẩm thành công" });
                }

                return Json(new { success = false, message = "Sản phẩm không có trong giỏ hàng" });
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
}
