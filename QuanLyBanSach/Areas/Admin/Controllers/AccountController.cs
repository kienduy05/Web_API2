using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QuanLyBanSach.Models.Authentication;
using QuanLyBanSach.Services;
using QuanLyBanSach.Models;
using QuanLyBanSach.Models.ViewModels;

namespace QuanLyBanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authentication]
    public class AccountController : Controller
    {
        AccountAPI accountAPI = new AccountAPI();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = HttpContext.Session.GetString("AccountType");

            if (role != "0")
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này!";
                context.Result = RedirectToAction("Index", "Home");
                return;
            }

            base.OnActionExecuting(context);
        }

        // ================= INDEX =================
        public async Task<IActionResult> Index(int page = 1, int? type = null)
        {
            int pageSize = 8;

            var data = await accountAPI.GetAllWithCustomer();

            // 🔥 filter theo type
            if (type != null)
            {
                data = data.Where(x => x.AccountType == type).ToList();
            }

            int totalItems = data.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var pagedData = data
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Type = type;

            return View(pagedData);
        }

        // ================= CREATE =================
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Customers = await accountAPI.GetCustomerNoAccount();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Account model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Customers = await accountAPI.GetCustomerNoAccount();
                return View(model);
            }

            // Check username
            if (await accountAPI.UsernameExists(model.Username))
            {
                TempData["Error"] = "Username đã tồn tại";
                ViewBag.Customers = await accountAPI.GetCustomerNoAccount();
                return View(model);
            }

            var result = await accountAPI.Add(model);

            if (result)
            {
                TempData["Success"] = "Thêm tài khoản thành công";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Thêm thất bại";
            return View(model);
        }

        // ================= DELETE =================
        public async Task<IActionResult> Delete(int id)
        {
            var result = await accountAPI.Delete(id);

            if (result.success)
                TempData["Success"] = result.message;
            else
                TempData["Error"] = result.message;

            return RedirectToAction("Index");
        }

        // ================= RESET PASSWORD =================

        //public IActionResult ResetPassword(int customerId)
        //{
        //    return Content($"Reset password for customer {customerId}");
        //}
        public async Task<IActionResult> ResetPassword(int id)
        {
            var result = await accountAPI.ResetPassword(id);

            if (result.success)
                TempData["Success"] = result.message;
            else
                TempData["Error"] = result.message;

            return RedirectToAction("Index");
        }


    }
}