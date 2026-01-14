public static class ApiResponseHelper
{
    public static ApiResponse<T> Ok<T>(T content, string message = MessageResponse.SUCCESS)
        => new() { StatusCode = StatusCodeResponse.Success, Message = message, Content = content };

    public static ApiResponse<T> Created<T>(T content)
        => new() { StatusCode = StatusCodeResponse.Created, Message = MessageResponse.CREATE_SUCCESSFULLY, Content = content };

    public static ApiResponse<T> Updated<T>(T content)
        => new() { StatusCode = StatusCodeResponse.Success, Message = MessageResponse.UPDATE_SUCCESSFULLY, Content = content };

    public static ApiResponse<T> Deleted<T>()
        => new() { StatusCode = StatusCodeResponse.Success, Message = MessageResponse.DELETE_SUCCESSFULLY, Content = default };

    public static ApiResponse<T> NotFound<T>(string message = MessageResponse.NOT_FOUND)
        => new() { StatusCode = StatusCodeResponse.NotFound, Message = message, Content = default };

    public static ApiResponse<T> BadRequest<T>(string message = MessageResponse.BAD_REQUEST)
        => new() { StatusCode = StatusCodeResponse.BadRequest, Message = message, Content = default };

    public static ApiResponse<T> Forbidden<T>(string message = MessageResponse.FORBIDDEN)
        => new() { StatusCode = StatusCodeResponse.Forbidden, Message = message, Content = default };

    public static ApiResponse<T> Conflict<T>(string message = MessageResponse.CONFLICT)
        => new() { StatusCode = StatusCodeResponse.Conflict, Message = message, Content = default };

    public static ApiResponse<T> ServerError<T>(string message = MessageResponse.ERROR_IN_SERVER)
        => new() { StatusCode = StatusCodeResponse.Error, Message = message, Content = default };

    // Dành cho phân trang
    public static ApiResponse<PagedResult<T>> OkPaged<T>(PagedResult<T> pagedData, string message = MessageResponse.SUCCESS)
    {
        return new ApiResponse<PagedResult<T>> 
        { 
            StatusCode = StatusCodeResponse.Success, 
            Message = message, 
            Content = pagedData 
        };
    }
}