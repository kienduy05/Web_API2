using Microsoft.AspNetCore.Mvc;
using QuanLyBanSach.Models;
using QuanLyBanSach.Models.ViewModels;
using QuanLyBanSach.Services;
using System.Security.Principal;

namespace QuanLyBanSach.Controllers
{
    public class AccessController : Controller
    {
        AccountAPI accountAPI = new AccountAPI();

        // GET LOGIN
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("Username") != null)
            {
                var role = HttpContext.Session.GetString("AccountType");

                if (role == "0" || role == "1")
                    return RedirectToAction("Index", "Home", new { area = "Admin" });

                return RedirectToAction("Index", "Home");
            }

            // 🔥 FIX: KHỞI TẠO VIEWMODEL
            var vm = new AuthViewModel
            {
                Login = new Account(),
                Register = new RegisterViewModel()
            };

            return View(vm);
        }

        // POST LOGIN
        [HttpPost]
        public async Task<IActionResult> Login(AuthViewModel vm)
        {
            if (HttpContext.Session.GetString("Username") != null)
                return RedirectToAction("Index", "Home");

            var acc = vm.Login;

            var user = await accountAPI.Login(acc.Username, acc.Password);

            if (user == null)
            {
                TempData["Error"] = "Sai tài khoản hoặc mật khẩu";
                return View("Login", vm);
            }

            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("AccountType", user.AccountType.ToString());
            HttpContext.Session.SetString("AccountDisplayName", user.AccountDisplayName ?? "");
            HttpContext.Session.SetInt32("AccountID", user.AccountID);

            if (user.CustomerID != null)
            {
                HttpContext.Session.SetInt32("CustomerID", user.CustomerID.Value);

                CustomerAPI customerAPI = new CustomerAPI();
                var customer = await customerAPI.GetById(user.CustomerID.Value);

                HttpContext.Session.SetString("CustomerName", customer.CustomerName);
            }

            if (user.AccountType == 0 || user.AccountType == 1)
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            return RedirectToAction("Index", "Home");
        }
        //Register
        [HttpPost]
        public async Task<IActionResult> Register(AuthViewModel vm)
        {
            var model = vm.Register;

            // 🔥 FIX: BỎ VALIDATE LOGIN
            ModelState.Remove("Login.Username");
            ModelState.Remove("Login.Password");
            ModelState.Remove("Login.AccountDisplayName");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng nhập đúng thông tin";
                return View("Login", vm);
            }

            var result = await accountAPI.Register(model);

            if (!result.success)
            {
                TempData["Error"] = result.message;
                return View("Login", vm);
            }

            TempData["Success"] = result.message;

            return RedirectToAction("Login");
        }

        // LOGOUT
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }
    }
}