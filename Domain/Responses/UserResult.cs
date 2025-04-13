using Domain.Models;

namespace Domain.Responses;

public class UserResult : ServiceResult
{
    
}

public class UserResult<T> : UserResult
{
    public T? Result { get; set; } 
}