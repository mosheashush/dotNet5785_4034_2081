namespace BO;

public class Volunteer
{
    public int id { get; init; }
    public string FullName { get; set; }
    public string CallNumber { get; set; }
    public string EmailAddress { get; set; }
    public string? Password { get; set; }
    public string? FullCurrentAddress { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude  { get; set; }
    public User CurrentPosition { get; set; }
    public bool Active { get; set; }
    public double? MaxDistanceForCall { get; set; }
    public Distance TypeOfDistance { get; set; }

    //new in BL
    public int SumCallsCompleted {  get; set; }
    public int SumCallsExpired { get; set; }
    public int SumCallsConcluded { get; set; }
    public BO.CallInProgress? CallInProgress { get; set; }

    public override string ToString() => this.ToString();

}
