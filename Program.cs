using Microsoft.AspNetCore.DataProtection;

namespace Minimal_API_Learning_Authentication_Authorisation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //microsoft service to encrypt cookies
            builder.Services.AddDataProtection();

            var app = builder.Build();

            app.MapGet("/username", (HttpContext ctx) =>
            {
                var authCookie = ctx.Request.Headers.Cookie.FirstOrDefault(x => x.StartsWith("auth="));
                var payload = authCookie.Split("=").Last();
                var parts = payload.Split(":");
                var key = parts[0];
                var value = parts[1];
                return value;
            }
                );
            

            app.MapGet("/login", (HttpContext ctx, IDataProtectionProvider idp) =>
            {
                var protector = idp.CreateProtector("auth-cookie");
                ctx.Response.Headers["set-cookie"] = "auth=usr:callum";
                return "ok";
            }
                );

            app.Run();
        }
    }
}
