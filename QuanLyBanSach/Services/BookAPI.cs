using Newtonsoft.Json;
using QuanLyBanSach.Models;
using System.Text;

namespace QuanLyBanSach.Services
{
    public class BookMetaResponse
    {
        public List<Book> Books { get; set; }
        public List<BookCategory> Categories { get; set; }
        public List<Author> Authors { get; set; }
        public List<Publisher> Publishers { get; set; }
    }
    class CacheItem
    {
        public Book Data { get; set; }
        public DateTime ExpireTime { get; set; }
    }
    public class BookAPI
    {
        static Dictionary<int, CacheItem> cache = new();
        static TimeSpan cacheDuration = TimeSpan.FromMinutes(5);
        private static readonly HttpClient _httpClient = new HttpClient();
        private string baseUrl = "http://localhost:5000/book";

        // (trang chủ khách hàng)
        public async Task<List<Book>> GetAll()
        {
            var response = await _httpClient.GetAsync($"{baseUrl}/getall");
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API lỗi: {json}");
            }

            return JsonConvert.DeserializeObject<List<Book>>(json) ?? new List<Book>();
        }


        public async Task<BookMetaResponse> GetAllWithMeta()
        {
            var response = await _httpClient.GetAsync($"{baseUrl}/getall-with-meta");
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<BookMetaResponse>(json);
        }


        public async Task<Book> GetById(int id)
        {
            if (cache.ContainsKey(id))
            {
                var item = cache[id];

                // kiểm tra hết hạn
                if (item.ExpireTime > DateTime.Now)
                {
                    return item.Data; 
                }
                else
                {
                    cache.Remove(id); 
                }
            }
            var response = await _httpClient.GetAsync($"{baseUrl}/getbyid/{id}");
            var json = await response.Content.ReadAsStringAsync();
            var book = JsonConvert.DeserializeObject<Book>(json);

            cache[id] = new CacheItem
            {
                Data = book,
                ExpireTime = DateTime.Now.Add(cacheDuration)
            };

            return book;
        }

        // ADD
        public async Task<bool> Add(Book book)
        {
            var json = JsonConvert.SerializeObject(book);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{baseUrl}/add", content);
            return response.IsSuccessStatusCode;
            
        }

        // UPDATE
        public async Task<bool> Update(Book book)
        {
            var json = JsonConvert.SerializeObject(book);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{baseUrl}/update", content);
            if (response.IsSuccessStatusCode)
            {
                
                if (cache.ContainsKey(book.BookID))
                {
                    cache.Remove(book.BookID);
                }

                return true;
            }

            return false;
        }

        // DELETE
        public async Task<bool> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"{baseUrl}/delete/{id}");
            if (response.IsSuccessStatusCode)
            {
                if (cache.ContainsKey(id))
                {
                    cache.Remove(id);
                }

                return true;
            }

            return false;
        }

        // SEARCH
        public async Task<List<Book>> Search(string keyword)
        {
            var response = await _httpClient.GetAsync($"{baseUrl}/search?keyword={keyword}");
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Book>>(json);
        }
        // loc
        public async Task<List<Book>> Filter(int? categoryId, int? authorId,
                                     decimal? minPrice, decimal? maxPrice,
                                     string sort)
        {
            var query = $"{baseUrl.Replace("/book", "")}/book/filter?";

            if (categoryId.HasValue) query += $"categoryId={categoryId}&";
            if (authorId.HasValue) query += $"authorId={authorId}&";
            if (minPrice.HasValue) query += $"minPrice={minPrice}&";
            if (maxPrice.HasValue) query += $"maxPrice={maxPrice}&";
            if (!string.IsNullOrEmpty(sort)) query += $"sort={sort}";

            var response = await _httpClient.GetAsync(query);
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Book>>(json);
        }
        public async Task<List<Book>> GetRelated(int categoryId, int bookId)
        {
            var response = await _httpClient.GetAsync($"{baseUrl}/related/{categoryId}/{bookId}");
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Book>>(json);
        }
    }
} 