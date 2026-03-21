namespace QuanLyBanSach.Models.ViewModels
{
    public class DashboardFullViewModel
    {
        public Summary Summary { get; set; }
        public List<RevenueByMonth> RevenueByMonth { get; set; }
        public List<TopBook> TopBooks { get; set; }
        public List<RecentOrder> RecentOrders { get; set; }
    }

    public class Summary
    {
        public int TotalBooks { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class RevenueByMonth
    {
        public int Month { get; set; }
        public decimal Revenue { get; set; }
    }

    public class TopBook
    {
        public string BookName { get; set; }
        public int TotalSold { get; set; }
    }

    public class RecentOrder
    {
        public int OrderID { get; set; }
        public string ReceiverName { get; set; }
        public decimal Total { get; set; }
        public int Status { get; set; }
    }
}
