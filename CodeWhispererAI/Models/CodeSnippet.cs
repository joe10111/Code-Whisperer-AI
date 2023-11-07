namespace CodeWhispererAI.Models
{
    public class CodeSnippet
    {
        public int Id { get; set; }        
        public string? CodeInputted { get; set; }
        public string? LanguageUsed { get; set; }
        public int ResponseStatus { get; set; }
        public int FeedbackID { get; set; }
    }
}
