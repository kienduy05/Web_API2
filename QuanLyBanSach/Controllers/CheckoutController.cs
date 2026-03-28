using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using QuanLyBanSach.Services;
using QuanLyBanSach.Models;
using static QuanLyBanSach.Controllers.CartController;

namespace QuanLyBanSach.Controllers
{
    public class CheckoutController : Controller
    {
        private const string CartSessionKey = "Cart";
        private readonly CheckoutLogicService _checkoutLogicService;
        private const string baseUrl = "http://localhost:5000";

        public CheckoutController()
        {
            _checkoutLogicService = new CheckoutLogicService();
        }

        public async Task<IActionResult> Index()
        {
            // Kiểm tra xem có đăng nhập không
            var customerName = HttpContext.Session.GetString("CustomerName");
            var customerId = HttpContext.Session.GetInt32("CustomerID");

            if (string.IsNullOrEmpty(customerName) || customerId == null)
            {
                return RedirectToAction("Login", "Access");
            }

            // Lấy cart từ session
            var cart = GetCart();

            if (cart.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            // Lấy CheckoutViewModel từ service
            var checkoutViewModel = await _checkoutLogicService.GetCheckoutViewModelAsync(cart, customerName, customerId.Value);

            return View(checkoutViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
        {
            try
            {
                // Kiểm tra đăng nhập
                var customerId = HttpContext.Session.GetInt32("CustomerID");
                var customerName = HttpContext.Session.GetString("CustomerName");

                if (customerId == null || string.IsNullOrEmpty(customerName))
                {
                    return Unauthorized(new { success = false, message = "Vui lòng đăng nhập trước" });
                }

                // Lấy cart từ session
                var cart = GetCart();
                if (cart.Count == 0) return BadRequest(new { success = false, message = "Giỏ hàng trống" });

                // Call the service to create order
                var result = await _checkoutLogicService.CreateOrderAsync(request, cart, customerId.Value, customerName);

                if (!result.success)
                {
                    return BadRequest(new { success = false, message = result.message });
                }

                // Xóa giỏ hàng từ session
                HttpContext.Session.Remove(CartSessionKey);

                return Ok(new { success = true, message = result.message, orderId = result.orderId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Lỗi hệ thống: {ex.Message}" });
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
    }
}
