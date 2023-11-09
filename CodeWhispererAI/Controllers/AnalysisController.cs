using CodeWhispererAI.Models;
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

            // Construct the prompt for the OpenAI API call
            string prompt = $"Give me feedback on this code snippet: `{viewModel.CodeSnippet.CodeInputted}`";

            try
            {
                 // Call the OpenAI service and get the results
                ChatCompletion chatCompletion = await _openAIService.PostAndGetChatCompletion(prompt);

                 // Update the ViewModel with the results
                viewModel.ChatCompletion = chatCompletion;

                 // Return the view with the ViewModel containing the input and results
                return View("Index", viewModel);
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
