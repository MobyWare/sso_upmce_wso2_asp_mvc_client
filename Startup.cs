using System;
using System.Threading.Tasks;
using System.Net;
using System.Linq;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Authentication.OpenIdConnect;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace aspnet_wso2
{
    
    public partial class Startup
    {
        
        public IConfiguration Configuration { get; set; }
        
        public Startup() {
            
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            Configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();
                        
        }
        
        
        
        public void ConfigureServices(IServiceCollection services) {
            
                       
            services.AddAuthentication(options => options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);
             
            // services.AddMvc();
            
        }
        
        
        public void Configure(IApplicationBuilder app) {

            var factory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            factory.AddConsole();
            factory.AddDebug();
                    
                    
              // Simple error page
            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    if (!context.Response.HasStarted)
                    {
                        context.Response.Clear();
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync(ex.ToString());
                    }
                    else
                    {
                        throw;
                    }
                }
            });

            app.UseIISPlatformHandler();

            app.UseCookieAuthentication(options => {
                
                options.AutomaticAuthenticate = true;
                options.AuthenticationScheme = "Cookies";
            });

            
            app.UseOpenIdConnectAuthentication( options => {
                
                options.CallbackPath = new PathString("/auth/oidc/callback");                
                options.ClientId = "ffQRSYQDDoyVjz7l44il3DNKiHIa";
                options.GetClaimsFromUserInfoEndpoint = true;
                options.ClientSecret = "z9xfhKSIDrB5X9T_Xb9MB2jfjeMa";
                options.Configuration = new OpenIdConnectConfiguration() {
                    AuthorizationEndpoint = "https://localhost:9443/oauth2/authorize",
                    TokenEndpoint = "https://localhost:9443/oauth2/token"
                };
                options.GetClaimsFromUserInfoEndpoint = true;
                options.ResponseType = OpenIdConnectResponseTypes.Code;               
                options.RequireHttpsMetadata = false;
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");
                options.TokenValidationParameters.ValidateLifetime = false;
                options.TokenValidationParameters.ValidateIssuer = false;
                options.Events = new OpenIdConnectEvents() {
                    
                    OnAuthorizationCodeReceived = (context) => {
                        
                        Console.WriteLine(context.AuthenticationTicket);
                        return Task.FromResult(0);   
                    }    
                    
                };
            //     options.Events = new OpenIdConnectEvents
            //     {
                
            //         OnAuthenticationFailed = (context) => {
                        
            //             Console.Write("test");
            //             context.HandleResponse(); 
                        
            //             return Task.FromResult(0);   
            //         }    
                    
            //     };
            });


            app.Run(async context =>
            {
                if (context.Request.Path.Equals("/signout"))
                {
                    await context.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync($"<html><body>Signing out {context.User.Identity.Name}<br>{Environment.NewLine}");
                    await context.Response.WriteAsync("<a href=\"/\">Sign In</a>");
                    await context.Response.WriteAsync($"</body></html>");
                    return;
                }

                if (!context.User.Identities.Any(identity => identity.IsAuthenticated))
                {
                    await context.Authentication.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "/" });
                    return;
                }

                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync($"<html><body>Hello Authenticated User {context.User.Identity.Name}<br>{Environment.NewLine}");
                foreach (var claim in context.User.Claims)
                {
                    await context.Response.WriteAsync($"{claim.Type}: {claim.Value}<br>{Environment.NewLine}");
                }
                await context.Response.WriteAsync("<a href=\"/signout\">Sign Out</a>");
                await context.Response.WriteAsync($"</body></html>");
            });

            //app.UseMvc();
            
            //ConfigureAuth(app);
            
        }

    
    
        public static void Main(string[] args) => Microsoft.AspNet.Hosting.WebApplication.Run<Startup>(args);
    
    
    }
    
}