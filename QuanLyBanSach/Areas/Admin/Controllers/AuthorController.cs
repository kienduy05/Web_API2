using Microsoft.AspNetCore.Mvc;
using QuanLyBanSach.Models;
using QuanLyBanSach.Models.Authentication;
using QuanLyBanSach.Services;

namespace QuanLyBanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authentication]
    public class AuthorController : Controller
    {
        AuthorAPI authorAPI = new AuthorAPI();

        public async Task<IActionResult> Index(string keyword, int page = 1)
        {
            int pageSize = 8;

            var data = await authorAPI.GetAll();

            ViewBag.AllAuthors = data.Select(x => x.AuthorName).ToList();

            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.Where(x => x.AuthorName
                           .Contains(keyword, StringComparison.OrdinalIgnoreCase))
                           .ToList();
            }

            int totalItems = data.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var pagedData = data
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Keyword = keyword;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(pagedData);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Author model)
        {
            if (string.IsNullOrWhiteSpace(model.AuthorName))
            {
                TempData["Error"] = "Tên tác giả không được để trống";
                return RedirectToAction("Index");
            }

            var result = await authorAPI.Add(model);

            if (result)
                TempData["Success"] = "Thêm tác giả thành công";
            else
                TempData["Error"] = "Thêm thất bại";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Author model)
        {
            if (string.IsNullOrWhiteSpace(model.AuthorName))
            {
                TempData["Error"] = "Tên tác giả không được để trống";
                return RedirectToAction("Index");
            }

            var result = await authorAPI.Update(model);

            if (result)
                TempData["Success"] = "Cập nhật thành công";
            else
                TempData["Error"] = "Cập nhật thất bại";

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await authorAPI.Delete(id);

            if (result)
                TempData["Success"] = "Xóa thành công";
            else
                TempData["Error"] = "Xóa thất bại";

            return RedirectToAction("Index");
        }
    }
}