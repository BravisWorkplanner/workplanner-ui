using System.Net;
using API.Contracts;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace API;

public static class APIClientExtensions
{
    public static async Task<Result<T>> HandleHttpCallAsync<T>(
        this IAPIClient client,
        Func<Task<APIResponse<T>>> httpCallFunc,
        ILogger logger)
    {
        if (client == null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        try
        {
            var result = await httpCallFunc.Invoke();

            return new Result<T>(result.Result, result.StatusCode);
        }
        catch (ApiException<ProblemDetails> ex)
        {
            var error = ExtractErrorFromTypeException<T>(ex);
            logger.LogError(error);
            return new Result<T>(ex.StatusCode, ex.Response);
        }
        catch (ApiException ex)
        {
            logger.LogError(ex, ex.ToString());
            return new Result<T>(ex.StatusCode, ex.Response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.ToString());

            return new Result<T>((int)HttpStatusCode.InternalServerError, "Internal server error");
        }
    }

    private static string ExtractErrorFromTypeException<T>(ApiException<ProblemDetails> ex)
    {
        var error = ex.Result.Title;
        if (ex.Result.AdditionalProperties.TryGetValue("errors", out var errors))
        {
            var dict = JObject.FromObject(errors).ToObject<Dictionary<string, object>>();
            if (dict != null)
            {
                var validationErrors = string.Join(',', dict.Select(x => string.Concat(x.Key, ':', x.Value)));
                error = $"{error} Validation errors: {validationErrors}";
            }
        }

        return error;
    }
}