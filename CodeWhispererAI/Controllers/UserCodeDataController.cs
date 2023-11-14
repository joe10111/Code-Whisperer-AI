using CodeWhispererAI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CodeWhispererAI.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeWhispererAI.Controllers
{
    [Authorize]
    public class UserCodeDataController : Controller
    {
        private readonly CodeWhispererAIContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserCodeDataController(CodeWhispererAIContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: UserCodeData
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var viewModel = new UserCodeDataViewModel
            {
                CodeSnippets = await _context.CodeSnippets.Where(c => c.ApplicationUserId == userId).ToListAsync(),
                CodeAnalyses = await _context.CodeAnalyses.Where(c => c.ApplicationUserId == userId).ToListAsync()
            };

            return View(viewModel);
        }
    }

}
