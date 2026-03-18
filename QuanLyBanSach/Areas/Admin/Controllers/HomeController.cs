using Microsoft.AspNetCore.Mvc;
using QuanLyBanSach.Models.Authentication;
namespace QuanLyBanSach.Areas.Admin.Controllers
    {
    [Area("Admin")]
    [Authentication]

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
