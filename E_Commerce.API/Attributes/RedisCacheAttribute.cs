using E_Commerce.Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace E_Commerce.API.Attributes
{
    public class RedisCacheAttribute : ActionFilterAttribute
    {
        private readonly int _durationInSeconds;
        public RedisCacheAttribute(int durationInSeconds)
        {
            _durationInSeconds = durationInSeconds;
        }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Get Cache Service From DI Container
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
            var cacheKey = CreateCacheKey(context.HttpContext.Request);

            // Check If Cache Data Exists 
            var cached = await cacheService.GetAsync(cacheKey);

            // If Exists, Return Cached Data and Skip Executing End point
            if(!string.IsNullOrEmpty(cached))
            {
                context.Result = new ContentResult()
                {
                    Content = cached,
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status200OK
                };
                return;
            }
            // If Not Exists, Execute End point and Cache the Result if 200 ok response
            var executed = await next.Invoke();
            if(executed.Result is OkObjectResult { Value : not null} ok)
                await cacheService.SetAsync(cacheKey, ok.Value, TimeSpan.FromSeconds(_durationInSeconds));
            return;
        }
        private static string CreateCacheKey(HttpRequest request)
        {
            //Path 

            var key = new StringBuilder();
            key.Append(request.Path).Append("?");

            foreach (var (k, v) in request.Query.OrderBy(x => x.Key))
            {
                key.Append(k).Append("=").Append(v).Append("&");
            }
            return key.ToString();
        }
    }
}
