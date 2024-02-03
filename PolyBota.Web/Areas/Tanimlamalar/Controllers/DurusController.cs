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
    public class DurusController : BaseController
    {
        // GET: Tanimlamalar/Durus
        public ActionResult DurusTanimListe(int id = 1)
        {
            var model = new TanimlamaDurusModel
            {
                Id = id,
                DurusGrubuTanims = _db.DurusGrubuTanims.ToList(),
                DurusTipiTanims = _db.DurusTipiTanims.Where(a => a.DurusGrubu == id).ToList()
            };
            return View(model);
        }

        public PartialViewResult DurusTipEkleDuzenle(int id, int ItemId)
        {
            var model = new TanimlamaDurusModel
            {

                DurusGrubuTanims = _db.DurusGrubuTanims.ToList(),
                DurusTipiTanim = new DurusTipiTanim() { DurusGrubu = id }
            };
            if (ItemId != 0)
            {
                model.DurusTipiTanim = _db.DurusTipiTanims.Find(ItemId);
            }

            return PartialView("_DurusTipEkleDuzenle", model);
        }
        [HttpPost]
        public ActionResult DurusTipEkleDuzenle(TanimlamaDurusModel model)
        {
            var item = model.DurusTipiTanim;

            if (item.DurusTipi==null)
            {
                TempDataCreate(4);
            }
            else
            {
                if (item.Id==0)
                {
                    var yeniItem = new DurusTipiTanim()
                    {
                        DurusGrubu = item.DurusGrubu,
                        DurusTipi = item.DurusTipi
                    };
                    _db.DurusTipiTanims.Add(yeniItem);
                    _db.SaveChanges();
                }
                else
                {
                    var editItem = _db.DurusTipiTanims.Find(item.Id);
                    editItem.DurusGrubu = item.DurusGrubu;
                    editItem.DurusTipi = item.DurusTipi;
                    _db.SaveChanges();
                }

                TempDataCreate(2);
            }
            return RedirectToAction("DurusTanimListe", "Durus", new {id = item.DurusGrubu});
        }



        public ActionResult DurusTipiSil(int id)
        {
         

            var editItem = _db.DurusTipiTanims.Find(id);
            var idd = editItem.DurusGrubu;

            _db.DurusTipiTanims.Remove(editItem);
            _db.SaveChanges();
            TempDataCreate(3);
            return RedirectToAction("DurusTanimListe", "Durus", new { id = idd });
        }
    }
}