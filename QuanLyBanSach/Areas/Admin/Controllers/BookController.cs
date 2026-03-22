using Microsoft.AspNetCore.Mvc;
using QuanLyBanSach.Models;
using QuanLyBanSach.Models.Authentication;
using QuanLyBanSach.Services;

namespace QuanLyBanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authentication]
    public class BookController : Controller
    {
        BookAPI bookAPI = new BookAPI();

        public async Task<IActionResult> Index(string keyword, int page = 1)
        {
            int pageSize = 8;

            var meta = await bookAPI.GetAllWithMeta();

            var data = string.IsNullOrEmpty(keyword)
                       ? meta.Books
                       : meta.Books.Where(x => x.BookName
                                 .Contains(keyword, StringComparison.OrdinalIgnoreCase))
                                 .ToList();

            int totalItems = data.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var pagedData = data
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Keyword = keyword;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Categories = meta.Categories;
            ViewBag.Authors = meta.Authors;
            ViewBag.Publishers = meta.Publishers;

            return View(pagedData);
        }

        // GET: Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var meta = await bookAPI.GetAllWithMeta();
            ViewBag.Categories = meta.Categories;
            ViewBag.Authors = meta.Authors;
            ViewBag.Publishers = meta.Publishers;
            return View();
        }

        // POST: Create
        [HttpPost]
        public async Task<IActionResult> Create(Book model, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Path.GetFileName(imageFile.FileName);
                var savePath = Path.Combine(Directory.GetCurrentDirectory(),
                                            "wwwroot", "images", "books", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(savePath));

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                model.BookImage = fileName;
            }

            var result = await bookAPI.Add(model);

            if (result)
                TempData["Success"] = "Thêm sách thành công";
            else
                TempData["Error"] = "Thêm thất bại";

            return RedirectToAction("Index");
        }

        // GET: Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Gọi song song GetById và GetAllWithMeta
            var bookTask = bookAPI.GetById(id);
            var metaTask = bookAPI.GetAllWithMeta();

            await Task.WhenAll(bookTask, metaTask);

            if (bookTask.Result == null)
                return RedirectToAction("Index");

            ViewBag.Categories = metaTask.Result.Categories;
            ViewBag.Authors = metaTask.Result.Authors;
            ViewBag.Publishers = metaTask.Result.Publishers;

            return View(bookTask.Result);
        }

        // POST: Edit
        [HttpPost]
        public async Task<IActionResult> Edit(Book model, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Path.GetFileName(imageFile.FileName);
                var savePath = Path.Combine(Directory.GetCurrentDirectory(),
                                            "wwwroot", "images", "books", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(savePath));

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                model.BookImage = fileName;
            }

            var result = await bookAPI.Update(model);

            if (result)
                TempData["Success"] = "Cập nhật thành công";
            else
                TempData["Error"] = "Cập nhật thất bại";

            return RedirectToAction("Index");
        }
        // DELETE
        public async Task<IActionResult> Delete(int id)
        {
            var result = await bookAPI.Delete(id);

            if (result)
                TempData["Success"] = "Xóa thành công";
            else
                TempData["Error"] = "Xóa thất bại";

            return RedirectToAction("Index");
        }
    }
}