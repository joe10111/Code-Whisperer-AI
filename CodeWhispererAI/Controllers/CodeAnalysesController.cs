using CodeWhispererAI.DataAccess;
using CodeWhispererAI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeWhispererAI.Controllers
{
    public class CodeAnalysesController : Controller
    {
        private readonly CodeWhispererAIContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CodeAnalysesController(CodeWhispererAIContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: CodeAnalyses
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var codeAnalyses = await _context.CodeAnalyses.Where(c => c.ApplicationUserId == userId).ToListAsync();
            return View(codeAnalyses);
        }

        // GET: CodeAnalyses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var codeAnalysis = await _context.CodeAnalyses.FindAsync(id);
            if (codeAnalysis == null || codeAnalysis.ApplicationUserId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            return View(codeAnalysis);
        }

        // POST: CodeAnalyses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,APIQuery,Analysis,Timestamp,...")] CodeAnalysis codeAnalysis)
        {
            if (id != codeAnalysis.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null || codeAnalysis.ApplicationUserId != user.Id)
                    {
                        return NotFound();
                    }

                    _context.Update(codeAnalysis);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CodeAnalysisExists(codeAnalysis.Id))
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
            return View(codeAnalysis);
        }

        // GET: CodeAnalyses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var codeAnalysis = await _context.CodeAnalyses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (codeAnalysis == null || codeAnalysis.ApplicationUserId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            return View(codeAnalysis);
        }

        // POST: CodeAnalyses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var codeAnalysis = await _context.CodeAnalyses.FindAsync(id);
            if (codeAnalysis.ApplicationUserId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            _context.CodeAnalyses.Remove(codeAnalysis);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CodeAnalysisExists(int id)
        {
            return _context.CodeAnalyses.Any(e => e.Id == id);
        }
    }
}