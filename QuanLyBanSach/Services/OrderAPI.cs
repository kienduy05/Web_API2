using Newtonsoft.Json;
using QuanLyBanSach.Models;
using System.Text;

namespace QuanLyBanSach.Services
{
    public class OrderAPI
    {
        private static readonly HttpClient _httpClient = new HttpClient() 
        { 
            Timeout = TimeSpan.FromSeconds(30) // Set timeout khi khởi tạo
        };

        private string baseUrl = "http://localhost:5000/orders";

        // GET ALL
        public async Task<List<Order>> GetAll()
        {
            var response = await _httpClient.GetAsync($"{baseUrl}/getall");
            if (!response.IsSuccessStatusCode) return new List<Order>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Order>>(json) ?? new List<Order>();
        }

        // GET BY ID
        public async Task<Order> GetById(int id)
        {
            var response = await _httpClient.GetAsync($"{baseUrl}/getbyid/{id}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<Order>>(json);
            return list?.FirstOrDefault();
        }

        // GET ORDER DETAILS
        public async Task<List<OrderDetail>> GetOrderDetails(int orderId)
        {
            var response = await _httpClient.GetAsync($"http://localhost:5000/orderdetail/getbyorderid/{orderId}");
            if (!response.IsSuccessStatusCode) return new List<OrderDetail>();

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(json)) return new List<OrderDetail>();

            return JsonConvert.DeserializeObject<List<OrderDetail>>(json) ?? new List<OrderDetail>();
        }

        // ADD (sử dụng object bất kỳ)
        public async Task<bool> Add(object orderData)
        {
            var options = new System.Text.Json.JsonSerializerOptions { PropertyNamingPolicy = null };
            var json = System.Text.Json.JsonSerializer.Serialize(orderData, options);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{baseUrl}/add", content);
            return response.IsSuccessStatusCode;
        }

        // UPDATE
        public async Task<bool> Update(Order order)
        {
            var json = JsonConvert.SerializeObject(order);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{baseUrl}/update", content);

            return response.IsSuccessStatusCode;
        }

        // UPDATE STATUS
        public async Task<bool> UpdateStatus(int orderId, int status)
        {
            var json = JsonConvert.SerializeObject(new { orderID = orderId, orderStatus = status });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{baseUrl}/updatestatus", content);

            return response.IsSuccessStatusCode;
        }

        // DELETE
        public async Task<bool> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"{baseUrl}/delete/{id}");

            return response.IsSuccessStatusCode;
        }

        // SEARCH
        public async Task<List<Order>> Search(string keyword, string type)
        {
            var response = await _httpClient.GetAsync($"{baseUrl}/search?keyword={keyword}&type={type}");
            if (!response.IsSuccessStatusCode) return new List<Order>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Order>>(json) ?? new List<Order>();
        }

        // FILTER BY STATUS
        public async Task<List<Order>> FilterByStatus(int status)
        {
            var response = await _httpClient.GetAsync($"{baseUrl}/filterbystatus/{status}");
            if (!response.IsSuccessStatusCode) return new List<Order>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Order>>(json) ?? new List<Order>();
        }

        // SEARCH BY MONTH/YEAR
        public async Task<List<Order>> SearchByMonthYear(int month, int year)
        {
            var response = await _httpClient.GetAsync($"{baseUrl}/searchbymonthyear?month={month}&year={year}");
            if (!response.IsSuccessStatusCode) return new List<Order>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Order>>(json) ?? new List<Order>();
        }
    }
}
