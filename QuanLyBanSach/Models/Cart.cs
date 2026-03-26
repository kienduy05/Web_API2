using System.Text.Json.Serialization;

namespace QuanLyBanSach.Models
{
    public class AddToCartRequest
    {
        public int BookId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateQuantityRequest
    {
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }

    public class RemoveCartRequest
    {
        [JsonPropertyName("bookId")]
        public int BookId { get; set; }
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
