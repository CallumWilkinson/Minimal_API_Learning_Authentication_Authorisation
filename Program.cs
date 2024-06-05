namespace Minimal_API_Learning_Authentication_Authorisation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.MapGet("/username", () =>
            {
                return "callum";
            }
                );

            app.MapGet("/login", (HttpContext ctx) =>
            {
                ctx.Response.Headers["set-cookie"] = "auth=usr:callum";
                return "ok";
            }
                );

            app.Run();
        }
    }
}
