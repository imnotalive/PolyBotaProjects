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
    public class BilesenController : BaseController
    {
        // GET: Tanimlamalar/Bilesen
        public ActionResult BilesenGrupListe()
        {
            var model = new TanimlamaBilesenModel()
            {
                BilesenCinsiGrups = _db.BilesenCinsiGrups.OrderBy(a=>a.BilesenCinsiGrupAdi).ToList()
            };

            return View(model);
        }

        public ActionResult BilesenCinsiGrupDetay(int id = 0)
        {
            var model = new TanimlamaBilesenModel()
            {
            };
            if (id!=0)
            {
                model.BilesenCinsis = _db.BilesenCinsis.Where(a => a.BilesenCinsiGrupId == id)
                    .OrderBy(a => a.BilesenCinsiAdi).ToList();
                model.BilesenCinsiGrup = _db.BilesenCinsiGrups.Find(id);

            }
            return View(model);
        }
        [HttpPost]
        public ActionResult BilesenCinsiGrupDetay(TanimlamaBilesenModel model)
        {
            var item = model.BilesenCinsiGrup;
            if (item.BilesenCinsiGrupAdi==null)
            {
                TempDataCreate(4);
            }
            else
            {
                if (item.Id==0)
                {
                    var yeniItem = new BilesenCinsiGrup()
                    {
                        BilesenCinsiGrupAdi = item.BilesenCinsiGrupAdi
                    };
                    _db.BilesenCinsiGrups.Add(yeniItem);
                    _db.SaveChanges();
                    item.Id = yeniItem.Id;
                }
                else
                {
                    var editItem = _db.BilesenCinsiGrups.Find(item.Id);
                    editItem.BilesenCinsiGrupAdi = item.BilesenCinsiGrupAdi;
                    _db.SaveChanges();
                }

                TempDataCreate(2);
            }
           
            return RedirectToAction("BilesenCinsiGrupDetay", "Bilesen", new { id = item.Id });
        }

        public PartialViewResult BilesenCinsiEkleDuzenle(int id, int itemId)
        {
            var model = new TanimlamaBilesenModel()
            {
                BilesenCinsi = new BilesenCinsi(){BilesenCinsiGrupId = id}
            };

            if (itemId!=0)
            {
                model.BilesenCinsi = _db.BilesenCinsis.Find(itemId);
            }

            return PartialView("_BilesenCinsiEkleDuzenle", model);
        }

        [HttpPost]
        public ActionResult BilesenCinsiEkleDuzenle(TanimlamaBilesenModel model)
        {
            var item = model.BilesenCinsi;

            if (item.BilesenCinsiAdi==null)
            {
                TempDataCreate(4);
            }
            else
            {
                if (item.Id==0)
                {
                    var yeniItem = new BilesenCinsi()
                    {
                        BilesenCinsiAdi = item.BilesenCinsiAdi,
                        BilesenCinsiGrupId = item.BilesenCinsiGrupId
                    };
                    _db.BilesenCinsis.Add(yeniItem);
                    _db.SaveChanges();
                }
                else
                {
                    var editItem = _db.BilesenCinsis.Find(item.Id);
                    editItem.BilesenCinsiAdi = item.BilesenCinsiAdi;
                    _db.SaveChanges();
                }

                TempDataCreate(2);
            }

            return RedirectToAction("BilesenCinsiGrupDetay", "Bilesen", new {id = item.BilesenCinsiGrupId});
        }


        public ActionResult BilesenCinsiSil(int id)
        {
            var item = _db.BilesenCinsis.Find(id);

            var idd = item.BilesenCinsiGrupId;

            _db.BilesenCinsis.Remove(item);
            _db.SaveChanges();
            return RedirectToAction("BilesenCinsiEkleDuzenle", "Bilesen", new { id = idd });
        }
    }
}