using System.Net.Http.Headers;
using System.Net.Http;
using CodeWhispererAI.DataAccess;
using CodeWhispererAI.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;
using Serilog;
using Microsoft.Extensions.Caching.Memory;

namespace CodeWhispererAI.Services
{
    public class OpenAIService
    {
        private static HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;

        public OpenAIService(IConfiguration configuration, IMemoryCache cache)
        {
            _configuration = configuration;
            InitializeHttpClient();
            _cache = cache;
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
             // Generate a Cache Key
            var cacheKey = $"ChatCompletion-{String.Join("-", prompts)}";

            var requestData = new
            {
                model = "gpt-4-1106-preview",
                messages = new[]
                {
                    new { role = "system", content = "You are an AI that provides feedback on code snippets. " +
                                                     "Provide feedback on code cleanliness, time complexity, and areas of improvement separately. " +
                                                     "Rule: Each seperate peice of feedback needs to be less than or equal to 500 tokens. " +
                                                     "Rule: Each output line should only be 80 characaters or less untill new line is insertred then go to next line. " +
                                                     "Rule: Dont not write out any code examples only educate on each topic. " +
                                                     "Rule: Dont not only focus on one of the topics, you have to do all three. " +
                                                     "Rule: Formatt each topic like this TopicName Feedback:" },
                    new { role = "user", content = prompts[0] }, // Code Cleanliness
                    new { role = "user", content = prompts[1] }, // Time Complexity
                    new { role = "user", content = prompts[2] }  // Areas of Improvement
                },
                temperature = 0.8,
                max_tokens = 2000
            };

             // If cache has key that matches current output pull it out
            if (!_cache.TryGetValue(cacheKey, out ChatCompletion cachedCompletion))
            {
                string jsonRequestData = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PostAsync("v1/chat/completions", content);

                if (response.IsSuccessStatusCode)
                {
                    Log.Information("API call was successful");

                    string responseContent = await response.Content.ReadAsStringAsync();
                    var chatCompletion = JsonConvert.DeserializeObject<ChatCompletion>(responseContent);

                    string chatContent = chatCompletion.Choices[0].Message.Content; // Your feedback string

                    // Define the markers that denote the start of each category
                    string[] categoryMarkers = new string[]
                    {
                    "Code Cleanliness Feedback:",
                    "Time Complexity Feedback:",
                    "Areas of Improvement Feedback:"
                    };

                    // Split the content string by the category markers
                    string[] feedbackSections = chatContent.Split(categoryMarkers, StringSplitOptions.RemoveEmptyEntries);

                    // Make sure to trim the feedback sections to remove any leading/trailing whitespace
                    for (int i = 0; i < feedbackSections.Length; i++)
                    {
                        feedbackSections[i] = feedbackSections[i].Trim();

                        // Add a new Choice for each feedback section
                        chatCompletion.Choices.Add(new Choice
                        {
                            Message = new Message { Content = categoryMarkers[i] + "\n" + feedbackSections[i].Trim() }
                        });
                    }

                     // Cache Results for 30 mins
                    _cache.Set(cacheKey, chatCompletion, TimeSpan.FromMinutes(30));

                    return chatCompletion;
                }
                else
                {
                    Log.Error("GBT API call failed");
                    throw new HttpRequestException($"Error: {response.StatusCode}");
                }
            }
            else
            {
                 // Return the cached result if available
                return cachedCompletion;
            }
        }
    }
}
