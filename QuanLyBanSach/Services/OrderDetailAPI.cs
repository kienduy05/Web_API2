using Newtonsoft.Json;
using QuanLyBanSach.Models;
using System.Text;

namespace QuanLyBanSach.Services
{
    public class OrderDetailAPI
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        private string baseUrl = "http://localhost:5000/orderdetail";

        // GET ALL
        public async Task<List<OrderDetail>> GetAll()
        {
            var response = await _httpClient.GetAsync($"{baseUrl}/getall");
            if (!response.IsSuccessStatusCode) return new List<OrderDetail>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<OrderDetail>>(json) ?? new List<OrderDetail>();
        }

        // GET BY ID
        public async Task<OrderDetail> GetById(int id)
        {
            var response = await _httpClient.GetAsync($"{baseUrl}/getbyid/{id}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<OrderDetail>(json);
        }

        // GET BY ORDER ID
        public async Task<List<OrderDetail>> GetByOrderId(int orderId)
        {
            var response = await _httpClient.GetAsync($"{baseUrl}/getbyorderid/{orderId}");
            if (!response.IsSuccessStatusCode) return new List<OrderDetail>();

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(json)) return new List<OrderDetail>();

            return JsonConvert.DeserializeObject<List<OrderDetail>>(json) ?? new List<OrderDetail>();
        }

        // ADD
        public async Task<bool> Add(OrderDetail orderDetail)
        {
            var json = JsonConvert.SerializeObject(orderDetail);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{baseUrl}/add", content);

            return response.IsSuccessStatusCode;
        }

        // UPDATE
        public async Task<bool> Update(OrderDetail orderDetail)
        {
            var json = JsonConvert.SerializeObject(orderDetail);
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
    }
}
