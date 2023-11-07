using CodeWhispererAI.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CodeWhispererAI.Controllers
{
    public class AnalysisController : Controller
    {
        private readonly OpenAIService _openAIService;
        public AnalysisController(OpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AnalyzeCodeSnippet(string codeSnippetInput)
        {
              // Handle the case where the code snippet is empty or null
            if (string.IsNullOrWhiteSpace(codeSnippetInput))
            {
                Log.Warning("Code Snippet is empty or null");
                return View("Index");
            }

            string prompt = $"Give me feedback on this code snippet: `{codeSnippetInput}`";
            try
            {
                var completion = await _openAIService.PostAndGetChatCompletion(prompt);
                // Do something with the completion, like displaying it in the view
                return View("Result", completion);
            }
            catch (HttpRequestException e)
            {
                  // Log the exception
                Log.Warning("Cuaght exception: " + e);
                return View("Error");
            }
        }
    }
}
