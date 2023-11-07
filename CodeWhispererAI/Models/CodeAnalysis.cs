namespace CodeWhispererAI.Models
{
    public class CodeAnalysis
    {
        public int Id { get; set; }
         // Inital Input Query
        public string ?APIQuery { get; set; }

         // Feedback of code
        public string ?Analysis { get; set; }

        public DateTime Timestamp { get; set; }

        public int ResponseStatus { get; set; }

        public int CodeId { get; set; }
    }
}
