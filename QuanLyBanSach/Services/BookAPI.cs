using Newtonsoft.Json;
using QuanLyBanSach.Models;
using System.Text;

namespace QuanLyBanSach.Services
{
    public class BookAPI
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private string baseUrl = "http://localhost:5000/book";

        // GET ALL (có JOIN tên danh mục, tác giả, NXB)
        public async Task<List<Book>> GetAll()
        {
            var response = await _httpClient.GetAsync($"{baseUrl}/getall");
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Book>>(json);
        }

        // GET BY ID
        public async Task<Book> GetById(int id)
        {
            var response = await _httpClient.GetAsync($"{baseUrl}/getbyid/{id}");
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Book>(json);
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
            return response.IsSuccessStatusCode;
        }

        // DELETE
        public async Task<bool> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"{baseUrl}/delete/{id}");
            return response.IsSuccessStatusCode;
        }

        // SEARCH
        public async Task<List<Book>> Search(string keyword)
        {
            var response = await _httpClient.GetAsync($"{baseUrl}/search?keyword={keyword}");
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Book>>(json);
        }
    }
}