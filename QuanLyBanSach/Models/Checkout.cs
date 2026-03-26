using System.Text.Json.Serialization;

namespace QuanLyBanSach.Models
{
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

    public class OrderRequestDTO
    {
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
