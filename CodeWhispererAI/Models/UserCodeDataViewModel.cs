namespace CodeWhispererAI.Models
{
    public class UserCodeDataViewModel
    {
        public IEnumerable<CodeSnippet> CodeSnippets { get; set; }
        public IEnumerable<CodeAnalysis> CodeAnalyses { get; set; }
    }
}
