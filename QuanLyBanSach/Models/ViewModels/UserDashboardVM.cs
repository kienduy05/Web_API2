namespace QuanLyBanSach.Models.ViewModels
{
    public class UserDashboardVM
    {
        public Customer Customer { get; set; }
        public List<Order> Orders { get; set; }
        public decimal TotalSpent { get; set; }
    }
}