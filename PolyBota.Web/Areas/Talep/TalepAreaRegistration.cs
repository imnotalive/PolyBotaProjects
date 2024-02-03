using System.Web.Mvc;

namespace PolyBota.Web.Areas.Talep
{
    public class TalepAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Talep";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Talep_default",
                "Talep/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}