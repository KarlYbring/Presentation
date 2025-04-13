namespace Domain.Responses;

public class ProjectResult<T> : ServiceResult
{
    public T? Result { get; set; }
}

public class ProjectResult : ServiceResult
{
    
}
