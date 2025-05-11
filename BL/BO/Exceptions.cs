namespace BO;

[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, Exception innerException) : base(message, innerException) { }
}

[Serializable]
public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message) : base(message) { }
    public BlAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }
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

//BlDataMismatchExcept
[Serializable]
public class BlDataMismatchException : Exception
{
    public BlDataMismatchException(string? message) : base(message) { }
}

//BlCanNotOrderNotExistsFieldException (Maybe unnecessary)
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
