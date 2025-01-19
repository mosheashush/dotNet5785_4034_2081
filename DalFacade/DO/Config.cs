namespace DO;
public record Config
(
    /// <summary>
    /// <param name="nextCallId"> The next call id</param>
    /// <patam name="nextAssignmentId"> The next assignment id</param>
    /// <patam name="Clock"> The clock time</param>
    /// <patam name="RiskRange"> The risk range</param>
    /// </summary>
    int NextCallId,
    int NextAssignmentId,
    TimeSpan RiskRange,
    DateTime Clock
)

{
    public Config(Config other)
    {
        NextCallId = other.NextCallId;
        NextAssignmentId = other.NextAssignmentId;
        RiskRange = other.RiskRange;
        Clock = other.Clock;
    }
    public Config() : this(10000000, 10000000, new TimeSpan(0, 0, 0, 0, 0), DateTime.Now) { }
    


}
