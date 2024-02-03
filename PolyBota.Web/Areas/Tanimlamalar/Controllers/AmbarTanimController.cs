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
    public class AmbarTanimController : BaseController
    {
        // GET: Tanimlamalar/AmbarTanim
        public ActionResult AmbarListe(int id=0)
        {

            if (_db.ViewEksikAmbarListes.Any())
            {
                var lst = _db.ViewEksikAmbarListes.AsNoTracking().ToList();

                var eklenecekListe = new List<Ambar>();
                foreach (var item in lst)
                {
                    eklenecekListe.Add(new Ambar()
                    {
                        AmbarTipi = 0,
                        AmbarAdi = item.AmbarAdi,
                        PotaAmbarNo = item.AmbarNo
                    });
                }

                _db.Ambars.AddRange(eklenecekListe);
                _db.SaveChanges();
            }


            var model = new TanimlamaAmbarModel()
            {
                Id = id,
                Ambars = _db.Ambars.Where(a=>a.AmbarTipi==id).OrderBy(a=>a.AmbarAdi).ToList(),
                AmbarTipiTanims = _db.AmbarTipiTanims.ToList()
            };
            return View(model);
        }


        public PartialViewResult AmbarTanimEkleDuzenle(int id)
        {
            var model = new TanimlamaAmbarModel()
            {
                Id = id,
               Ambar = new Ambar(){AmbarTipi = 1}
            };

            if (id != 0)
            {
                model.Ambar = _db.Ambars.Find(id);
            }

            return PartialView("_AmbarTanimEkleDuzenle", model);
        }

        [HttpPost]
        public ActionResult AmbarTanimEkleDuzenle(TanimlamaAmbarModel model)
        {
            var item = model.Ambar;

            if (item.AmbarAdi == null)
            {
                TempDataCustom(1, "Lütfen Bilgileri Kontrol Ediniz");
            }
            else
            {
                if (item.Id == 0)
                {
                    var yeniItem = new Ambar()
                    {
                        AmbarTipi = 1,
                        PotaAmbarNo = "",
                        AmbarAdi = item.AmbarAdi
                    };
                    _db.Ambars.Add(yeniItem);
                    _db.SaveChanges();
                }
                else
                {
                    var editItem = _db.Ambars.Find(item.Id);
                    editItem.AmbarAdi = item.AmbarAdi;
                    _db.SaveChanges();
                }
                TempDataCreate(2);
            }

            return RedirectToAction("AmbarListe", "AmbarTanim", new { id = item.AmbarTipi });
        }


        public ActionResult AmbarTanimSil(int id)
        {
            var item = _db.Ambars.Find(id);

            var idd = item.AmbarTipi;

            _db.Ambars.Remove(item);
            _db.SaveChanges();
            TempDataCreate(3);

            return RedirectToAction("AmbarListe", "AmbarTanim", new { id = idd});
        }
    }
}