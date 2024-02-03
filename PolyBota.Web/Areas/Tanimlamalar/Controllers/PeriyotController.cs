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
    public class PeriyotController : BaseController
    {
        // GET: Tanimlamalar/Periyot
        public ActionResult PeriyotTanimListe(int id=1)
        {
            var model = new TanimlamaPeriyotModel()
            {
                Id = id,
                PeriyotTipiTanims = _db.PeriyotTipiTanims.ToList(),
                PeriyotTanims = _db.PeriyotTanims.Where(a=>a.PeriyotTipi== id).ToList()
            };
            return View(model);
        }

        public PartialViewResult PeriyotTanimEkleDuzenle(int id)
        {
            var model = new TanimlamaPeriyotModel()
            {
                Id = id,
                PeriyotTipiTanims = _db.PeriyotTipiTanims.ToList(),
               PeriyotTanim = new PeriyotTanim{PeriyotTipi = 1, PeriyotDonemi = 6, BildirimDonemi = 3}
            };

            if (id != 0)
            {
                model.PeriyotTanim = _db.PeriyotTanims.Find(id);
            }

            if (string.IsNullOrEmpty(model.PeriyotTanim.RenkSecimi))
            {
                model.PeriyotTanim.RenkSecimi = "#26B99A";
            }
            return PartialView("_PeriyotTanimEkleDuzenle", model);
        }

        [HttpPost]
        public ActionResult PeriyotTanimEkleDuzenle(TanimlamaPeriyotModel model)
        {
            var item = model.PeriyotTanim;

            if (item.PeriyotTanimAdi == null)
            {
                TempDataCustom(1,"Lütfen Bilgileri Kontrol Ediniz");
            }
            else
            {
                if (item.Id==0)
                {
                    var yeniItem = new PeriyotTanim()
                    {
                        PeriyotTipi = item.PeriyotTipi,
                        PeriyotTanimAdi = item.PeriyotTanimAdi,
                        PeriyotDonemi = item.PeriyotDonemi,
                        RenkSecimi = item.RenkSecimi,
                        BildirimDonemi = item.BildirimDonemi
                    };
                    _db.PeriyotTanims.Add(yeniItem);
                    _db.SaveChanges();
                }
                else
                {
                    var editItem = _db.PeriyotTanims.Find(item.Id);
                    editItem.PeriyotTipi = item.PeriyotTipi;
                    editItem.PeriyotTanimAdi = item.PeriyotTanimAdi;
                    editItem.PeriyotDonemi = item.PeriyotDonemi;
                    editItem.RenkSecimi = item.RenkSecimi;
                    editItem.BildirimDonemi = item.BildirimDonemi;
                    _db.SaveChanges();
                }
                TempDataCreate(2);
            }

            return RedirectToAction("PeriyotTanimListe", "Periyot", new {id = item.PeriyotTipi});
        }


        public ActionResult PeriyotTanimSil(int id)
        {
            var item = _db.PeriyotTanims.Find(id);

            var idd = item.PeriyotTipi;

            _db.PeriyotTanims.Remove(item);
            _db.SaveChanges();
            TempDataCreate(3);

            return RedirectToAction("PeriyotTanimListe", "Periyot", new { id = idd });
        }
    }
}