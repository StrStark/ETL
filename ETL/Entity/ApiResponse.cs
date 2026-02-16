using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ETL.Entity;

public class ApiResponse<T>
{
    public HttpStatusCode Code { get; set; }
    public string Status { get; set; } = "success";
    public string Message { get; set; } = string.Empty;
    public T? Body { get; set; }

    public static ApiResponse<T> Success(string message , HttpStatusCode Code , T? body = default)
    {
        return new ApiResponse<T>
        {
            Status = "success",
            Message = message,
            Body = body,
            Code = Code
        };
    }

    public static ApiResponse<T> Fail(string message, HttpStatusCode Code , T? body = default)
    {
        return new ApiResponse<T>
        {
            Status = "error",
            Message = message,
            Body = body,
            Code = Code
        };
    }
}
public class ApiResponse
{
    public HttpStatusCode Code { get; set; }
    public string Status { get; set; } = "success";
    public string Message { get; set; } = string.Empty;

    public static ApiResponse Success( string message  , HttpStatusCode Code)
    {
        return new ApiResponse
        {
            Status = "success",
            Message = message,
            Code = Code
        };
    }

    public static ApiResponse Fail(string message, HttpStatusCode Code)
    {
        return new ApiResponse 
        {
            Status = "error",
            Message = message,
            Code = Code
        };
    }
}