using System.Web.Mvc;

namespace PolyBota.Web.Areas.Takip
{
    public class TakipAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Takip";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Takip_default",
                "Takip/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}