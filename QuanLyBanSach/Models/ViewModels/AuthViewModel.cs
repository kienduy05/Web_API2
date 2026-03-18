namespace QuanLyBanSach.Models.ViewModels
{
    public class AuthViewModel
    {
        public Account Login { get; set; } = new Account();

        public RegisterViewModel Register { get; set; } = new RegisterViewModel();
    }
}