public class CorrelationIdDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CorrelationIdDelegatingHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context != null && context.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId))
        {
            request.Headers.Add("X-Correlation-ID", correlationId.ToString());
        }
        else
        {
            // Fallback: Generate a new one if missing
            request.Headers.Add("X-Correlation-ID", Guid.NewGuid().ToString());
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
