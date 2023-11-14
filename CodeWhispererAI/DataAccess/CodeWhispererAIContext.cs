using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using CodeWhispererAI.Models;


namespace CodeWhispererAI.DataAccess
{
    public class CodeWhispererAIContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<CodeSnippet> CodeSnippets { get; set; }
        public DbSet<CodeAnalysis> CodeAnalyses { get; set; }
        public CodeWhispererAIContext(DbContextOptions<CodeWhispererAIContext> options) : base(options)
        {
            
        }
    }
}
