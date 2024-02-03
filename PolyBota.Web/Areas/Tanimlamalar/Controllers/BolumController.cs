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
    public class BolumController : BaseController
    {
        // GET: Tanimlamalar/Bolum

        public void DepartmanRec(int id)
        {
            if (id!=0)
            {
                var item = _db.Departmen.Find(id);

                Departmans.Add(item);

                DepartmanRec(item.BagliOlduguId);
            }
        }
        public List<Departman> Departmans { get; set; }
        public ActionResult BolumListe(int id)
        {
            Departmans = new List<Departman>();
            DepartmanRec(id);

            Departmans.Reverse();
            var model = new TanimlamaBolumModel
            {
                Departman = _db.Departmen.Find(id),
                Bolums = _db.Bolums.Where(a=>a.DepartmanId==id).ToList(),
                Departmans = Departmans
            };
            return View(model);
        }

        public PartialViewResult BolumEkleDuzenle(int id, int bolumId, int state)
        {
            var model = new TanimlamaBolumModel
            {
                Bolum = new Bolum(){DepartmanId = id, BagliOlduguId = bolumId}
            };
            if (state==1)
            {
                model.Bolum = _db.Bolums.Find(bolumId);
            }

            return PartialView("_BolumEkleDuzenle", model);
        }
        [HttpPost]
        public ActionResult BolumEkleDuzenle(TanimlamaBolumModel model)
        {
            var item = model.Bolum;

            if (item.Id == 0)
            {
                _db.Bolums.Add(item);
                _db.SaveChanges();
            }
            else
            {
                var editItem = _db.Bolums.Find(item.Id);
                editItem.BolumAdi = item.BolumAdi;
                _db.SaveChanges();

            }

            TempDataCreate(2);
            return RedirectToAction("BolumListe","Bolum",new{id=item.DepartmanId});
        }

        public ActionResult BolumSil(int id)
        {
            var item = _db.Bolums.Find(id);
            int idd = item.DepartmanId;
            var lst = _db.Bolums.Where(a => a.BagliOlduguId == id).ToList();
            lst.Add(item);
            _db.Bolums.RemoveRange(lst);
            _db.SaveChanges();

            TempDataCreate(3);

            return RedirectToAction("BolumListe","Bolum",new{id= idd });
        }
    }
}