using Microsoft.AspNetCore.Mvc;
using QuanLyBanSach.Models.Authentication;
using QuanLyBanSach.Services;
namespace QuanLyBanSach.Areas.Admin.Controllers
    {
    [Area("Admin")]
    [Authentication]
    public class HomeController : Controller
    {
        private readonly DashboardAPI _dashboardAPI;

        public HomeController(DashboardAPI dashboardAPI)
        {
            _dashboardAPI = dashboardAPI;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _dashboardAPI.GetDashboard();
            return View(data);
        }
    }
}
