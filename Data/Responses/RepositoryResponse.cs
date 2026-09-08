namespace ExpenseManager.Data.Responses;

public class RepositoryResponse<T>
{
    public T? Data { get; set; }
    public bool IsSuccess { get; set; } = true;
    public string Message { get; set; } = string.Empty;
    public static RepositoryResponse<T> Success(T data, string message = "Success")
        => new() { Data = data, Message = message, IsSuccess = true };

    public static RepositoryResponse<T> Success(string message = "Success")
        => new() { Message = message, IsSuccess = true };

    public static RepositoryResponse<T> NotFound(string message = "Resource not found")
        => new() { IsSuccess = false, Message = message };

    public static RepositoryResponse<T> InternalError(string message)
         => new() { IsSuccess = false, Message = message };
    public static RepositoryResponse<T> NotAllowed(string message)
  => new() { IsSuccess = false, Message = message };
}

public class RepositoryResponse : RepositoryResponse<object>
{
    public new static RepositoryResponse Success(string message = "Success")
        => new() { Message = message, IsSuccess = true };

    public new static RepositoryResponse NotFound(string message = "Resource not found")
        => new() { IsSuccess = false, Message = message };

    public new static RepositoryResponse InternalError(string message)
        => new() { IsSuccess = false, Message = message };
    public new static RepositoryResponse NotAllowed(string message)
           => new() { IsSuccess = false, Message = message };
}

