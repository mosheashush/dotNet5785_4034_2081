using DalApi;

namespace Dal;

/// <summary>
///Implement the properties and methods defined in the IConfig interface to manage configurations related to the "Configuration" entity.
/// </summary>
/// 
public class ConfigImplementation : IConfig
{
    /// <summary>
    /// Gets or sets the current clock time for the configuration.
    /// This property interacts with the Config.Clock field to get or set the value.
    /// </summary>
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }

    /// <summary>
    /// Gets or sets the risk range time span for the configuration.
    /// This property interacts with the Config.RiskRange field to get or set the value.
    /// </summary>
    public TimeSpan RiskRange
    {
        get => Config.RiskRange;
        set => Config.RiskRange = value;
    }

    /// <summary>
    /// Gets the next available Call ID for the configuration.
    /// This property is read-only and retrieves the next Call ID from the Config.NextCallId field.
    /// </summary>
    public int nextCallId
    { get => Config.NextCallId; }

    /// <summary>
    /// Gets the next available Assignment ID.
    /// This property currently throws a NotImplementedException as it is not implemented yet.
    /// </summary>
    public int nextAsignmentId { get => Config.NextAssignmentId; }

    /// <summary>
    /// Resets the configuration settings.
    /// Calls the Config.Reset method to reset the configuration to its default state.
    /// </summary>
    public void Reset()
    {
        Config.Reset();
    }
}