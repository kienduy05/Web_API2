using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QuanLyBanSach.Models;
using QuanLyBanSach.Models.Authentication;
using QuanLyBanSach.Services;

namespace QuanLyBanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authentication]
    public class CustomerController : Controller
    {
        CustomerAPI customerAPI = new CustomerAPI();
        public override void OnActionExecuting(ActionExecutingContext context)
        {

            var role = HttpContext.Session.GetString("AccountType");

            // Chỉ admin được truy cập
            if (role != "0")
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này!";
                context.Result = RedirectToAction("Index", "Home");
                return;
            }

            base.OnActionExecuting(context);
        }

        public async Task<IActionResult> Index(string keyword, string type, int page = 1)
        {
            int pageSize = 8;

            List<Customer> data;

            if (!string.IsNullOrEmpty(keyword) && !string.IsNullOrEmpty(type))
            {
                data = await customerAPI.Search(keyword, type);
            }
            else
            {
                data = await customerAPI.GetAll();
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
        // GET: Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create
        [HttpPost]
        public async Task<IActionResult> Create(Customer model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Check Phone
            if (await customerAPI.PhoneExists(model.CustomerPhone))
            {
                TempData["Error"] = "Số điện thoại đã tồn tại";
                return View(model);
            }

            // Check Email
            if (await customerAPI.EmailExists(model.CustomerEmail))
            {
                TempData["Error"] = "Email đã tồn tại";
                return View(model);
            }

            var result = await customerAPI.Add(model);

            if (result)
            {
                TempData["Success"] = "Thêm khách hàng thành công";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Thêm thất bại";
            return View(model);
        }


        // GET: Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var customer = await customerAPI.GetById(id);

            if (customer == null)
                return RedirectToAction("Index");

            return View(customer);
        }

        // POST: Edit
        [HttpPost]
        public async Task<IActionResult> Edit(Customer model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await customerAPI.Update(model);

            if (result)
            {
                TempData["Success"] = "Cập nhật thành công";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Cập nhật thất bại";
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await customerAPI.Delete(id);

            if (result)
                TempData["Success"] = "Xóa thành công";
            else
                TempData["Error"] = "Xóa thất bại";

            return RedirectToAction("Index");
        }

    }
}