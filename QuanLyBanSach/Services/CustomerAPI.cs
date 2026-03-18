using Newtonsoft.Json;
using QuanLyBanSach.Models;
using System.Text;

namespace QuanLyBanSach.Services
{
    public class CustomerAPI
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        private string baseUrl = "http://localhost:5000/customer";

        

        // GET ALL
        public async Task<List<Customer>> GetAll()
        {
            var response = await _httpClient.GetAsync($"{baseUrl}/getall");

            var json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<Customer>>(json);
        }

        // GET BY ID
        public async Task<Customer> GetById(int id)
        {
            var response = await _httpClient.GetAsync($"{baseUrl}/getbyid/{id}");

            var json = await response.Content.ReadAsStringAsync();

            var list = JsonConvert.DeserializeObject<List<Customer>>(json);

            return list.FirstOrDefault();
        }

        // ADD
        public async Task<bool> Add(Customer customer)
        {
            var json = JsonConvert.SerializeObject(customer);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{baseUrl}/add", content);

            return response.IsSuccessStatusCode;
        }

        // UPDATE
        public async Task<bool> Update(Customer customer)
        {
            var json = JsonConvert.SerializeObject(customer);

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


        public async Task<bool> PhoneExists(string phone)
        {
            var res = await _httpClient.GetAsync($"{baseUrl}/check-phone/{phone}");
            var json = await res.Content.ReadAsStringAsync();

            dynamic data = JsonConvert.DeserializeObject(json);

            return data.exists;
        }
        public async Task<bool> EmailExists(string email)
        {
            var res = await _httpClient.GetAsync($"{baseUrl}/check-email/{email}");
            var json = await res.Content.ReadAsStringAsync();

            dynamic data = JsonConvert.DeserializeObject(json);

            return data.exists;
        }

        public async Task<List<Customer>> Search(string keyword, string type)
        {
            var response = await _httpClient.GetAsync($"{baseUrl}/search?keyword={keyword}&type={type}");

            var json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<Customer>>(json);
        }
    }
}