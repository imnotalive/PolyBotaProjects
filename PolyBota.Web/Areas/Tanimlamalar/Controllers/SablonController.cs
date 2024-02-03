using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PolyBota.DAL;
using PolyBota.Web.Areas.Tanimlamalar.Models;
using PolyBota.Web.Controllers;

namespace PolyBota.Web.Areas.Tanimlamalar.Controllers
{
    public class SablonController : BaseController
    {
        // GET: Tanimlamalar/Sablon

        public void SablonKirilimTemizle(int id)
        {

            var lst = _db.SablonGrupItems.Where(a => a.SablonGrupId == id).ToList();

            var kayitliBagliIdler = lst.Select(a => a.BagliOlduguId).ToList();
            kayitliBagliIdler = kayitliBagliIdler.Where(a => a != 0).ToList();
            var silinecekBagliIdler = new List<int>();

            foreach (var bagliId in kayitliBagliIdler)
            {
                if (lst.Count(a => a.Id == bagliId) == 0)
                {
                    silinecekBagliIdler.Add(bagliId);
                }
            }


            

            if (silinecekBagliIdler.Any())
            {
                var silListe = lst.Where(b => silinecekBagliIdler.Any(a => a == b.BagliOlduguId)).ToList();
                _db.SablonGrupItems.RemoveRange(silListe);
                _db.SaveChanges();



            }
        }

        #region Sablon Gru CRUD
        public ActionResult SablonGrupListe()
        {
            var model = new TanimlamaSablonModel()
            {
                SablonGrups = _db.SablonGrups.OrderBy(a => a.SablonGrupAdi).ToList()
            };
            return View(model);
        }
        public ActionResult SablonGrupEkleDuzenle(int id=0)
        {
            
               var model = new TanimlamaSablonModel()
            {
                KomponentTanimGrups = _db.KomponentTanimGrups.OrderBy(a => a.KomponentTanimGrupAdi).ToList(),
                KomponentTanims = _db.KomponentTanims.OrderBy(a => a.KomponentTanimAdi).ToList()
            };
            if (id != 0)
            {
                model.SablonGrup = _db.SablonGrups.Find(id);
                model.SablonGrupItems = _db.SablonGrupItems.Where(a => a.SablonGrupId == id).ToList();


            }

            return View(model);
        }

        [HttpPost]
        public ActionResult SablonGrupEkleDuzenle(TanimlamaSablonModel model)
        {
            var item = model.SablonGrup;

            if (item.SablonGrupAdi==null)
            {
                TempDataCreate(4);
            }
            else
            {
                if (item.Id == 0)
                {
                    var yeniItem = new SablonGrup()
                    {
                        SablonGrupAdi = item.SablonGrupAdi
                    };
                    _db.SablonGrups.Add(yeniItem);
                    _db.SaveChanges();

                    item.Id = yeniItem.Id;
                }
                else
                {
                    var editItem = _db.SablonGrups.Find(item.Id);
                    editItem.SablonGrupAdi = item.SablonGrupAdi;
                    _db.SaveChanges();
                }
                TempDataCreate(2);
            }
            return RedirectToAction("SablonGrupEkleDuzenle","Sablon",new{id=item.Id});
        }
        public ActionResult SablonGrupSil(int id)
        {
            var item = _db.SablonGrups.Find(id);
            var lst = _db.SablonGrupItems.Where(a => a.SablonGrupId == id).ToList();
            _db.SablonGrups.Remove(item);
            _db.SaveChanges();
            _db.SablonGrupItems.RemoveRange(lst);
            _db.SaveChanges();
            TempDataCreate(3);
            return RedirectToAction("SablonGrupListe");
        }
        #endregion

        #region Sablon Item CRUD

        public PartialViewResult SablonGrupItemEkleDuzenle(int id, int itemId, int state)
        {
            var model = new TanimlamaSablonModel()
            {
                KomponentTanimGrups = _db.KomponentTanimGrups.OrderBy(a => a.KomponentTanimGrupAdi).ToList(),
                KomponentTanims = _db.KomponentTanims.OrderBy(a => a.KomponentTanimAdi).ToList(),
                SablonGrupItem = new SablonGrupItem() { BagliOlduguId = itemId, SablonGrupId = id }
            };

            if (state != 0)
            {
                model.SablonGrupItem = _db.SablonGrupItems.Find(itemId);
            }


            return PartialView("_SablonGrupItemEkleDuzenle", model);
        }

        [HttpPost]
        public ActionResult SablonGrupItemEkleDuzenle(TanimlamaSablonModel model)
        {
            var item = model.SablonGrupItem;

            if (item.KomponentTanimId!=0)
            {
                if (item.Id == 0)
                {
                    var yeniItem = new SablonGrupItem()
                    {
                        BagliOlduguId = item.BagliOlduguId,
                        KomponentTanimId = item.KomponentTanimId,
                        SablonGrupId = item.SablonGrupId
                    };
                    _db.SablonGrupItems.Add(yeniItem);
                    _db.SaveChanges();
                }
                else
                {
                    var editItem = _db.SablonGrupItems.Find(item.Id);
                    editItem.KomponentTanimId = item.KomponentTanimId;
                    _db.SaveChanges();
                }
                TempDataCreate(2);
            }

            return RedirectToAction("SablonGrupEkleDuzenle", "Sablon", new {id = item.SablonGrupId});
        }

        public ActionResult SablonGrupItemSil(int id)
        {
            var item = _db.SablonGrupItems.Find(id);
            int dd = item.SablonGrupId;
            _db.SaveChanges();
            _db.SablonGrupItems.Remove(item);
            _db.SaveChanges();
            TempDataCreate(3);
            SablonKirilimTemizle(dd);
            return RedirectToAction("SablonGrupEkleDuzenle", "Sablon", new { id = dd });
        }
        #endregion

    }
}