namespace QuanLyBanSach.Models.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<Book> MainBooks { get; set; } = new List<Book>();
        public Book FeaturedBook { get; set; } = new Book();
    }
}
