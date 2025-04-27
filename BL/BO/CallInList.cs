
using System.Data;

namespace BO
{
    public class CallInList
    {
        public int? IdAssignment { get; init; }
        public int IdCall { get; init; }
        public CallType Type { get; set; }
        public DateTime CallStartTime { get; set; }

        //new in BL
        public TimeSpan? TimeRemaining { get; set; }
        public string? NameFinalVolunteer { get; set; }
        public TimeSpan? SumTimeProcess { get; set; }
        public CallState collState { get; set; }
        public int SumOfAssignments { get; set; }

        public override string ToString() => this.ToString();
    }
}
