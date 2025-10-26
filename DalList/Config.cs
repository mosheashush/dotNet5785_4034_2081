namespace Dal;
using System;
internal static class Config
{
    /// <summary>
    /// The starting point for the call id
    /// <param name = "StartCallId"> The starting point for the call id</param>
    /// <param name="nextCallId"> The next call id</param>
    /// <patam name="StartAssignmentId"> The starting point for the assignment id</param>
    /// <patam name="nextAssignmentId"> The next assignment id</param>
    /// <patam name="Clock"> The clock time</param>
    /// <patam name="RiskRange"> The risk range</param>
    /// <patam name="Reset"> The reset function</param>
    /// </summary>
    /// 
    
    internal const int StartCallId = 1000;
    private static int nextCallId = StartCallId;
    internal static int NextCallId { get => nextCallId++; }

    internal const int StartAssignmentId = 2000;
    private static int nextAssignmentId = StartAssignmentId;
    internal static int NextAssignmentId { get => nextAssignmentId++; }

    private static DateTime startVirtualTime = DateTime.Now;
    private static DateTime realStartTime = DateTime.Now;
    internal static DateTime Clock {
        get => startVirtualTime + (DateTime.Now - realStartTime);
        set
        {
            startVirtualTime = value;
            realStartTime = DateTime.Now;
        }
    }

    internal static TimeSpan RiskRange { get; set; } = TimeSpan.Zero;

    internal static void Reset()
    {
        nextCallId = StartCallId;
        nextAssignmentId = StartAssignmentId;
        Clock = new DateTime(2023, 1, 1, 8, 0, 0);
        RiskRange = new TimeSpan(1, 12, 0, 0);
    }

    internal static void ResetClock()
    {
        Clock = new DateTime(2023, 1, 1, 8, 0, 0);
    }

}
