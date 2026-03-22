using System.ComponentModel.DataAnnotations;

namespace QuanLyBanSach.Models.ViewModels
{
    public class AdminChangePasswordVM
    {
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu cũ")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; }
    }
}
