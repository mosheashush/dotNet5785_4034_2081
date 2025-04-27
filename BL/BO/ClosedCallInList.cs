namespace BO
{
    public class ClosedCallInList
    {
        public int IdCall { get; init; }
        public CallType Type { get; set; }
        public string FullAddress { get; set; }
        public DateTime CallStartTime { get; set; }
        public DateTime VolunteerTakeCall { get; set; } //take from assignment entity
        public DateTime? CompletionTime { get; set; }
        public CompletionType? FinishType { get; set; }

        public override string ToString() => this.ToString();

    }
}
