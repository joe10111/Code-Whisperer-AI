using CodeWhispererAI.Models;
using CodeWhispererAI.Services;
using CodeWhispererAI.DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Microsoft.AspNetCore.Authorization;

namespace CodeWhispererAI.Controllers
{
    public class AnalysisController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly OpenAIService _openAIService;
        private readonly CodeWhispererAIContext _dbContext;
        public AnalysisController(OpenAIService openAIService, UserManager<ApplicationUser> userManager, CodeWhispererAIContext dbContext)
        {
            _openAIService = openAIService;
            _userManager = userManager;
            _dbContext = dbContext;
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

                // Retrieve the current logged-in user
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    // Create and save the CodeSnippet
                    var codeSnippet = new CodeSnippet
                    {
                        CodeInputted = viewModel.CodeSnippet.CodeInputted,
                        LanguageUsed = "C#",
                        ApplicationUserId = user.Id // Set the foreign key to the user's Id
                                                    
                    };
                    _dbContext.CodeSnippets.Add(codeSnippet);

                    var codeAnalysis = new CodeAnalysis
                    {
                        APIQuery = (prompts[0] + prompts[1] + prompts[2]), 
                        Analysis = chatCompletion.Choices[0].Message.Content,
                        Timestamp = DateTime.UtcNow,
                        ApplicationUserId = user.Id // Set the foreign key to the user's Id
                                                    
                    };
                    _dbContext.CodeAnalyses.Add(codeAnalysis);

                    // Save changes to the database
                    await _dbContext.SaveChangesAsync();
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
    }
}
