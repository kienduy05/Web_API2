using QuanLyBanSach.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyBanSach.Services
{
    public class CartService
    {
        private readonly BookAPI _bookAPI;

        public CartService()
        {
            _bookAPI = new BookAPI();
        }

        public async Task<List<CartItemDetail>> GetCartDetailsAsync(List<CartItem> cart)
        {
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
            return cartItems;
        }

        public async Task<(bool success, string message, CartItem newItem)> AddToCartAsync(int bookId, int quantity, List<CartItem> currentCart)
        {
            if (bookId <= 0 || quantity <= 0)
                return (false, "Dữ liệu không hợp lệ", null);

            var book = await _bookAPI.GetById(bookId);
            if (book == null)
                return (false, "Sách không tồn tại", null);

            var existingItem = currentCart.FirstOrDefault(c => c.BookId == bookId);
            int totalQuantityInCart = quantity + (existingItem != null ? existingItem.Quantity : 0);

            if (totalQuantityInCart > book.BookQuantity)
                return (false, $"chỉ mua tối đa = {book.BookQuantity}", null);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                return (true, "Thêm vào giỏ hàng thành công", null);
            }
            
            var newItem = new CartItem { BookId = bookId, Quantity = quantity };
            return (true, "Thêm vào giỏ hàng thành công", newItem);
        }

        public async Task<(bool success, string message)> UpdateQuantityAsync(int bookId, int quantity, List<CartItem> currentCart)
        {
            if (bookId <= 0 || quantity <= 0)
                return (false, "Dữ liệu không hợp lệ");

            var book = await _bookAPI.GetById(bookId);
            if (book == null)
                return (false, "Sách không tồn tại");

            if (quantity > book.BookQuantity)
                return (false, $"chỉ mua tối đa = {book.BookQuantity}");

            var cartItem = currentCart.FirstOrDefault(c => c.BookId == bookId);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                return (true, "Cập nhật số lượng thành công");
            }

            return (false, "Sản phẩm không có trong giỏ hàng");
        }
    }
}