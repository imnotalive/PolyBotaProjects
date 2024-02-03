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
    public class SicilOzellikController : BaseController
    {
        // GET: Tanimlamalar/SicilOzellik
     
        // GET: Tanimlamalar/SicilOzellik

        #region SicilOzellik Grup CRUD
        public ActionResult SicilOzellikGrupTanimListe()
        {

            var model = new TanimlamaSicilOzellikModel();
            model.SicilOzellikHeaderTanims = _db.SicilOzellikHeaderTanims.OrderBy(a => a.SicilOzellikHeaderTanimAdi).ToList();
            return View(model);
        }


        public ActionResult SicilOzellikGrupDetay(int id = 0)
        {
            var model = new TanimlamaSicilOzellikModel()
            {

            };
            if (id != 0)
            {
                model.SicilOzellikHeaderTanim = _db.SicilOzellikHeaderTanims.Find(id);
                model.SicilOzellikTanims = _db.SicilOzellikTanims.Where(a => a.SicilOzellikHeaderTanimId == id)
                    .OrderBy(a => a.SicilOzellikTanimAdi).ToList();
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult SicilOzellikGrupDetay(TanimlamaSicilOzellikModel model)
        {
            var grup = model.SicilOzellikHeaderTanim;

            if (grup.SicilOzellikHeaderTanimAdi == null)
            {
                TempDataCreate(4);
            }
            else
            {
                if (grup.Id == 0)
                {

                    var yeniItem = new SicilOzellikHeaderTanim()
                    {
                        SicilOzellikHeaderTanimAdi = grup.SicilOzellikHeaderTanimAdi
                    };
                    _db.SicilOzellikHeaderTanims.Add(yeniItem);
                    _db.SaveChanges();
                    grup.Id = yeniItem.Id;
                }
                else
                {
                    var editItem = _db.SicilOzellikHeaderTanims.Find(grup.Id);
                    editItem.SicilOzellikHeaderTanimAdi = grup.SicilOzellikHeaderTanimAdi;
                    _db.SaveChanges();

                }

                TempDataCreate(2);
            }

            return RedirectToAction("SicilOzellikGrupDetay", "SicilOzellik", new { id = grup.Id });
        }



        public ActionResult SicilOzellikGrupSil(int id)
        {
            var grup = _db.SicilOzellikHeaderTanims.Find(id);
            _db.SicilOzellikHeaderTanims.Remove(grup);
            _db.SaveChanges();

            var lst = _db.SicilOzellikTanims.Where(a => a.SicilOzellikHeaderTanimId == id).ToList();

            _db.SicilOzellikTanims.RemoveRange(lst);
            _db.SaveChanges();
            TempDataCreate(3);
            return RedirectToAction("SicilOzellikGrupTanimListe");
        }

        #endregion

        #region SicilOzellik CRUD
        public PartialViewResult SicilOzellikEkleDuzenle(int id = 0, int kompId = 0, int state = 0)
        {
            var model = new TanimlamaSicilOzellikModel()
            {

            };
            if (state == 0)
            {
                // id altına yeni Item Eklenecek
                model.SicilOzellikTanim = new SicilOzellikTanim() { SicilOzellikHeaderTanimId = id };
            }
            else

            {
                // id düzenlenecek
                model.SicilOzellikTanim = _db.SicilOzellikTanims.Find(kompId);
            }
            return PartialView("_SicilOzellikEkleDuzenle", model);
        }

        [HttpPost]
        public ActionResult SicilOzellikEkleDuzenle(TanimlamaSicilOzellikModel model)
        {
            var item = model.SicilOzellikTanim;

            if (item.Id == 0)
            {
                _db.SicilOzellikTanims.Add(item);
                _db.SaveChanges();
            }
            else
            {
                var editItem = _db.SicilOzellikTanims.Find(item.Id);
                editItem.SicilOzellikTanimAdi = item.SicilOzellikTanimAdi;
                _db.SaveChanges();

            }

            TempDataCreate(2);

            return RedirectToAction("SicilOzellikGrupDetay", "SicilOzellik", new { id = item.SicilOzellikHeaderTanimId });
        }
        public ActionResult SicilOzellikSil(int id)
        {
            var item = _db.SicilOzellikTanims.Find(id);

            var idd = item.SicilOzellikHeaderTanimId;

            _db.SicilOzellikTanims.Remove(item);
            _db.SaveChanges();

            TempDataCreate(3);

            return RedirectToAction("SicilOzellikGrupDetay", "SicilOzellik", new { id = idd });
        }


        #endregion
    }
}