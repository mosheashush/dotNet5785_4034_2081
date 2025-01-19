
namespace DO
{

    /// <summary>
    /// Call Entity represents a call with all its props
    /// <param name = "Id"> The Calls identity number</param>
    /// <param name = "Type"> The call type </param>
    /// <param name = "Latitude">Latitude of the call address</param>
    /// <param name = "Longitude">Longitude of the call address</param>
    /// <param name = "CallStartTime"> the time if the start of the call </param>
    /// <param name = "description"> The call description </param>
    /// <param name = "MaxTimeForCall"> maximum time for the call </param>
    public record Call
    (
        int Id,
        CallType Type,
        string FullAddress,
        double Latitude,
        double Longitude,
        DateTime CallStartTime,
        string? description = null,
        DateTime? MaxTimeForCall = null

    )
    {
        public Call() : this(0, 0, "", 0, 0, DateTime.Now, " ", null) { }
        public Call WithId(int id) => new Call
        {
            Id = id,
            Type = this.Type,
            FullAddress = this.FullAddress,
            Latitude = this.Latitude,
            Longitude = this.Longitude,
            CallStartTime = this.CallStartTime,
            MaxTimeForCall = this.MaxTimeForCall,
            description = this.description
        };

    }

 };
