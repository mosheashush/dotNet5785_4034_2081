using Helpers;

namespace BO;

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
    
    //BO fields:
    public CallState CallState { get; set; }
    public List<BO.CallAssignInList>? callAssignInLists { get; set; }

    public override string ToString() => this.ToStringProperty();

}
