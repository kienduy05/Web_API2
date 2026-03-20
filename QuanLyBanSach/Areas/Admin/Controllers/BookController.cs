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
        BookCategoryAPI bookCategoryAPI = new BookCategoryAPI();
        AuthorAPI authorAPI = new AuthorAPI();
        PublisherAPI publisherAPI = new PublisherAPI();

        // Load 3 dropdown vào ViewBag
        private async Task LoadDropdowns()
        {
            ViewBag.Categories = await bookCategoryAPI.GetAll();
            ViewBag.Authors = await authorAPI.GetAll();
            ViewBag.Publishers = await publisherAPI.GetAll();
        }

        // GET: Index
        public async Task<IActionResult> Index(string keyword, int page = 1)
        {
            int pageSize = 8;

            List<Book> data;

            if (!string.IsNullOrEmpty(keyword))
            {
                data = await bookAPI.Search(keyword);
            }
            else
            {
                data = await bookAPI.GetAll();
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

            await LoadDropdowns();

            return View(pagedData);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await bookAPI.GetById(id);
            if (book == null)
                return RedirectToAction("Index");

            await LoadDropdowns();
            return View(book);
        }
        // POST: Create
        [HttpPost]
        public async Task<IActionResult> Create(Book model, IFormFile? imageFile)
        {
            // Xử lý upload ảnh
            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Path.GetFileName(imageFile.FileName);
                var savePath = Path.Combine(Directory.GetCurrentDirectory(),
                                            "wwwroot", "images", "books", fileName);

                // Tạo thư mục nếu chưa có
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

        // POST: Edit
        [HttpPost]
        public async Task<IActionResult> Edit(Book model, IFormFile? imageFile)
        {
            // Nếu có upload ảnh mới
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

        // GET: Delete
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