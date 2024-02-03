using System.Web.Mvc;

namespace PolyBota.Web.Areas.Veritabani
{
    public class VeritabaniAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Veritabani";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Veritabani_default",
                "Veritabani/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}