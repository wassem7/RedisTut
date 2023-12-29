using System.Net;

namespace Caching.Models;

public class ApiResponse
{
    public bool IsSuccess { get; set; }

    public HttpStatusCode HttpStatusCode { get; set; }

    public string SuccessMessage { get; set; } = string.Empty;

    public object Result { get; set; }

    public List<string> ErrorMessages { get; set; }
}
