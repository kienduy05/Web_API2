using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QuanLyBanSach.Models;
using QuanLyBanSach.Models.Authentication;
using QuanLyBanSach.Services;

namespace QuanLyBanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authentication]
    public class BookCategoryController : Controller
    {
        BookCategoryAPI bookCategoryAPI = new BookCategoryAPI();

        //Get
        
        [HttpPost]
        public async Task<IActionResult> Create(BookCategory model)
        {
            if (string.IsNullOrWhiteSpace(model.BookCategoryName))
            {
                TempData["Error"] = "Tên danh mục không được để trống";
                return RedirectToAction("Index");
            }

            var result = await bookCategoryAPI.Add(model);

            if (result)
                TempData["Success"] = "Thêm danh mục thành công";
            else
                TempData["Error"] = "Thêm thất bại";

            return RedirectToAction("Index");
        }

        //post
        [HttpPost]
        public async Task<IActionResult> Edit(BookCategory model)
        {
            if (string.IsNullOrWhiteSpace(model.BookCategoryName))
            {
                TempData["Error"] = "Tên danh mục không được để trống";
                return RedirectToAction("Index");
            }

            var result = await bookCategoryAPI.Update(model);

            if (result)
                TempData["Success"] = "Cập nhật thành công";
            else
                TempData["Error"] = "Cập nhật thất bại";

            return RedirectToAction("Index");
        }
        //delete
        public async Task<IActionResult> Delete(int id)
        {
            var result = await bookCategoryAPI.Delete(id);

            if (result)
                TempData["Success"] = "Xóa thành công";
            else
                TempData["Error"] = "Xóa thất bại";

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index(string keyword, int page = 1)
        {
            int pageSize = 8;

            var data = await bookCategoryAPI.GetAll();

            // Truyền toàn bộ tên danh mục cho dropdown gợi ý
            ViewBag.AllCategories = data.Select(x => x.BookCategoryName).ToList();

            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.Where(x => x.BookCategoryName
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
    }
}
