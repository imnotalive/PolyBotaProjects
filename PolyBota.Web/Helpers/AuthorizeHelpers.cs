using PolyBota.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PolyBota.Web.Helpers
{
    public class CookieLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var user = new User();

            bool LoginStatus = false;
            string UyariMesaji = "";

            if (filterContext.HttpContext.Session["kul"] != null)
            {
                LoginStatus = true;
            }
            else
            {


                if (filterContext.HttpContext.Request.Cookies["sua"] != null)
                {
                    var req = filterContext.HttpContext.Request.Cookies["sua"];

                    if (req.Expires > DateTime.Now)
                    {
                        int id = Convert.ToInt32(req.Value);
                        var _db = new POTA_BOTAEntities();

                        if (_db.Users.Any(a => a.Id == id))
                        {
                            LoginStatus = true;
                            filterContext.HttpContext.Session["kul"] = _db.Users.First(a => a.Id == id);
                        }
                        else
                        {
                            UyariMesaji = "Giriş Yapınız";
                        }
                    }

                }
                else
                {
                    UyariMesaji = "Giriş Yapınız";
                }
            }


            if (LoginStatus)
            {
                if (filterContext.HttpContext.Session["unauthorize"] != null)
                {
                    if (filterContext.HttpContext.Session["unauthorize"] == "hopp")
                    {
                        filterContext.HttpContext.Session["hop"] = "yetkisiz";
                        filterContext.Result = new RedirectResult("/UserHome/Info");
                    }
                }
            }
            else
            {

                filterContext.Result = new RedirectResult("/User/Login");
            }
            base.OnActionExecuting(filterContext);
        }
    }
}