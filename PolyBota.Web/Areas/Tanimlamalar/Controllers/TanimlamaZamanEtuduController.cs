using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PolyBota.Web.Areas.Tanimlamalar.Models;
using PolyBota.Web.Controllers;

namespace PolyBota.Web.Areas.Tanimlamalar.Controllers
{
    public class TanimlamaZamanEtuduController : BaseController
    {
        // GET: Tanimlamalar/TanimlamaZamanEtudu
        public ActionResult ZeKategoriListe()
        {
            var model = new TanimlamaZeModel()
            {
                ZamanEtuduKategoris = _db.ZamanEtuduKategoris.OrderBy(a => a.KategoriAdi).ToList()
            };
            return View(model);
        }

        public PartialViewResult ZeKategoriEkleDuzenle(int id)
        {
            var model = new TanimlamaZeModel()
            {
              
            };

            if (id!=0)
            {
                model.ZamanEtuduKategori = _db.ZamanEtuduKategoris.Find(id);
            }
            return PartialView("_ZeKategoriEkleDuzenle", model);
        }

        [HttpPost]
        public ActionResult ZeKategoriEkleDuzenle(TanimlamaZeModel model)
        {
            var item = model.ZamanEtuduKategori;

            if (item.Id==0)
            {
                _db.ZamanEtuduKategoris.Add(item);
                _db.SaveChanges();
            }
            else
            {
                var editItem = _db.ZamanEtuduKategoris.Find(item.Id);

                editItem.KategoriAdi = item.KategoriAdi;
                _db.SaveChanges();
            }

            TempDataCreate(2);

            return RedirectToAction("ZeKategoriListe");
        }
        public ActionResult ZeKategoriSil(int id)
        {
            var editItem = _db.ZamanEtuduKategoris.Find(id);

            _db.ZamanEtuduKategoris.Remove(editItem);
            _db.SaveChanges();
            TempDataCreate(3);

            return RedirectToAction("ZeKategoriListe");
        }
    }
}