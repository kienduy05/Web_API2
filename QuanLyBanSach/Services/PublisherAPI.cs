using Newtonsoft.Json;
using QuanLyBanSach.Models;

namespace QuanLyBanSach.Services
{
    public class PublisherAPI
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private string baseUrl = "http://localhost:5000/publisher";
        public async Task<List<Publisher>> GetAll()
        {
            var reponsive = await _httpClient.GetAsync($"{baseUrl}/getall");
            var json = await reponsive.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Publisher>>(json);

        }
        public async Task<List<Publisher>> Search(string keyword, string type)
        {
            var response = await _httpClient.GetAsync($"{baseUrl}/search?keyword={keyword}&type={type}");
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Publisher>>(json);
        }
        public async Task<Publisher> GetById(int id)
        {
            var reponsive = await _httpClient.GetAsync($"{baseUrl}/getbyid/{id}");
            var json = await reponsive.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Publisher>(json);
        }
        public async Task<bool> Add(Publisher publisher)
        {
            var json = JsonConvert.SerializeObject(publisher);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var reponsive = await _httpClient.PostAsync($"{baseUrl}/add", content);
            return reponsive.IsSuccessStatusCode;
        }
        public async Task<bool> Update(Publisher publisher)
        {
            var json = JsonConvert.SerializeObject(publisher);
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
