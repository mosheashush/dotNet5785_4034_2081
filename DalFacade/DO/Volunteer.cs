
namespace DO
{
    /// <summary>
    /// An entity that represents a volunteer with all their personal details
    /// <param name = "id"> The volunteer's identity number</param>
    /// <param name = "FullName"> First and last name of the volunteer</param> 
    /// <param name = "EmailAddress"> Email address of the volunteer</param>
    /// <param name = "Password">The volunteer's password</param>
    /// <param name = "FullCurrentAddress">The volunteer's current full address</param>
    /// <param name = "Latitude">Latitude of the volunteer's address</param>
    /// <param name = "Longitud">Longitude of the volunteer's address</param>
    /// <param name = "CurrentPosition">The role of the volunteer</param>
    /// <param name = "Active">Whether the volunteer is active or not</param>
    /// <param name = "MaxDistanceForCall">The maximum distance up to which the volunteer can receive a call</param>
    /// <param name = "TypeOfDistance">The distance type of the reading</param>
    /// </summary> 
    public record Volunteer
    (
        int id,
        string FullName,
        string CallNumber,
        string EmailAddress,
        string Password,
        string FullCurrentAddress,
        double Latitude,
        double Longitud,
        User CurrentPosition,
        bool Active,
        double MaxDistanceForCall,
        Distance TypeOfDistance
    )


    
    {
        /// <summary>
        /// contains the details of the volunteer's current location
        /// </summary>
        public Volunteer with => new Volunteer
            (
            this.id,
            this.FullName,
            this.CallNumber,
            this.EmailAddress,
            this.Password,
            this.FullCurrentAddress,
            this.Latitude,
            this.Longitud,
            this.CurrentPosition,
            this.Active,
            this.MaxDistanceForCall,
            this.TypeOfDistance
             );

        ///<summary>
        /// default constructor
        /// </summary>
        public Volunteer() : this(0, ""," ", "", "", "", 0, 0, User.volnteer, false, 0, Distance.air){}
        
    };



}
