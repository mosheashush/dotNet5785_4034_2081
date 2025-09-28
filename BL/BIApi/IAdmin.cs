namespace BIApi;

public interface IAdmin
{
    /// <summary>
    /// Returns the current system clock value.
    /// </summary>
    DateTime GetClock();

    /// <summary>
    /// Advances the system clock according to the given time unit.
    /// </summary>
    /// <param name="timeUnit">The time unit to advance the clock by.</param>
    void AdvanceClock(BO.TimeUnit timeUnit);
    
    /// <summary>
    /// Returns the current configuration value for "Risk Time Span".
    /// </summary>
    TimeSpan GetRiskTimeSpan();

    /// <summary>
    /// Sets the configuration value for "Risk Time Span".
    /// </summary>
    /// <param name="riskTimeSpan">The new risk time span value.</param>
    void SetRiskTimeSpan(TimeSpan riskTimeSpan);

    /// <summary>
    /// Resets the database: resets configuration values and clears all entity data.
    /// </summary>
    void ResetDatabase();

    /// <summary>
    /// Initializes the database: resets it and populates it with initial data.
    /// </summary>
    void InitializeDatabase();

    #region Stage 5 
    void AddConfigObserver(Action configObserver);
    void RemoveConfigObserver(Action configObserver);
    void AddClockObserver(Action clockObserver);
    void RemoveClockObserver(Action clockObserver);
    #endregion Stage 5 
}

/// <summary>
/// Represents the available time units for advancing the clock.
/// </summary>


