using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QuanLyBanSach.Models;
using QuanLyBanSach.Models.Authentication;
using QuanLyBanSach.Services;

namespace QuanLyBanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Orders")]
    [Authentication]
    public class OrderController : Controller
    {
        OrderAPI orderAPI = new OrderAPI();

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

        public async Task<IActionResult> Index(string keyword, string type, int? status, int page = 1)
        {
            int pageSize = 6;

            List<Order> data = new List<Order>();

            try
            {
                // Filter by status
                if (status.HasValue)
                {
                    data = await orderAPI.FilterByStatus(status.Value);
                }
                // Search by keyword and type
                else if (!string.IsNullOrEmpty(keyword) && !string.IsNullOrEmpty(type))
                {
                    data = await orderAPI.Search(keyword, type);
                }
                // Get all orders
                else
                {
                    data = await orderAPI.GetAll();
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải dữ liệu: {ex.Message}";
                data = new List<Order>();
            }

            int totalItems = data.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var pagedData = data
                .OrderByDescending(x => x.OrderCreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Keyword = keyword;
            ViewBag.Type = type;
            ViewBag.Status = status;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(pagedData);
        }

        // GET: Details
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var order = await orderAPI.GetById(id);

            if (order == null)
                return RedirectToAction("Index");

            var orderDetails = await orderAPI.GetOrderDetails(id);

            ViewBag.OrderDetails = orderDetails;
            ViewBag.StatusText = GetStatusText(order.OrderStatus);

            return View(order);
        }

        // GET: Edit Status
        [HttpGet("EditStatus/{id}")]
        public async Task<IActionResult> EditStatus(int id)
        {
            try
            {
                var order = await orderAPI.GetById(id);

                if (order == null)
                    return RedirectToAction("Index");

                return View(order);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải đơn hàng: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // POST: Edit Status
        [HttpPost("EditStatus/{id}")]
        public async Task<IActionResult> EditStatus(int id, int newStatus)
        {
            if (newStatus < 0 || newStatus > 2)
            {
                TempData["Error"] = "Trạng thái không hợp lệ";
                return RedirectToAction("Details", new { id });
            }

            var result = await orderAPI.UpdateStatus(id, newStatus);

            if (result)
            {
                TempData["Success"] = "Cập nhật trạng thái đơn hàng thành công";
                return RedirectToAction("Details", new { id });
            }

            TempData["Error"] = "Cập nhật trạng thái thất bại";
            return RedirectToAction("Details", new { id });
        }

        private string GetStatusText(int status)
        {
            return status switch
            {
                0 => "Đang chuẩn bị",
                1 => "Đang giao",
                2 => "Đã giao",
                _ => "Không xác định"
            };
        }
    }
}
