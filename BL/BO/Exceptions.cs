namespace BO;

[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, Exception innerException) : base(message, innerException) { }
}

[Serializable]
public class BlNullPropertyException : Exception
{
    public BlNullPropertyException(string? message) : base(message) { }
}

[Serializable]
public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message) : base(message) { }
    public BlAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }
}

//blCanNotCreatArgumentNullException
[Serializable]
public class blCanNotCreatArgumentNullException : Exception
{
    public blCanNotCreatArgumentNullException(string? message) : base(message) { }
}
//BlInMiddlePerformingTaskException

[Serializable]
public class BlInMiddlePerformingTaskException : Exception
{
    public BlInMiddlePerformingTaskException(string? message) : base(message) { }
}

//BlNotAllowedMakeChangesException
[Serializable]
public class BlNotAllowedMakeChangesException : Exception
{
    public BlNotAllowedMakeChangesException(string? message) : base(message) { }
}

//BlInvalidValueException
[Serializable]
public class BlInvalidValueException : Exception
{
    public BlInvalidValueException(string? message) : base(message) { }
}

//BlCanNotOrderNotExistsFieldException
[Serializable]
public class BlCanNotOrderNotExistsFieldException : Exception
{
    public BlCanNotOrderNotExistsFieldException(string? message) : base(message) { }
}

//NoTimeCompleteTaskException
[Serializable]
public class NoTimeCompleteTaskException : Exception
{
    public NoTimeCompleteTaskException(string? message) : base(message) { }
}