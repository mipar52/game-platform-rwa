using System.Net.Http.Headers;
using System.Text.Json;
using WebApp.Models;
using WebApp.Utilities;
using WebApp.ViewModels;

namespace WebApp.Services
{
    public class ApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _baseUrl = "http://localhost:5062/api/";
        public ApiService(IHttpClientFactory factory, IHttpContextAccessor accessor)
        {
            _httpClientFactory = factory;
            _httpContextAccessor = accessor;
        }

        private HttpClient CreateClient()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var token = _httpContextAccessor.HttpContext.Session.GetString("jwt_token");

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }

        public async Task<int> GetLogCountAsync()
        {
            var client = CreateClient();
            var response = await client.GetAsync("api/logs/count");

            if (!response.IsSuccessStatusCode)
                return 0;

            var count = await response.Content.ReadFromJsonAsync<int>();
            return count;
        }

        public async Task<List<LogEntryViewModel>> GetLogsAsync(int count)
        {
            var client = CreateClient();
            var response = await client.GetAsync($"api/logs/get/{count}");

            if (!response.IsSuccessStatusCode)
                return new List<LogEntryViewModel>();

            return await response.Content.ReadFromJsonAsync<List<LogEntryViewModel>>();
        }

        public async Task<T?> GetAsync<T>(string uri)
        {
            var client = _httpClientFactory.CreateClient();

            // ✅ Add JWT token if available
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            DebugHelper.PrintDebugMessage("[WebApp] - Getting the data from: " + _baseUrl + uri);

            var response = await client.GetAsync(_baseUrl + uri);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                DebugHelper.PrintDebugMessage("[WebApp] - JSON: " + json);
                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            DebugHelper.PrintDebugMessage($"[WebApp] - Request to {uri} failed with status: {response.StatusCode}");
            return default;
        }

        // eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYmYiOjE3NDg4OTA4NjksImV4cCI6MTc0ODg5ODA2OSwiaWF0IjoxNzQ4ODkwODY5fQ.RPzFeubncVE_5by6ZScU0BHkToZHMDetkLeJP2iquxc
    }

}
