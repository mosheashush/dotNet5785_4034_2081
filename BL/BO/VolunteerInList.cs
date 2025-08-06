using Helpers;

namespace BO;

public class VolunteerInList
{
    public int IdVolunteer { get; init; }
    public string FullName { get; set; }
    public bool Active { get; set; }

    public int? IdCall { get; set; }
    public CallType Type { get; set; }

    public int SumCallsCompleted { get; set; }
    public int SumCallsExpired { get; set; }
    public int SumCallsConcluded { get; set; }

    public override string ToString() => this.ToStringProperty();

}
