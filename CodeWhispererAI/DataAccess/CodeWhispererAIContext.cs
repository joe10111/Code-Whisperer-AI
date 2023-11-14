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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CodeSnippet>()
                .HasOne(cs => cs.ApplicationUser)
                .WithMany(u => u.CodeSnippets)
                .HasForeignKey(cs => cs.ApplicationUserId);

            modelBuilder.Entity<CodeAnalysis>()
                .HasOne(ca => ca.ApplicationUser)
                .WithMany(u => u.CodeAnalyses)
                .HasForeignKey(ca => ca.ApplicationUserId);
        }
    }
}
