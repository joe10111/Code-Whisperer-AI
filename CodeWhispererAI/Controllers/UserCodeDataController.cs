using CodeWhispererAI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CodeWhispererAI.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CodeWhispererAI.Controllers
{
    public class UserCodeDataController : Controller
    {
        //private readonly CodeWhispererAIContext _context;
        private readonly IMemoryCache _memoryCache;


        public UserCodeDataController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        // GET: UserCodeData
        public async Task<IActionResult> Index()
        {
            var userId = GetUserIdFromCookie(HttpContext);
            if (userId == null)
            {
                // Handle the case where the user ID is not in the cookie
                return View(new UserCodeDataViewModel());
            }

            var viewModel = new UserCodeDataViewModel();

            // Construct the cache keys
            var snippetCacheKey = $"CodeSnippet_{userId}";
            var analysisCacheKey = $"CodeAnalysis_{userId}";

            // Try to retrieve the entire collection of CodeSnippets from cache
            if (_memoryCache.TryGetValue(snippetCacheKey, out IEnumerable<CodeSnippet> cachedSnippets))
            {
                viewModel.CodeSnippets = cachedSnippets;
            }
            else
            {
                viewModel.CodeSnippets = Enumerable.Empty<CodeSnippet>();
            }

            // Try to retrieve the entire collection of CodeAnalyses from cache
            if (_memoryCache.TryGetValue(analysisCacheKey, out IEnumerable<CodeAnalysis> cachedAnalyses))
            {
                viewModel.CodeAnalyses = cachedAnalyses;
            }
            else
            {
                viewModel.CodeAnalyses = Enumerable.Empty<CodeAnalysis>();
            }

            return View(viewModel);
        }

        public string GetUserIdFromCookie(HttpContext context)
        {
            if (context.Request.Cookies.TryGetValue("UserIdentifier", out string userId))
            {
                return userId;
            }
            return null;
        }
    }

}
