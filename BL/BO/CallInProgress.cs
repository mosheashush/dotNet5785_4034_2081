using Helpers;

namespace BO;

public class CallInProgress
{
    public int IdAssignment { get; init; } 
    public int IdCall { get; init; }
    public CallType Type { get; set; }
    public string? description { get; set; }
    public string FullAddress { get; set; }
    public DateTime CallStartTime { get; set; }
    public DateTime? MaxTimeForCall { get; set; }
    public DateTime VolunteerTakeCall { get; set; } //take from assignment entity

    //new in BL
    public double DistanceFromVolunteer { get; set; }
    public CallState CallState { get; set; }

    public override string ToString() => this.ToStringProperty();

}
