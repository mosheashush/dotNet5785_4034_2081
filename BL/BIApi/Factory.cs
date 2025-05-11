namespace BIApi;

public static class Factory
{
    /// <summary>
    /// Returns the BIApi instance.
    /// </summary>
    public static IBl Get() => new BlImplementation.BI();
}
