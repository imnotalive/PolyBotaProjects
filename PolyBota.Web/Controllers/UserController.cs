using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using PolyBota.DAL;

namespace PolyBota.Web.Controllers
{
    public class UserController : Controller
    {
        public void CookieKaydet(User User, DateTime tarihDateTime)
        {

            Response.Cookies.Remove("sua");
            string CookieName = "sua";
            HttpCookie cookie = HttpContext.Response.Cookies[CookieName] ?? new HttpCookie(CookieName);

            //HttpCookie cookie = new HttpCookie("Cookie");

            cookie.Value = User.Id.ToString();

            cookie.Expires = tarihDateTime;
            this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);



        }

        private POTA_BOTAEntities _db = new POTA_BOTAEntities();
        // GET: User
        public ActionResult Login()
        {
            return View(new User());
        }

        [HttpPost]
        public ActionResult Login(User model)
        {
            bool GirisSorunuVarmi = false;
            if (model.UserName != null && model.Password != null)
            {
                if (this._db.Users.Any(a => a.Password == model.Password && (a.UserName == model.UserName || a.PersonelCode == model.UserName)))
                {
                    var User = _db.Users.First(
                        a => a.Password == model.Password && (a.UserName == model.UserName || a.PersonelCode == model.UserName));
                    CookieKaydet(User, DateTime.Now.AddDays(2));
                }
                else
                {
                    GirisSorunuVarmi = true;
                }
            }
            else
            {
                GirisSorunuVarmi = true;
            }

            if (GirisSorunuVarmi)
            {
                TempData["Msg"] = "Giriş Bilgileriniz eksik ya da hatalı";
                TempData["class"] = "danger";
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        public ActionResult LogOut()
        {
            Session["kul"] = null;
            Session["userRoles"] = null;
            Session["UserHeaderItemList"] = null;
            Session["kulLink"] = null;

            var reqCookie = HttpContext.Request.Cookies["sua"];
            if (reqCookie != null)
            {

                int decodedUserId = Convert.ToInt32(reqCookie.Value);

                var User = this._db.Users.First(a => a.Id == decodedUserId);

                CookieKaydet(User, DateTime.Now.AddDays(-1));

            }


            return RedirectToAction("Login", "User");
        }

        public ActionResult LangChange(string id, string returnUrl)
        {
            if (id != null)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(id);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(id);
            }

            HttpCookie cookie = new HttpCookie("langPack");
            cookie.Value = id;
            Response.Cookies.Add(cookie);

            Session["lang"] = id;
            if (returnUrl == null)
            {
                return RedirectToAction("Login");
            }
            return this.Redirect(returnUrl);
        }

    }
}