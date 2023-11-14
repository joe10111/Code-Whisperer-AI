namespace CodeWhispererAI.Models
{
    public class ChatCompletion
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public long Created { get; set; }
        public string Model { get; set; }
        public string SystemFingerprint { get; set; }
        public List<Choice> Choices { get; set; }
        public Usage Usage { get; set; }

         // Method to get category names to populate card headers in `Analysis/index.cshtml`
        public string ExtractCategoryName(string content)
        {
            var endOfCategoryMarker = content.IndexOf(":");
            if (endOfCategoryMarker > -1)
            {
                return content.Substring(0, endOfCategoryMarker);
            }
            return "Unknown Category"; // Fallback if no category name is found
        }
    }  

    public class Choice
    {
        public int Index { get; set; }
        public Message Message { get; set; }
        public string FinishReason { get; set; }
    }

    public class Message
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }

    public class Usage
    {
        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }
        public int TotalTokens { get; set; }
    }

}
