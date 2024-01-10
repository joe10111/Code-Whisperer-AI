using CodeWhispererAI.Models;
using CodeWhispererAI.Services;
using CodeWhispererAI.DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;

namespace CodeWhispererAI.Controllers
{
    public class AnalysisController : Controller
    {
        private readonly OpenAIService _openAIService;
        private readonly CodeWhispererAIContext _dbContext;
        private readonly IMemoryCache _memoryCache;
        public AnalysisController(OpenAIService openAIService, CodeWhispererAIContext dbContext, IMemoryCache memoryCache)
        {
            _openAIService = openAIService;
            _dbContext = dbContext;
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            var viewModel = new CodeSnippetAnalysisViewModel
            {
                CodeSnippet = new CodeSnippet(), 
                ChatCompletion = null 
            };

            return View(viewModel);
        }

        [HttpPost] 
        public async Task<IActionResult> AnalyzeCodeSnippet(CodeSnippetAnalysisViewModel viewModel)
        {
              // Handle the case where the code snippet is empty or null
            if (string.IsNullOrWhiteSpace(viewModel.CodeSnippet.CodeInputted))
            {
                Log.Warning("Code Snippet is empty or null");
                return View("Index");
            }

            // Retrieve the current logged-in user ID and IP address
            var userId = GetUserIdFromCookie(HttpContext);
            var ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();

            // Check if the analysis limit has been reached
            if (!string.IsNullOrWhiteSpace(userId) && await IsAnalysisLimitReached(userId, ipAddress))
            {
                // Inform the user that the limit has been reached
                viewModel.LimitReached = true; 
                return View("Index", viewModel);
            }

            // Construct the prompt for the OpenAI API call
            string[] prompts = {
                                 $"Review this for Code Cleanliness: `{viewModel.CodeSnippet.CodeInputted}`",
                                 $"Analyze the Time Complexity of: `{viewModel.CodeSnippet.CodeInputted}`",
                                 $"Suggest Areas of Improvement for: `{viewModel.CodeSnippet.CodeInputted}`"
                               };

            try
            {
                // Call the OpenAI service and get the results
                ChatCompletion chatCompletion = await _openAIService.PostAndGetChatCompletion(prompts);

                // Update the ViewModel with the results
                viewModel.ChatCompletion = chatCompletion;

      
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    // Create separate cache keys
                    var snippetCacheKey = $"CodeSnippet_{userId}";
                    var analysisCacheKey = $"CodeAnalysis_{userId}";

                    // Construct instances of concrete types for caching
                    var cachedSnippet = new CodeSnippet
                    {
                        CodeInputted = viewModel.CodeSnippet.CodeInputted,
                        LanguageUsed = "C#",
                        ApplicationUserId = userId
                    };

                    var cachedAnalysis = new CodeAnalysis
                    {
                        APIQuery = string.Join(" ", prompts),
                        Analysis = chatCompletion.Choices[0].Message.Content,
                        Timestamp = DateTime.UtcNow,
                        ApplicationUserId = userId
                    };

                    // Set cache options
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromDays(1)); // Example: 1 day expiration

                    // Set the individual objects in cache
                    _memoryCache.Set(snippetCacheKey, new List<CodeSnippet> { cachedSnippet }, cacheEntryOptions);
                    _memoryCache.Set(analysisCacheKey, new List<CodeAnalysis> { cachedAnalysis }, cacheEntryOptions);
                }

                // Return the view with the ViewModel containing the input and results
                return View("Index", viewModel);
            }
            catch (HttpRequestException e)
            {
                // Log the exception
                Log.Warning("Caught exception: " + e);
                return View("Error");
            }
        }

        public string GetUserIdFromCookie(HttpContext context)
        {
            if (context.Request.Cookies.TryGetValue("UserIdentifier", out string userId))
            {
                return userId;
            }
            return null;
        }

        private async Task<bool> IsAnalysisLimitReached(string userId, string ipAddress)
        {
            int currentWeek = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(DateTime.UtcNow, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            string cacheKey = $"AnalysisAttempts_{userId}_{ipAddress}_{currentWeek}";

            if (!_memoryCache.TryGetValue(cacheKey, out int currentCount))
            {
                currentCount = 0;
            }

            if (currentCount >= 3)
            {
                // Limit is reached
                return true;
            }

            // Increment and update the cache
            currentCount++;
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(new DateTimeOffset(DateTime.UtcNow.AddDays(7 - (int)DateTime.UtcNow.DayOfWeek)));
            _memoryCache.Set(cacheKey, currentCount, cacheEntryOptions);

            // Limit not reached
            return false;
        }

    }
}

