using Newtonsoft.Json;
using QuanLyBanSach.Models;

namespace QuanLyBanSach.Services
{
    public class BookCategoryAPI
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private string baseUrl = "http://localhost:5000/bookcategory";

        //get all

        public async Task<List<BookCategory>> GetAll()
        {
            var reponsive = await _httpClient.GetAsync($"{baseUrl}/getall");
            var json = await reponsive.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<BookCategory>>(json);

        }

        //getby id
        public async Task<BookCategory> GetById(int id)
        {
            var reponsive = await _httpClient.GetAsync($"{baseUrl}/getbyid/{id}");
            var json = await reponsive.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<BookCategory>(json);
        }

        //add
        public async Task<bool> Add(BookCategory bookCategory)
        {
            var json = JsonConvert.SerializeObject(bookCategory);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var reponsive = await _httpClient.PostAsync($"{baseUrl}/add", content);
            return reponsive.IsSuccessStatusCode;
        }
        //update 
        public async Task<bool> Update(BookCategory bookCategory)
        {
            var json = JsonConvert.SerializeObject(bookCategory);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var reponsive = await _httpClient.PutAsync($"{baseUrl}/update", content);
            return reponsive.IsSuccessStatusCode;
        }
        //delete
        public async Task<bool> Delete(int id)
        {
            var reponsive = await _httpClient.DeleteAsync($"{baseUrl}/delete/{id}");
            return reponsive.IsSuccessStatusCode;
        }
    }
}
