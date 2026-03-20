using Microsoft.AspNetCore.Mvc;
using QuanLyBanSach.Models;
using QuanLyBanSach.Models.Authentication;
using QuanLyBanSach.Services;

namespace QuanLyBanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authentication]
    public class PublisherController : Controller
    {
        PublisherAPI publisherAPI = new PublisherAPI();

        public async Task<IActionResult> Index(string keyword, string type, int page = 1)
        {
            int pageSize = 8;

            List<Publisher> data;

            if (!string.IsNullOrEmpty(keyword) && !string.IsNullOrEmpty(type))
            {
                data = await publisherAPI.Search(keyword, type);
            }
            else
            {
                data = await publisherAPI.GetAll();
            }

            int totalItems = data.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var pagedData = data
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Keyword = keyword;
            ViewBag.Type = type;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(pagedData);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Publisher model)
        {
            if (string.IsNullOrWhiteSpace(model.PublisherName))
            {
                TempData["Error"] = "Tên nhà xuất bản không được để trống";
                return RedirectToAction("Index");
            }
            if (!string.IsNullOrEmpty(model.PublisherPhone) && model.PublisherPhone.Length != 10)
            {
                TempData["Error"] = "Số điện thoại phải đúng 10 chữ số";
                return RedirectToAction("Index");
            }
            var result = await publisherAPI.Add(model);

            if (result)
                TempData["Success"] = "Thêm nhà xuất bản thành công";
            else
                TempData["Error"] = "Thêm thất bại";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Publisher model)
        {
            if (string.IsNullOrWhiteSpace(model.PublisherName))
            {
                TempData["Error"] = "Tên nhà xuất bản không được để trống";
                return RedirectToAction("Index");
            }

            var result = await publisherAPI.Update(model);

            if (result)
                TempData["Success"] = "Cập nhật thành công";
            else
                TempData["Error"] = "Cập nhật thất bại";

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await publisherAPI.Delete(id);

            if (result)
                TempData["Success"] = "Xóa thành công";
            else
                TempData["Error"] = "Xóa thất bại";

            return RedirectToAction("Index");
        }
    }
}