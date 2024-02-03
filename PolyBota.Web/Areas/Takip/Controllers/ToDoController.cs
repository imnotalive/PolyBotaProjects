using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PolyBota.Web.Controllers;

namespace PolyBota.Web.Areas.Takip.Controllers
{
    public class ToDoController : BaseController
    {
        // GET: Takip/ToDo
        public ActionResult Liste()
        {
            return View();
        }
    }
}