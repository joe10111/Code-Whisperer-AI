using System.Net.Http.Headers;
using System.Net.Http;
using CodeWhispererAI.DataAccess;
using CodeWhispererAI.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;
using Serilog;

namespace CodeWhispererAI.Services
{
    public class OpenAIService
    {
        private static HttpClient _client;
        private readonly IConfiguration _configuration;

        public OpenAIService( IConfiguration configuration)
        {
            _configuration = configuration;
            InitializeHttpClient();
        }

        private void InitializeHttpClient()
        {
            if (_client == null)
            {
                _client = new HttpClient()
                {
                    BaseAddress = new Uri("https://api.openai.com/")
                };

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["OpenAIAPIKey"]);
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }

        public async Task<ChatCompletion> PostAndGetChatCompletion(string prompt)
        {
            var requestData = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = "You are a code analyzer" },
                    new { role = "user", content = prompt }
                },
                temperature = 0.5,
                max_tokens = 60
            };


            string jsonRequestData = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PostAsync("v1/chat/completions", content);

            if(response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var chatCompletion = JsonConvert.DeserializeObject<ChatCompletion>(responseContent);

                return chatCompletion;
            }
            else
            {
                Log.Error("GBT 3.5 Turbo API call failed");
                throw new HttpRequestException($"Error: {response.StatusCode}");
            }
        }
    }
}
