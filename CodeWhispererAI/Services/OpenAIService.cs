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

        public OpenAIService(IConfiguration configuration)
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

        public async Task<ChatCompletion> PostAndGetChatCompletion(string[] prompts)
        {
            var requestData = new
            {
                model = "gpt-4-1106-preview",
                messages = new[]
                {
                    new { role = "system", content = "You are an AI that provides feedback on code snippets. Provide feedback on code cleanliness, time complexity, and areas of improvement separately. Each seperate peice of feedback needs to be less than or equal to 300 tokens. Dont not write out any code examples only educate on each topic. Dont not only focus on one of the topics, you have to do all three." },
                    new { role = "user", content = prompts[0] }, // Code Cleanliness
                    new { role = "user", content = prompts[1] }, // Time Complexity
                    new { role = "user", content = prompts[2] }  // Areas of Improvement
                },
                temperature = 0.8,
                max_tokens = 2000
            };

            string jsonRequestData = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PostAsync("v1/chat/completions", content);

            if(response.IsSuccessStatusCode)
            {
                Log.Information("API call was successful");

                string responseContent = await response.Content.ReadAsStringAsync();
                var chatCompletion = JsonConvert.DeserializeObject<ChatCompletion>(responseContent);

                return chatCompletion;
            }
            else
            {
                Log.Error("GBT API call failed");
                throw new HttpRequestException($"Error: {response.StatusCode}");
            }
        }
    }
}
