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
                System.Diagnostics.Debug.WriteLine($"========== CreateOrder START ==========");
                System.Diagnostics.Debug.WriteLine($"Request: ReceiverName={request.ReceiverName}, Phone={request.ReceiverPhone}, Address={request.ReceiverAddress}, Total={request.Total}");

                // Kiểm tra đăng nhập
                var customerId = HttpContext.Session.GetInt32("CustomerID");
                var customerName = HttpContext.Session.GetString("CustomerName");

                System.Diagnostics.Debug.WriteLine($"Session - CustomerId: {customerId}, CustomerName: {customerName}");

                if (customerId == null || string.IsNullOrEmpty(customerName))
                {
                    return Unauthorized(new { success = false, message = "Vui lòng đăng nhập trước" });
                }

                // Lấy cart từ session
                var cart = GetCart();
                System.Diagnostics.Debug.WriteLine($"Cart items: {cart.Count}");

                if (cart.Count == 0)
                {
                    return BadRequest(new { success = false, message = "Giỏ hàng trống" });
                }

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

                System.Diagnostics.Debug.WriteLine($"Order object created - CustomerID: {order.CustomerID}, Total: {order.OrderTotalAmount}");

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

                System.Diagnostics.Debug.WriteLine($"OrderRequestDTO created: CustomerID={orderDTO.CustomerID}");

                // Test connection to Backend API
                System.Diagnostics.Debug.WriteLine($"Testing connection to Backend API...");
                bool apiConnected = await _orderAPI.TestConnection();
                if (!apiConnected)
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR: Cannot connect to Backend API at {baseUrl}/orders");
                    return BadRequest(new { success = false, message = "❌ Không thể kết nối Backend API (localhost:5000). Vui lòng kiểm tra:\n1. Backend API có chạy không?\n2. Port 5000 có đang dùng không?" });
                }

                System.Diagnostics.Debug.WriteLine($"Backend API connection OK");

                // Lưu Order
                bool orderAdded = await _orderAPI.Add(orderDTO);
                System.Diagnostics.Debug.WriteLine($"Order.Add() returned: {orderAdded}");

                if (!orderAdded)
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR: Failed to add order to database");
                    return BadRequest(new { success = false, message = "Lỗi khi tạo đơn hàng. Vui lòng kiểm tra kết nối Backend API (localhost:5000/orders/add)" });
                }

                // Lấy danh sách đơn hàng để lấy ID vừa tạo
                System.Diagnostics.Debug.WriteLine($"Fetching all orders to get new order ID...");
                var orders = await _orderAPI.GetAll();
                System.Diagnostics.Debug.WriteLine($"Total orders in system: {orders?.Count}");

                if (orders == null || orders.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR: No orders found in database");
                    return BadRequest(new { success = false, message = "Không thể lấy dữ liệu đơn hàng từ database" });
                }

                var newOrder = orders.LastOrDefault();

                if (newOrder == null)
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR: Last order is null");
                    return BadRequest(new { success = false, message = "Không thể lấy ID đơn hàng vừa tạo" });
                }

                System.Diagnostics.Debug.WriteLine($"New order ID: {newOrder.OrderID}");

                // Tạo OrderDetail cho từng sản phẩm
                int detailCount = 0;
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

                        bool detailAdded = await _orderDetailAPI.Add(orderDetail);
                        System.Diagnostics.Debug.WriteLine($"OrderDetail for BookID {cartItem.BookId}: {(detailAdded ? "SUCCESS" : "FAILED")}");
                        if (detailAdded) detailCount++;
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Total OrderDetails added: {detailCount}/{cart.Count}");

                // Xóa giỏ hàng từ session
                HttpContext.Session.Remove(CartSessionKey);
                System.Diagnostics.Debug.WriteLine($"Cart cleared from session");

                System.Diagnostics.Debug.WriteLine($"========== CreateOrder SUCCESS ==========");
                return Ok(new { success = true, message = "Đặt hàng thành công", orderId = newOrder.OrderID });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"========== CreateOrder ERROR ==========");
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine($"========== CreateOrder ERROR END ==========");

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

    public class CheckoutItem
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public decimal BookPrice { get; set; }
        public int Quantity { get; set; }
        public decimal ItemTotal { get; set; }
    }

    public class CheckoutViewModel
    {
        public List<CheckoutItem> Items { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Shipping { get; set; }
        public decimal Total { get; set; }
        public string CustomerName { get; set; }
        public int CustomerId { get; set; }
    }

    public class OrderRequest
    {
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string ReceiverAddress { get; set; }
        public decimal Total { get; set; }
    }

    // DTO để gửi tới Backend Python (với tên field phù hợp)
    public class OrderRequestDTO
    {
        // Sử dụng lowercase property names để match với JSON được serialize
        [JsonPropertyName("customerID")]
        public int CustomerID { get; set; }

        [JsonPropertyName("customerName")]
        public string CustomerName { get; set; }

        [JsonPropertyName("receiverName")]
        public string ReceiverName { get; set; }

        [JsonPropertyName("receiverPhone")]
        public string ReceiverPhone { get; set; }

        [JsonPropertyName("receiverAddress")]
        public string ReceiverAddress { get; set; }

        [JsonPropertyName("orderTotalAmount")]
        public decimal OrderTotalAmount { get; set; }

        [JsonPropertyName("orderStatus")]
        public int OrderStatus { get; set; }

        [JsonPropertyName("orderCreatedDate")]
        public string OrderCreatedDate { get; set; }
    }
}
