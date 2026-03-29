using Microsoft.AspNetCore.Mvc;
using QuanLyBanSach.Services;
using QuanLyBanSach.Models;

namespace QuanLyBanSach.Controllers
{
    public class BookController : Controller
    {
        BookAPI _bookAPI = new BookAPI();
        BookCategoryAPI _categoryAPI = new BookCategoryAPI();
        AuthorAPI _authorAPI = new AuthorAPI();

        public async Task<IActionResult> Index(string keyword, int? categoryId, int? authorId,
                                               decimal? minPrice, decimal? maxPrice,
                                               string sort, int page = 1)
        {
            int pageSize = 9;
            var categoryTask = _categoryAPI.GetAll();
            var authorTask = _authorAPI.GetAll();
            List<Book> data;

            if (!string.IsNullOrEmpty(keyword))
            {
 
                data = await _bookAPI.Search(keyword);
            }
            else
            {

                data = await _bookAPI.Filter(categoryId, authorId, minPrice, maxPrice, sort);
            }

            await Task.WhenAll(categoryTask, authorTask);

           

            int totalItems = data.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var pagedData = data
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Categories = categoryTask.Result;
            ViewBag.Authors = authorTask.Result;
            ViewBag.CategoryId = categoryId;
            ViewBag.AuthorId = authorId;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.Sort = sort;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View(pagedData);
        }
        public async Task<IActionResult> Detail(int id)
        {
            var book = await _bookAPI.GetById(id);

            if (book == null)
                return NotFound();
            var relatedBooks = await _bookAPI.GetRelated(book.BookCategoryID, book.BookID);
            ViewBag.RelatedBooks = relatedBooks;
            return View(book);
        }
    }
}