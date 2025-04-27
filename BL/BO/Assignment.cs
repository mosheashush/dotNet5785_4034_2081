
namespace BO
{
    public class Assignment
    {
        public int IdAssignment { get; init; }
        public int IdCall { get; init; }
        public int VolunteerId { get; init; }
        public DateTime StarCall { get; set; }
        public DateTime? CompletionTime { get; set; }
        public CompletionType? FinishType { get; set; }
        public override string ToString() => this.ToString();

    }
}
