using Newtonsoft.Json;
using QuanLyBanSach.Models;
using QuanLyBanSach.Models.ViewModels;
using System.Text;

namespace QuanLyBanSach.Services
{
    public class UserAPI
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<UserDashboardVM> GetDashboard(int customerId)
        {
            var res = await _httpClient.GetAsync($"http://localhost:5000/user/dashboard/{customerId}");

            var json = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
                throw new Exception(json);

            return JsonConvert.DeserializeObject<UserDashboardVM>(json);
        }

        public async Task<(bool success, string message)> UpdateProfile(int customerId, Customer model)
        {
            var json = JsonConvert.SerializeObject(model);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = await _httpClient.PutAsync($"http://localhost:5000/user/update/{customerId}", content);

            var result = await res.Content.ReadAsStringAsync();

            dynamic data = JsonConvert.DeserializeObject(result);

            return (data.success, data.mess.ToString());
        }

        public async Task<(bool success, string message)> ChangePassword(int accountId, string oldPass, string newPass)
        {
            var data = new
            {
                OldPassword = oldPass,
                NewPassword = newPass
            };

            var json = JsonConvert.SerializeObject(data);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = await _httpClient.PutAsync($"http://localhost:5000/user/change-password/{accountId}", content);

            var result = await res.Content.ReadAsStringAsync();

            dynamic obj = JsonConvert.DeserializeObject(result);

            return (obj.success, obj.mess.ToString());
        }

        public async Task<List<OrderDetail>> GetOrderDetails(int orderId)
        {
            var res = await _httpClient.GetAsync($"http://localhost:5000/orderdetail/getbyorderid/{orderId}");

            if (!res.IsSuccessStatusCode)
                return new List<OrderDetail>();

            var json = await res.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<OrderDetail>>(json) ?? new List<OrderDetail>();
        }

        public async Task<OrderDetailFullVM> GetOrderDetailFull(int orderId)
        {
            var res = await _httpClient.GetAsync($"http://localhost:5000/user/order-detail/{orderId}");

            var json = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
                throw new Exception(json);

            dynamic data = JsonConvert.DeserializeObject(json);

            if (data.success != true)
                throw new Exception(data.mess.ToString());

            var vm = new OrderDetailFullVM
            {
                Order = JsonConvert.DeserializeObject<Order>(data.order.ToString()),
                Details = JsonConvert.DeserializeObject<List<OrderDetail>>(data.details.ToString())
            };

            return vm;
        }
    }
}