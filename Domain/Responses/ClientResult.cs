using Domain.Models;

namespace Domain.Responses;

public class ClientResult : ServiceResult
{
    
}

public class ClientResult<T> : ClientResult
{
    public T? Result { get; set; }
}
