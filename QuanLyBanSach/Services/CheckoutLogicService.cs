using QuanLyBanSach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyBanSach.Services
{
    public class CheckoutLogicService
    {
        private readonly BookAPI _bookAPI;
        private readonly OrderAPI _orderAPI;
        private readonly OrderDetailAPI _orderDetailAPI;

        public CheckoutLogicService()
        {
            _bookAPI = new BookAPI();
            _orderAPI = new OrderAPI();
            _orderDetailAPI = new OrderDetailAPI();
        }

        public async Task<CheckoutViewModel> GetCheckoutViewModelAsync(List<CartItem> cart, string customerName, int customerId)
        {
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

            return new CheckoutViewModel
            {
                Items = checkoutItems,
                SubTotal = totalAmount,
                Shipping = 50000,
                Total = totalAmount + 50000,
                CustomerName = customerName,
                CustomerId = customerId
            };
        }

        public async Task<(bool success, string message, int? orderId)> CreateOrderAsync(
            OrderRequest request, 
            List<CartItem> cart, 
            int customerId, 
            string customerName)
        {
            var order = new Order
            {
                CustomerID = customerId,
                CustomerName = customerName,
                ReceiverName = request.ReceiverName,
                ReceiverPhone = request.ReceiverPhone,
                ReceiverAddress = request.ReceiverAddress,
                OrderCreatedDate = DateTime.Now,
                OrderTotalAmount = request.Total,
                OrderStatus = 0
            };

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

            bool apiConnected = await _orderAPI.TestConnection();
            if (!apiConnected) return (false, "❌ Không thể kết nối Backend API (localhost:5000).", null);

            bool orderAdded = await _orderAPI.Add(orderDTO);
            if (!orderAdded) return (false, "Lỗi khi tạo đơn hàng.", null);

            var orders = await _orderAPI.GetAll();
            if (orders == null || orders.Count == 0) return (false, "Không thể lấy dữ liệu đơn hàng từ database", null);

            var newOrder = orders.LastOrDefault();
            if (newOrder == null) return (false, "Không thể lấy ID đơn hàng vừa tạo", null);

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

                    if (book.BookQuantity >= cartItem.Quantity)
                    {
                        book.BookQuantity -= cartItem.Quantity;
                        await _bookAPI.Update(book);
                    }
                }
            }
            return (true, "Đặt hàng thành công", newOrder.OrderID);
        }
    }
}