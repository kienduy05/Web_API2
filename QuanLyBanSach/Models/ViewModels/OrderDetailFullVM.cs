namespace QuanLyBanSach.Models.ViewModels
{
    public class OrderDetailFullVM
    {
        public Order Order { get; set; }
        public List<OrderDetail> Details { get; set; }
    }
}