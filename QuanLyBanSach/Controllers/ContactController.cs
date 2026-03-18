using Microsoft.AspNetCore.Mvc;

namespace QuanLyBanSach.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
