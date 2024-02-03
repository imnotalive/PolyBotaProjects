using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PolyBota.Entities;
using PolyBota.Web.Areas.Takip.Models;
using PolyBota.Web.Controllers;

namespace PolyBota.Web.Areas.Takip.Controllers
{
    public class MerveController : BaseController
    {
        // GET: Takip/Merve
        public ActionResult Liste()
        {
            var model = new MerveModel();

            model.Users = _db.Users.OrderBy(a => a.UserName).ToList();

            foreach (var userr in model.Users)
            {
                model.DropItems.Add(new DropItem()
                {
                    Tanim= string.Format("{0} - {1}", userr.Id, userr.Name),
                    Id = userr.Id.ToString()
                });
            }

            return View(model);
        }

        public ActionResult Detay(int id)
        {
            var model = new MerveModel
            {
                User = _db.Users.Find(id)
            };

            return View(model);
        }
    }
}