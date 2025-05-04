namespace BO;

public class CallAssignInList
{
    public int? IdVolunteer { get; init; }
    public string? FullName { get; set; }
    public DateTime CallStartTime { get; set; }
    public DateTime? CompletionTime { get; set; }
    public CompletionType? FinishType { get; set; }

    public override string ToString() => this.ToString();

}
