using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PolyBota.DAL;
using PolyBota.Web.Areas.Tanimlamalar.Models;
using PolyBota.Web.Controllers;

namespace PolyBota.Web.Areas.Tanimlamalar.Controllers
{
    public class DepartmanController : BaseController
    {
        // GET: Tanimlamalar/Departman
        public ActionResult DepartmanListe()
        {
            var model = new TanimlamaDepartmanModel()
            {
                Departmans = _db.Departmen.OrderBy(a=>a.BagliOlduguId).ThenBy(a=>a.DepartmanAdi).ToList()
            };
            return View(model);
        }

        public PartialViewResult DepartmanEkleDuzenle(int id = 0,  int state=0)
        {
            var model = new TanimlamaDepartmanModel()
            {
            
            };
            if (state==0)
            {
                // id altına yeni Item Eklenecek
                model.Departman = new Departman(){BagliOlduguId = id};
            }
            else
          
            {
                // id düzenlenecek
                model.Departman = _db.Departmen.Find(id);
            }
            return PartialView("_DepartmanEkleDuzenle",model);
        }

        [HttpPost]
        public ActionResult DepartmanEkleDuzenle(TanimlamaDepartmanModel model)
        {
            var item = model.Departman;

            if (item.Id==0)
            {
                _db.Departmen.Add(item);
                _db.SaveChanges();
            }
            else
            {
                var editItem = _db.Departmen.Find(item.Id);
                editItem.DepartmanAdi = item.DepartmanAdi;
                _db.SaveChanges();

            }

            TempDataCreate(2);
            return RedirectToAction("DepartmanListe");
        }

        public ActionResult DepartmanSil(int id)
        {
            var item = _db.Departmen.Find(id);

            var lst = _db.Departmen.Where(a => a.BagliOlduguId == id).ToList();
            lst.Add(item);
            _db.Departmen.RemoveRange(lst);
            _db.SaveChanges();

            TempDataCreate(3);

            return RedirectToAction("DepartmanListe");
        }
    }
}