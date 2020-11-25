using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Dessert.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dessert.Application.Behavior
{
    public class RequestLogger<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<TRequest> _logger;
        private readonly ICurrentUserService _currentUserService;

        public RequestLogger(ILogger<TRequest> logger, ICurrentUserService currentUserService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var timer = Stopwatch.StartNew();
            var requestName = typeof(TRequest).Name;
            var userName = _currentUserService.Email ?? "[Not logged in]";
            try
            {
                var response = await next();
                timer.Stop();
                var elapsed = timer.Elapsed.TotalMilliseconds;
                _logger.LogInformation(
                    $"Request: {{RequestName}} ({{UserName}}) responded in {{Elapsed:0.0000}} ms, Request: {{@Request}}",
                    requestName,
                    userName,
                    elapsed,
                    request);
                return response;
            }
            catch (Exception ex)
            {
                timer.Stop();
                var elapsed = timer.Elapsed.TotalMilliseconds;
                _logger.LogError(
                    $"Request Failed: {{RequestName}} ({{UserName}}) failed in {{Elapsed:0.0000}} ms, Request: {{@Request}}, Exception: {{@Exception}}",
                    requestName,
                    userName,
                    elapsed,
                    request,
                    ex);
                throw;
            }
        }
    }
}