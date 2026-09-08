namespace ExpenseManager.Data.Responses;

public class ServiceResponse<T>
{
    public T? Data { get; set; }
    public bool IsSuccess { get; set; } = true;
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; } = 200;

    public static ServiceResponse<T> Success(T data, string message = "Success")
        => new() { Data = data, Message = message, IsSuccess = true, StatusCode = 200 };

    public static ServiceResponse<T> Success(string message = "Success")
        => new() { Message = message, IsSuccess = true, StatusCode = 200 };

    public static ServiceResponse<T> NotFound(string message = "Resource not found")
        => new() { IsSuccess = false, Message = message, StatusCode = 404 };

    public static ServiceResponse<T> BadRequest(string message)
        => new() { IsSuccess = false, Message = message, StatusCode = 400 };
    public static ServiceResponse<T> InternalError(string message)
         => new() { IsSuccess = false, Message = message, StatusCode = 500 };
}

public class ServiceResponse : ServiceResponse<object>
{
    public new static ServiceResponse Success(string message = "Success")
        => new() { Message = message, IsSuccess = true, StatusCode = 200 };

    public new static ServiceResponse NotFound(string message = "Resource not found")
        => new() { IsSuccess = false, Message = message, StatusCode = 404 };

    public new static ServiceResponse BadRequest(string message)
        => new() { IsSuccess = false, Message = message, StatusCode = 400 };
    public new static ServiceResponse InternalError(string message)
        => new() { IsSuccess = false, Message = message, StatusCode = 500 };
}

