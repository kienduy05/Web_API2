using Microsoft.AspNetCore.Mvc;
using QuanLyBanSach.Models;
using QuanLyBanSach.Models.ViewModels;
using QuanLyBanSach.Services;
using System.Diagnostics;

namespace QuanLyBanSach.Controllers
{
    public class HomeController : Controller
    {
        BookAPI _bookAPI = new BookAPI();

        

        public async Task<IActionResult> Index()
        {
            HomeIndexViewModel viewModel = new HomeIndexViewModel
            {
                MainBooks = await _bookAPI.GetAll(),
                FeaturedBook = await _bookAPI.GetById(1)
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
