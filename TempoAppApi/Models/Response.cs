namespace TempoAppApi.Models;

public class Response<TData> where TData : class 
{
    public string? Message { get; set; }
    public bool Succeed { get; set; }
    public TData? Data { get; set; }
    public string[]? Errors { get; set; }

    public Response(string message, TData? data)
    {
        Message = message;
        Data = data;
        Succeed = true;
    }

    public Response(string message, string[] errors)
    {
        Succeed = false;
        Message = message;
        Errors = errors;
    }

    public Response(string? message)
    {
        Succeed = false;
        Message = message;
    }
}