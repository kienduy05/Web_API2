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
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                // Validate input
                if (request.BookId <= 0 || request.Quantity <= 0)
                {
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                // Get book info to check stock
                var book = await _bookAPI.GetById(request.BookId);
                if (book == null)
                {
                    return Json(new { success = false, message = "Sách không tồn tại" });
                }

                // Get cart from session
                var cart = GetCart();

                // Check if item already in cart
                var existingItem = cart.FirstOrDefault(c => c.BookId == request.BookId);
                int totalQuantityInCart = request.Quantity;

                if (existingItem != null)
                {
                    totalQuantityInCart = existingItem.Quantity + request.Quantity;
                }

                // Check if total quantity exceeds available stock
                if (totalQuantityInCart > book.BookQuantity)
                {
                    return Json(new { success = false, message = $"chỉ mua tối đa = {book.BookQuantity}" });
                }

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

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int id, [FromBody] UpdateQuantityRequest request)
        {
            try
            {
                if (request == null || id <= 0 || request.Quantity <= 0)
                {
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                var book = await _bookAPI.GetById(id);
                if (book == null) return Json(new { success = false, message = "Sách không tồn tại" });

                if (request.Quantity > book.BookQuantity)
                {
                    return Json(new { success = false, message = $"chỉ mua tối đa = {book.BookQuantity}" });
                }

                var cart = GetCart();
                var cartItem = cart.FirstOrDefault(c => c.BookId == id);
                if (cartItem != null)
                {
                    cartItem.Quantity = request.Quantity;
                    SaveCart(cart);
                    return Json(new { success = true, message = "Cập nhật số lượng thành công" });
                }

                return Json(new { success = false, message = "Sản phẩm không có trong giỏ hàng" });
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
