namespace QuanLyBanSach.Models
{
    public class Book
    {
        public int BookID { get; set; }
        public string BookName { get; set; }
        public string BookDescription { get; set; }
        public int BookCategoryID { get; set; }
        public int BookAuthorID { get; set; }
        public int BookPublisherID { get; set; }
        public int BookQuantity { get; set; }
        public decimal BookPrice { get; set; }
        public int BookStatus { get; set; }
        public string BookImage { get; set; }
        public string BookCategoryName { get; set; }
        public string AuthorName { get; set; }
        public string PublisherName { get; set; }
    }
}
