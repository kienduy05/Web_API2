    namespace QuanLyBanSach.Models
{
    public class Order
    {
        public int OrderID { get; set; }

        public int CustomerID { get; set; }

        public string CustomerName { get; set; }

        public DateTime OrderCreatedDate { get; set; }

        public string ReceiverName { get; set; }

        public string ReceiverPhone { get; set; }

        public string ReceiverAddress { get; set; }

        public decimal OrderTotalAmount { get; set; }

        public int OrderStatus { get; set; }
    }
}
