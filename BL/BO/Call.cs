namespace BO
{
    public class Call
    {
        public int IdCall { get; init; }
        public CallType Type { get; set; }
        public string? description { get; set; }
        public string FullAddress { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime CallStartTime { get; set; }
        public DateTime? MaxTimeForCall { get; set; }
        public CallState CollState { get; set; }
        public List<BO.CallAssignInList>? callAssignInLists { get; set; }
        public override string ToString() => this.ToString();

    }
}
