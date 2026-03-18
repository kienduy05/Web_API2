using Microsoft.AspNetCore.Mvc;

namespace QuanLyBanSach.Controllers
{
    public class InfoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
