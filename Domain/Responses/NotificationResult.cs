namespace Domain.Responses;

public class NotificationResult : ServiceResult
{

}

public class NotificationResult<T> : NotificationResult
{
    public T? Result { get; set; }
}