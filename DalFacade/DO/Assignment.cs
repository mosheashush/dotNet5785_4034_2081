
namespace DO;

public record Assignment

    /// <summary>
    /// Volunteer Entity represents an assignment with all its props
    /// <param name = "Id"> The Assignments identity number</param>
    /// <param name = "CallId"> The call identity number</param>
    /// <param name = "VolunteerId"> The volunteer identity number</param>
    /// <param name = "StarCall"> The time of the assignment entry</param>
    /// <param name = "CompletionTime"> The time of the assignment completion</param>
    /// <param name = "FinishType"> The type of the assignment completion</param>
    /// </summary>
    (
     int Id,
     int CallId,
     int VolunteerId,
     DateTime StarCall,
     DateTime? CompletionTime = null,
     CompletionType? FinishType = null
    )

{
    /// <summary>
    /// default constructor
    /// </summary>
    public Assignment() : this(0, 0, 0, DateTime.Now, null, null) { }
    /// <summary>
    /// constructor
    /// </summary>
    public Assignment WithId(int id) => new Assignment
    {
        Id = id,
        CallId = this.CallId,
        VolunteerId = this.VolunteerId,
        StarCall = this.StarCall,
        CompletionTime = this.CompletionTime,
        FinishType = this.FinishType
    };

}

