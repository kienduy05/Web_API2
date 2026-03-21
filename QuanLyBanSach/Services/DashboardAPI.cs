using Newtonsoft.Json;
using QuanLyBanSach.Models.ViewModels;

namespace QuanLyBanSach.Services
{
    public class DashboardAPI
    {
        private readonly HttpClient _httpClient;

        public DashboardAPI(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DashboardFullViewModel> GetDashboard()
        {
            var res = await _httpClient.GetAsync("http://localhost:5000/dashboard/full");

            var json = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
                throw new Exception(json);

            return JsonConvert.DeserializeObject<DashboardFullViewModel>(json);
        }
    }
}
