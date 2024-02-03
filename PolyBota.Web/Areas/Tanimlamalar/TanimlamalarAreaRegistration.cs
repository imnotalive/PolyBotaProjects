using System.Web.Mvc;

namespace PolyBota.Web.Areas.Tanimlamalar
{
    public class TanimlamalarAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Tanimlamalar";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Tanimlamalar_default",
                "Tanimlamalar/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}