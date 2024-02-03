using System.Web.Mvc;

namespace PolyBota.Web.Areas.Raporlama
{
    public class RaporlamaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Raporlama";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Raporlama_default",
                "Raporlama/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}