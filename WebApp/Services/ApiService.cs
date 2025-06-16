using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using GamePlatformBL.Models;
using GamePlatformBL.Utilities;
using GamePlatformBL.ViewModels;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

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
            var client = _httpClientFactory.CreateClient();


            var token = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "JwtToken");

            DebugHelper.AppPrintDebugMessage($"TOKEN: {token}");
            if (token != null && !string.IsNullOrEmpty(token.Value))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
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
                var client = CreateClient();

                DebugHelper.AppPrintDebugMessage("[WebApp] - Getting the data from: " + _baseUrl + uri);
                try
                {
                    var response = await client.GetAsync(_baseUrl + uri);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        DebugHelper.AppPrintDebugMessage("JSON: " + json);
                        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }

                    DebugHelper.AppPrintDebugMessage($"Request to {uri} failed with status: {response.StatusCode}");
                    return default;
                }
                catch (Exception ex)
                {
                    return default;
                }

            }

            public async Task<HttpResponseMessage> PostAsync<T>(string uri, T data)
            {
            var client = CreateClient();

            var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var fullUri = $"{_baseUrl}{uri}";
                DebugHelper.AppPrintDebugMessage($"Posting to: {fullUri}");
                DebugHelper.AppPrintDebugMessage($"Payload: {json}");

                return await client.PostAsync(fullUri, content);
            }

            public async Task<HttpResponseMessage> PutWithResponseAsync<T>(string uri, T data)
            {
            var client = CreateClient();

                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var fullUri = $"{_baseUrl}{uri}";
                DebugHelper.AppPrintDebugMessage($"Posting to: {fullUri}");
                DebugHelper.AppPrintDebugMessage($"Payload: {json}");

                return await client.PutAsync(fullUri, content);
            }
            public async Task<HttpResponseMessage> DeleteAsync(string uri)
            {
            var client = CreateClient();

                var fullUri = $"{_baseUrl}{uri}";
                DebugHelper.AppPrintDebugMessage($"Sending DELETE to: {fullUri}");

                return await client.DeleteAsync(fullUri);
            }

        }

    }
