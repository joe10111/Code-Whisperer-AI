using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CodeWhispererAI.DataAccess;
using Microsoft.AspNetCore.Mvc;
using CodeWhispererAI.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeWhispererAI.Controllers
{
    [Authorize]
    public class CodeSnippetsController : Controller
    {
        private readonly CodeWhispererAIContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CodeSnippetsController(CodeWhispererAIContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: CodeSnippets
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var codeSnippets = await _context.CodeSnippets.Where(c => c.ApplicationUserId == userId).ToListAsync();
            return View(codeSnippets);
        }

        // GET: CodeSnippets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var codeSnippet = await _context.CodeSnippets.FindAsync(id);
            if (codeSnippet == null || codeSnippet.ApplicationUserId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            return View(codeSnippet);
        }

        // POST: CodeSnippets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CodeInputted,LanguageUsed")] CodeSnippet codeSnippet)
        {
            if (id != codeSnippet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null || codeSnippet.ApplicationUserId != user.Id)
                    {
                        return NotFound();
                    }

                    _context.Update(codeSnippet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CodeSnippetExists(codeSnippet.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(codeSnippet);
        }

        // GET: CodeSnippets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var codeSnippet = await _context.CodeSnippets
                .FirstOrDefaultAsync(m => m.Id == id);
            if (codeSnippet == null || codeSnippet.ApplicationUserId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            return View(codeSnippet);
        }

        // POST: CodeSnippets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var codeSnippet = await _context.CodeSnippets.FindAsync(id);
            if (codeSnippet.ApplicationUserId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            _context.CodeSnippets.Remove(codeSnippet);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CodeSnippetExists(int id)
        {
            return _context.CodeSnippets.Any(e => e.Id == id);
        }
    }
}
