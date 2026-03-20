using Newtonsoft.Json;
using QuanLyBanSach.Models;

namespace QuanLyBanSach.Services
{
    public class AuthorAPI
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private string baseUrl = "http://localhost:5000/author";
        public async Task<List<Author>> GetAll()
        {
            var reponsive = await _httpClient.GetAsync($"{baseUrl}/getall");
            var json = await reponsive.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Author>>(json);

        }
        public async Task<Author> GetById(int id)
        {
            var reponsive = await _httpClient.GetAsync($"{baseUrl}/getbyid/{id}");
            var json = await reponsive.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Author>(json);
        }
        public async Task<bool> Add(Author author)
        {
            var json = JsonConvert.SerializeObject(author);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var reponsive = await _httpClient.PostAsync($"{baseUrl}/add", content);
            return reponsive.IsSuccessStatusCode;
        }
        public async Task<bool> Update(Author author)
        {
            var json = JsonConvert.SerializeObject(author);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var reponsive = await _httpClient.PutAsync($"{baseUrl}/update", content);
            return reponsive.IsSuccessStatusCode;
        }
        public async Task<bool> Delete(int id)
        {
            var reponsive = await _httpClient.DeleteAsync($"{baseUrl}/delete/{id}");
            return reponsive.IsSuccessStatusCode;
        }
    }
}
