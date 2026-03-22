using Microsoft.AspNetCore.Mvc;
using QuanLyBanSach.Models;
using QuanLyBanSach.Models.ViewModels;
using QuanLyBanSach.Services;

namespace QuanLyBanSach.Controllers
{
    public class UserController : Controller
    {
        private  UserAPI _userAPI = new UserAPI();

        

        public async Task<IActionResult> Index()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerID");

            if (customerId == null)
                return RedirectToAction("Login", "Access");

            var data = await _userAPI.GetDashboard(customerId.Value);

            return View(data);
        }

        

        // GET: EDIT PROFILE
        public async Task<IActionResult> Edit()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerID");

            if (customerId == null)
                return RedirectToAction("Login", "Access");

            var data = await _userAPI.GetDashboard(customerId.Value);

            return View(data.Customer);
        }

        // POST: UPDATE PROFILE
        [HttpPost]
        public async Task<IActionResult> Edit(Customer model)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerID");

            if (customerId == null)
                return RedirectToAction("Login", "Access");

            var result = await _userAPI.UpdateProfile(customerId.Value, model);

            if (!result.success)
            {
                TempData["Error"] = result.message;
                return View(model);
            }

            // cập nhật lại session name
            HttpContext.Session.SetString("CustomerName", model.CustomerName);

            TempData["Success"] = "Cập nhật thành công";

            return RedirectToAction("Index");
        }


        // GET: CHANGE PASSWORD
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            var accountId = HttpContext.Session.GetInt32("AccountID");

            if (accountId == null)
                return RedirectToAction("Login", "Access");

            // ✅ validate tự động từ DataAnnotation
            if (!ModelState.IsValid)
                return View(model);

            // ✅ validate logic riêng
            if (model.NewPassword != model.ConfirmPassword)
            {
                TempData["Error"] = "Mật khẩu xác nhận không khớp";
                return View(model);
            }

            var result = await _userAPI.ChangePassword(
                accountId.Value,
                model.OldPassword,
                model.NewPassword
            );

            if (!result.success)
            {
                TempData["Error"] = result.message;
                return View(model);
            }

            TempData["Success"] = "Đổi mật khẩu thành công";

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> OrderDetail(int id)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerID");

            if (customerId == null)
                return RedirectToAction("Login", "Access");

            try
            {
                var data = await _userAPI.GetOrderDetailFull(id);

                return View(data);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}