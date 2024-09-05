using KnowledgeMining.Models;

namespace KnowledgeMining.Common
{

    public static class ApiResponseBuilder
    {
        public class ApiResponse<T>
        {
            public bool IsSuccess { get; set; }
            public string Message { get; set; }
            public T Data { get; set; }
        }

        public static ApiResponse<T> Success<T>(T data, string message = "Operation successful")
        {
            return new ApiResponse<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> Failure<T>(string message, T data = default)
        {
            return new ApiResponse<T>
            {
                IsSuccess = false,
                Message = message,
                Data = data
            };
        }
    }
}
