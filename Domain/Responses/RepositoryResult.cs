namespace Domain.Responses;


public class RepositoryResult : ServiceResult
{
   
}

public class RepositoryResult<T> : RepositoryResult
{
    public T? Result { get; set; }
}
