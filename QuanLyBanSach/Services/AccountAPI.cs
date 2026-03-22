using Newtonsoft.Json;
using QuanLyBanSach.Models;
using QuanLyBanSach.Models.ViewModels;
using System.Text;

namespace QuanLyBanSach.Services
{
    public class AccountAPI
    {
        private static readonly HttpClient _httpClient = new HttpClient();
       
        private string baseUrl = "http://localhost:5000/account";

       

        // LOGIN
        public async Task<Account> Login(string username, string password)
        {
            var data = new
            {
                Username = username,
                Password = password
            };

            var json = JsonConvert.SerializeObject(data);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{baseUrl}/login", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var result = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Account>(result);
        }

        // GET ALL
        public async Task<List<Account>> GetAll()
        {
            var response = await _httpClient.GetAsync($"{baseUrl}/getall");

            var json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<Account>>(json);
        }

        // ADD
        public async Task<bool> Add(Account account)
        {
            var json = JsonConvert.SerializeObject(account);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{baseUrl}/add", content);

            return response.IsSuccessStatusCode;
        }

        // UPDATE
        public async Task<bool> Update(Account account)
        {
            var json = JsonConvert.SerializeObject(account);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{baseUrl}/update", content);

            return response.IsSuccessStatusCode;
        }

        // DELETE
        public async Task<(bool success, string message)> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"{baseUrl}/delete/{id}");

            var json = await response.Content.ReadAsStringAsync();

            dynamic data = JsonConvert.DeserializeObject(json);

            return (response.IsSuccessStatusCode, data.mess.ToString());
        }

        public async Task<bool> UsernameExists(string username)
        {
            var res = await _httpClient.GetAsync($"{baseUrl}/check-username/{username}");
            var json = await res.Content.ReadAsStringAsync();

            dynamic data = JsonConvert.DeserializeObject(json);

            return data.exists;
        }
        public async Task<List<AccountViewModel>> GetAllWithCustomer()
        {
            var response = await _httpClient.GetAsync($"{baseUrl}/getall-with-customer");

            var json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<AccountViewModel>>(json);
        }
        public async Task<List<Customer>> GetCustomerNoAccount()
        {
            var response = await _httpClient.GetAsync($"{baseUrl}/get-customer-no-account");

            var json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<Customer>>(json);
        }
        public async Task<(bool success, string message)> ResetPassword(int id)
        {
            var response = await _httpClient.PutAsync($"{baseUrl}/reset-password/{id}", null);

            var json = await response.Content.ReadAsStringAsync();

            dynamic data = JsonConvert.DeserializeObject(json);

            return (response.IsSuccessStatusCode, data.mess.ToString());
        }

        public async Task<(bool success, string message)> Register(RegisterViewModel model)
        {
            var json = JsonConvert.SerializeObject(model);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{baseUrl}/register", content);

            var result = await response.Content.ReadAsStringAsync();

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

            var res = await _httpClient.PutAsync($"{baseUrl}/change-password/{accountId}", content);

            var result = await res.Content.ReadAsStringAsync();

            dynamic obj = JsonConvert.DeserializeObject(result);

            return (obj.success, obj.mess.ToString());
        }
    }
}