public class ApiResponse<T>
{
    public string StatusCode { get; set; } = string.Empty;
    public string? Message { get; set; }
    public T? Content { get; set; }
}
