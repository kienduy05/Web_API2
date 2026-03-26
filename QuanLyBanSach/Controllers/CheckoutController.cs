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
        private readonly BookAPI _bookAPI;
        private readonly OrderAPI _orderAPI;
        private readonly OrderDetailAPI _orderDetailAPI;
        private const string baseUrl = "http://localhost:5000";

        public CheckoutController()
        {
            _bookAPI = new BookAPI();
            _orderAPI = new OrderAPI();
            _orderDetailAPI = new OrderDetailAPI();
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

            // Lấy chi tiết sản phẩm từ API
            var checkoutItems = new List<CheckoutItem>();
            decimal totalAmount = 0;

            foreach (var item in cart)
            {
                var book = await _bookAPI.GetById(item.BookId);
                if (book != null)
                {
                    decimal itemTotal = book.BookPrice * item.Quantity;
                    totalAmount += itemTotal;

                    checkoutItems.Add(new CheckoutItem
                    {
                        BookId = item.BookId,
                        BookName = book.BookName,
                        BookPrice = book.BookPrice,
                        Quantity = item.Quantity,
                        ItemTotal = itemTotal
                    });
                }
            }

            // Tạo CheckoutViewModel
            var checkoutViewModel = new CheckoutViewModel
            {
                Items = checkoutItems,
                SubTotal = totalAmount,
                Shipping = 50000,
                Total = totalAmount + 50000,
                CustomerName = customerName,
                CustomerId = customerId ?? 0
            };

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

                // Tạo đơn hàng
                var order = new Order
                {
                    CustomerID = customerId.Value,
                    CustomerName = customerName,
                    ReceiverName = request.ReceiverName,
                    ReceiverPhone = request.ReceiverPhone,
                    ReceiverAddress = request.ReceiverAddress,
                    OrderCreatedDate = DateTime.Now,
                    OrderTotalAmount = request.Total,
                    OrderStatus = 0
                };

                // Tạo DTO để gửi tới Backend Python
                var orderDTO = new OrderRequestDTO
                {
                    CustomerID = order.CustomerID,
                    CustomerName = order.CustomerName,
                    ReceiverName = order.ReceiverName,
                    ReceiverPhone = order.ReceiverPhone,
                    ReceiverAddress = order.ReceiverAddress,
                    OrderTotalAmount = order.OrderTotalAmount,
                    OrderStatus = order.OrderStatus,
                    OrderCreatedDate = order.OrderCreatedDate.ToString("yyyy-MM-ddTHH:mm:ss")
                };

                // Test connection to Backend API
                bool apiConnected = await _orderAPI.TestConnection();
                if (!apiConnected) return BadRequest(new { success = false, message = "❌ Không thể kết nối Backend API (localhost:5000)." });

                // Lưu Order
                bool orderAdded = await _orderAPI.Add(orderDTO);
                if (!orderAdded) return BadRequest(new { success = false, message = "Lỗi khi tạo đơn hàng." });

                // Lấy danh sách đơn hàng để lấy ID vừa tạo
                var orders = await _orderAPI.GetAll();
                if (orders == null || orders.Count == 0) return BadRequest(new { success = false, message = "Không thể lấy dữ liệu đơn hàng từ database" });

                var newOrder = orders.LastOrDefault();
                if (newOrder == null) return BadRequest(new { success = false, message = "Không thể lấy ID đơn hàng vừa tạo" });

                // Tạo OrderDetail cho từng sản phẩm và giảm tồn kho
                foreach (var cartItem in cart)
                {
                    var book = await _bookAPI.GetById(cartItem.BookId);
                    if (book != null)
                    {
                        var orderDetail = new OrderDetail
                        {
                            OrderID = newOrder.OrderID,
                            BookID = cartItem.BookId,
                            BookName = book.BookName,
                            Quantity = cartItem.Quantity,
                            UnitPrice = book.BookPrice
                        };

                        await _orderDetailAPI.Add(orderDetail);

                        // Giảm tồn kho
                        if (book.BookQuantity >= cartItem.Quantity)
                        {
                            book.BookQuantity -= cartItem.Quantity;
                            await _bookAPI.Update(book);
                        }
                    }
                }

                // Xóa giỏ hàng từ session
                HttpContext.Session.Remove(CartSessionKey);

                return Ok(new { success = true, message = "Đặt hàng thành công", orderId = newOrder.OrderID });
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
