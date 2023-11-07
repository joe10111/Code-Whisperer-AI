using Microsoft.EntityFrameworkCore;
using CodeWhispererAI.Models;


namespace CodeWhispererAI.DataAccess
{
    public class CodeWhispererAIContext : DbContext
    {
        public DbSet<CodeSnippet> CodeSnippets { get; set; }
        public DbSet<CodeAnalysis> CodeAnalyses { get; set; }
        public CodeWhispererAIContext(DbContextOptions<CodeWhispererAIContext> options) : base(options)
        {
            
        }
    }
}
