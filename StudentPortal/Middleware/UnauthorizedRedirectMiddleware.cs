namespace StudentPortal.Middleware
{
    public class UnauthorizedRedirectMiddleware
    {
        private readonly RequestDelegate _next;

        public UnauthorizedRedirectMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); //  wrap in try/catch
            }
            catch (UnauthorizedAccessException)
            {
                //  catch exception → redirect to login
                var returnUrl = context.Request.Path + context.Request.QueryString;

                context.Response.Clear();
                context.Response.Redirect(
                    $"/Auth/Login");
                return;
            }

            //  also handle 401 status code (for non-exception 401s)
            if (!context.Response.HasStarted &&
                context.Response.StatusCode == 401 &&
                !context.Request.Path.StartsWithSegments("/api"))
            {
                var returnUrl = context.Request.Path + context.Request.QueryString;
                context.Response.Redirect(
                    $"/Auth/Login");
            }
        }
    }
}