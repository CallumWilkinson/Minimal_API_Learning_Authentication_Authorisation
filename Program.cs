using Microsoft.AspNetCore.DataProtection;
using System.Runtime.Intrinsics.Arm;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;

namespace Minimal_API_Learning_Authentication_Authorisation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            //microsoft service to encrypt cookies
            builder.Services.AddDataProtection();
            builder.Services.AddHttpContextAccessor();
            //is this adding the authservice class i added below?
            builder.Services.AddScoped<AuthService>();
        

            var app = builder.Build();

            app.Use((ctx, next) =>
            {
                var idp = ctx.RequestServices.GetRequiredService<IDataProtectionProvider>();
                var protector = idp.CreateProtector("auth-cookie");

                var authCookie = ctx.Request.Headers.Cookie.FirstOrDefault(x => x.StartsWith("auth="));
                var protectedPayload = authCookie.Split("=").Last();
                var payload = protector.Unprotect(protectedPayload);
                var parts = payload.Split(":");
                var key = parts[0];
                var value = parts[1];

                

                return next();
            });

            app.MapGet("/username", (HttpContext ctx) =>

            {
                return ctx.User;
            });
            

            app.MapGet("/login", (AuthService auth) =>
            {
                auth.SignIn();
                return "ok";
            });

            app.Run();

        }


        public class AuthService
        {
            private readonly IDataProtectionProvider _idp;
            private readonly IHttpContextAccessor _accessor;

            public AuthService(IDataProtectionProvider idp, IHttpContextAccessor accessor)
            {
                _idp = idp;
                _accessor = accessor;
            }


            public void SignIn()
            {
                var protector = _idp.CreateProtector("auth-cookie");
                _accessor.HttpContext.Response.Headers["set-cookie"] = $"auth={protector.Protect("usr:callum")}";
            }
        }

        
    }
}
