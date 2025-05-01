namespace BO;

public enum User
{
    volnteer,
    admin
}

public enum Distance
{
    air,
    drive,
    walk,
}

public enum CallType
{
    makingfood,
    deliveringfood,
}

public enum CompletionType
{
    canceledAdmin,
    canceledVolunteer,
    completed,
    expired,
}

//new in BL:

public enum CallState
{
    open,
    openOnRisk,
    processed,
    ProcessedOnRisk,
    completed,
    expired,
}

public enum VolunteerInListFields
{
    IdVolunteer,
    FullName,
    Active,
    IdCall,
    Type,
    SumCallsCompleted,
    SumCallsExpired,
    SumCallsConcluded
}

public enum CallInListFields
{
    IdAssignment,
    IdCall,
    Type,
    CallStartTime,
    TimeRemaining,
    NameFinalVolunteer,
    SumTimeProcess,
    CollState,
    SumOfAssignments
}

public enum ClosedCallFields
{
    IdCall,
    Type,
    FullAddress,
    CallStartTime,
    VolunteerTakeCall,
    CompletionTime,
    FinishType
}

public enum OpenCallFields
{
    IdCall,
    Type,
    Description,
    FullAddress,
    CallStartTime,
    MaxTimeForCall,
    DistanceFromVolunteer
}

public enum TimeUnit
{
    Minute,
    Hour,
    Day,
    Month,
    Year
}