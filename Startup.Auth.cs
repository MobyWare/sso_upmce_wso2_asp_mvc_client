using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace aspnet_wso2
{
    
    public partial class Startup
    {
        
        public void ConfigureAuth(IApplicationBuilder app){
            
            app.UseCookieAuthentication(options => {
                
                options.AutomaticAuthenticate = true;   
                options.AutomaticChallenge = true;
                options.LoginPath = new PathString("/account/login");                        
                
            });            
                   
        }
        
    }
    
}