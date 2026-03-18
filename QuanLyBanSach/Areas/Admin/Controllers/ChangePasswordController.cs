using Microsoft.AspNetCore.Mvc;
using QuanLyBanSach.Services;
using QuanLyBanSach.Models;
using QuanLyBanSach.Models.Authentication;

namespace QuanLyBanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authentication]
    public class ChangePasswordController : Controller
    {
        AccountAPI accountAPI = new AccountAPI();

        // GET
        public async Task<IActionResult> Index()
        {
            var accountId = HttpContext.Session.GetInt32("AccountID");

            if (accountId == null)
                return RedirectToAction("Login", "Access");

            var accounts = await accountAPI.GetAll();

            var account = accounts.FirstOrDefault(x => x.AccountID == accountId);

            return View(account);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Index(Account model, string ConfirmPassword)
        {
            var accountId = HttpContext.Session.GetInt32("AccountID");

            if (accountId == null)
                return RedirectToAction("Login", "Access");

            if (model.Password != ConfirmPassword)
            {
                TempData["Error"] = "Mật khẩu xác nhận không khớp";
                return View(model);
            }

            var accounts = await accountAPI.GetAll();
            var oldAccount = accounts.FirstOrDefault(x => x.AccountID == accountId);

            oldAccount.Password = model.Password;
            oldAccount.AccountDisplayName = model.AccountDisplayName;

            var result = await accountAPI.Update(oldAccount);

            if (result)
            {
                TempData["Success"] = "Đổi mật khẩu thành công";

                // cập nhật lại session tên
                HttpContext.Session.SetString("AccountDisplayName", oldAccount.AccountDisplayName);

                return RedirectToAction("Index");
            }

            TempData["Error"] = "Cập nhật thất bại";
            return View(model);
        }
    }
}