using Microsoft.AspNetCore.Mvc;
using QuanLyBanSach.Models.ViewModels;
using QuanLyBanSach.Services;

[Area("Admin")]
public class ChangePasswordController : Controller
{
    private AccountAPI accountAPI = new AccountAPI();

    // GET
    public IActionResult Index()
    {
        return View(new AdminChangePasswordVM());
    }

    // POST
    [HttpPost]
    public async Task<IActionResult> Index(AdminChangePasswordVM model)
    {
        var accountId = HttpContext.Session.GetInt32("AccountID");

        if (accountId == null)
            return RedirectToAction("Login", "Access");

        // ✅ validate model
        if (!ModelState.IsValid)
            return View(model);

        if (model.NewPassword != model.ConfirmPassword)
        {
            TempData["Error"] = "Mật khẩu xác nhận không khớp";
            return View(model);
        }

        var result = await accountAPI.ChangePassword(
            accountId.Value,
            model.OldPassword,
            model.NewPassword
        );

        if (!result.success)
        {
            TempData["Error"] = result.message;
            return View(model);
        }

        TempData["Success"] = result.message;

        return RedirectToAction("Index");
    }
}