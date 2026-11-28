using System.Net;

namespace Dara.Clients.Core.Api;

// a refused request of the http api: the status and the problem details of the server ("code", "rule", "detail")
public class DaraApiException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string? Code { get; }
    public string? Rule { get; }
    public string? Detail { get; }

    public DaraApiException(HttpStatusCode statusCode, string? code, string? rule, string? detail)
        : base($"{(int)statusCode} {code}: {detail ?? statusCode.ToString()}")
    {
        StatusCode = statusCode;
        Code = code;
        Rule = rule;
        Detail = detail;
    }
}
