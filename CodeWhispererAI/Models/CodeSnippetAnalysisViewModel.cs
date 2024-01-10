namespace CodeWhispererAI.Models
{
    public class CodeSnippetAnalysisViewModel
    {
        public CodeSnippet CodeSnippet { get; set; }
        public ChatCompletion ChatCompletion { get; set; }
        public bool LimitReached { get; set; }  // A flag to indicate if the rate limit is reached
    }
}
