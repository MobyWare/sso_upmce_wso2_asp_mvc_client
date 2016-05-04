using Microsoft.AspNet.Mvc;


namespace aspnet_wso2
{
    
    public class HomeController: Controller
    {
        [HttpGetAttribute("~/")]
        public ActionResult Index() {
            
            return View("Index");
        }
    }
}