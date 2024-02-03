using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PolyBota.DAL;
using PolyBota.Web.Areas.Tanimlamalar.Models;
using PolyBota.Web.Controllers;

namespace PolyBota.Web.Areas.Tanimlamalar.Controllers
{
    public class KomponentController : BaseController
    {
        public ActionResult KomponentTanimListe()
        {
            
            var model = new TanimlamaKomponentModel();
            model.KomponentTanimGrups = _db.KomponentTanimGrups.OrderBy(a => a.KomponentTanimGrupAdi).ToList();
            return View(model);
        }
        // GET: Tanimlamalar/Komponent

        #region Komponent Grup CRUD
        public ActionResult KomponentGrupTanimListe()
        {

            var model = new TanimlamaKomponentModel();
            model.KomponentTanimGrups = _db.KomponentTanimGrups.OrderBy(a => a.KomponentTanimGrupAdi).ToList();
            return View(model);
        }


        public ActionResult KomponentGrupDetay(int id = 0)
        {/*
           var lst = new List<KomponentTanim>();

            for (int i = 1; i < 501; i++)
            {

                var str = "";
                if (i < 100)
                {
                    str = "0";
                }
                if (i<10)
                {
                    str = "00";
                }

                str += i;
               

                lst.Add(new KomponentTanim()
                {
                    KomponentTanimGrupId = 3,
                    KomponentTanimAdi =  str,
                    SeciliMi = false
                });

            }

            _db.KomponentTanims.AddRange(lst);
            _db.SaveChanges();
            */
            var model = new TanimlamaKomponentModel()
            {

            };
            if (id != 0)
            {
                model.KomponentTanimGrup = _db.KomponentTanimGrups.Find(id);
                model.KomponentTanims = _db.KomponentTanims.Where(a => a.KomponentTanimGrupId == id)
                    .OrderBy(a => a.KomponentTanimAdi).ToList();
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult KomponentGrupDetay(TanimlamaKomponentModel model)
        {
            var grup = model.KomponentTanimGrup;

            if (grup.KomponentTanimGrupAdi == null)
            {
                TempDataCreate(4);
            }
            else
            {
                if (grup.Id == 0)
                {

                    var yeniItem = new KomponentTanimGrup()
                    {
                        KomponentTanimGrupAdi = grup.KomponentTanimGrupAdi
                    };
                    _db.KomponentTanimGrups.Add(yeniItem);
                    _db.SaveChanges();
                    grup.Id = yeniItem.Id;
                }
                else
                {
                    var editItem = _db.KomponentTanimGrups.Find(grup.Id);
                    editItem.KomponentTanimGrupAdi = grup.KomponentTanimGrupAdi;
                    _db.SaveChanges();

                }

                TempDataCreate(2);
            }

            return RedirectToAction("KomponentGrupDetay", "Komponent", new { id = grup.Id });
        }



        public ActionResult KomponentGrupSil(int id)
        {
            var grup = _db.KomponentTanimGrups.Find(id);
            _db.KomponentTanimGrups.Remove(grup);
            _db.SaveChanges();

            var lst = _db.KomponentTanims.Where(a => a.KomponentTanimGrupId == id).ToList();

            _db.KomponentTanims.RemoveRange(lst);
            _db.SaveChanges();
            TempDataCreate(3);
            return RedirectToAction("KomponentGrupTanimListe");
        }

        #endregion

        #region Komponent CRUD
        public PartialViewResult KomponentEkleDuzenle(int id = 0, int kompId = 0, int state = 0)
        {
            var model = new TanimlamaKomponentModel()
            {

            };

            if (state == 0)
            {
                // id altına yeni Item Eklenecek
                model.KomponentTanim = new KomponentTanim() { KomponentTanimGrupId = id };
            }
            else

            {
                // id düzenlenecek
                model.KomponentTanim = _db.KomponentTanims.Find(kompId);
            }
            return PartialView("_KomponentEkleDuzenle", model);
        }

        [HttpPost]
        public ActionResult KomponentEkleDuzenle(TanimlamaKomponentModel model)
        {
            var item = model.KomponentTanim;

            if (item.Id == 0)
            {
                _db.KomponentTanims.Add(item);
                _db.SaveChanges();
            }
            else
            {
                var editItem = _db.KomponentTanims.Find(item.Id);
                editItem.KomponentTanimAdi = item.KomponentTanimAdi;
                _db.SaveChanges();

            }

            TempDataCreate(2);

            return RedirectToAction("KomponentGrupDetay", "Komponent", new { id = item.KomponentTanimGrupId });
        }
        public ActionResult KomponentSil(int id)
        {
            var item = _db.KomponentTanims.Find(id);

            var idd = item.KomponentTanimGrupId;

            _db.KomponentTanims.Remove(item);
            _db.SaveChanges();

            TempDataCreate(3);

            return RedirectToAction("KomponentGrupDetay", "Komponent", new { id = idd });
        }


        #endregion

    }
}