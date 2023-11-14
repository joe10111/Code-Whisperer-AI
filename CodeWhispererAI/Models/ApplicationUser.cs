using Microsoft.AspNetCore.Identity;

namespace CodeWhispererAI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<CodeSnippet> CodeSnippets { get; set; } = new List<CodeSnippet>();

        public  ICollection<CodeAnalysis> CodeAnalyses { get; set; } = new List<CodeAnalysis>();
    }
}
