namespace BO;

public class OpenCallInList
{
    public int IdCall { get; init; }
    public CallType Type { get; set; }
    public string? description { get; set; }
    public string FullAddress { get; set; }
    public DateTime CallStartTime { get; set; }
    public DateTime? MaxTimeForCall { get; set; }
    public double DistanceFromVolunteer { get; set; }

    public override string ToString() => this.ToString();

}
