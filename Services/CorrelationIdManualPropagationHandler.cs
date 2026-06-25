public class CorrelationIdManualPropagationHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CorrelationIdManualPropagationHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context != null)
        {
            // Try to extract incoming ID from Postman
            var incomingId = context.Request.Headers["X-Correlation-ID"].ToString();

            // Fallback generation logic if Postman leaves it empty!
            if (string.IsNullOrWhiteSpace(incomingId))
            {
                incomingId = Guid.NewGuid().ToString();
                // Attach it back to the current context so your gateway logs match
                context.Request.Headers["X-Correlation-ID"] = incomingId;
            }

            // Explicitly force insert the header on the outgoing client call
            request.Headers.Remove("X-Correlation-ID"); // Prevent duplicate keys
            request.Headers.TryAddWithoutValidation("X-Correlation-ID", incomingId);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
