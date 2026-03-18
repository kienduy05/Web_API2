namespace QuanLyBanSach.Models
{
    public class Account
    {
        public int AccountID { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public int AccountType { get; set; }

        public string AccountDisplayName { get; set; }

        public int? CustomerID { get; set; }
    }
}