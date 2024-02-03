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
    public class TalimatController : BaseController
    {
        // GET: Tanimlamalar/Talimat
        public ActionResult TalimatGrupTanimListe()
        {
            var model = new TanimlamaTalimatModel()
            {
                TalimatGrupTanims = _db.TalimatGrupTanims.OrderBy(a=>a.TalimatGrupTanimAdi).ToList()
            };
            return View(model);
        }

        public ActionResult TalimatGrupDetay(int id = 0)
        {
            var model = new TanimlamaTalimatModel()
            {
               
            };
            if (id!=0)
            {
                model.TalimatGrupTanim = _db.TalimatGrupTanims.Find(id);
                model.TalimatTanims = _db.TalimatTanims.Where(a => a.TalimatGrupTanimId == id)
                    .OrderBy(a => a.TalimatAciklama).ToList();

            }
            return View(model);
        }

        [HttpPost]
        public ActionResult TalimatGrupDetay(TanimlamaTalimatModel model)
        {
            var grup = model.TalimatGrupTanim;

            if (grup.TalimatGrupTanimAdi==null)
            {
                TempDataCreate(4);
            }
            else
            {
                if (grup.Id == 0)
                {
                    var yeniItem = new TalimatGrupTanim()
                    {
                        TalimatGrupTanimAdi = grup.TalimatGrupTanimAdi
                    };
                    _db.TalimatGrupTanims.Add(yeniItem);
                    _db.SaveChanges();
                    grup.Id = yeniItem.Id;
                }
                else
                {
                    var editItem = _db.TalimatGrupTanims.Find(grup.Id);
                    editItem.TalimatGrupTanimAdi = grup.TalimatGrupTanimAdi;
                    _db.SaveChanges();
                }
                TempDataCreate(2);
            }

            return RedirectToAction("TalimatGrupDetay", "Talimat", new {id = grup.Id});

        }


        public ActionResult TalimatGrupSil(int id)
        {
            var grup = _db.TalimatGrupTanims.Find(id);
            _db.TalimatGrupTanims.Remove(grup);
            _db.SaveChanges();

            var lst = _db.TalimatTanims.Where(a => a.TalimatGrupTanimId == id).ToList();

            _db.TalimatTanims.RemoveRange(lst);
            _db.SaveChanges();
            TempDataCreate(3);
            return RedirectToAction("TalimatGrupTanimListe");
        }

        public PartialViewResult TalimatEkleDuzenle(int id, int talimatId)
        {
            var model = new TanimlamaTalimatModel()
            {
                TalimatTanim = new TalimatTanim(){TalimatGrupTanimId = id}
            };
            if (talimatId!=0)
            {
                model.TalimatTanim = _db.TalimatTanims.Find(talimatId);
            }

            return PartialView("_TalimatEkleDuzenle", model);
        }

        [HttpPost]
        public ActionResult TalimatEkleDuzenle(TanimlamaTalimatModel model)
        {
            var talimat = model.TalimatTanim;

            if (talimat.TalimatAciklama==null)
            {
                TempDataCreate(4);
            }
            else
            {
                if (talimat.Id==0)
                {
                    var yeniItem = new TalimatTanim(){TalimatGrupTanimId = talimat.TalimatGrupTanimId, TalimatAciklama = talimat.TalimatAciklama};
                    _db.TalimatTanims.Add(yeniItem);
                    _db.SaveChanges();
                }
                else
                {
                    var editItem = _db.TalimatTanims.Find(talimat.Id);
                    editItem.TalimatAciklama = talimat.TalimatAciklama;
                    _db.SaveChanges();

                }
                TempDataCreate(2);
            }

            return RedirectToAction("TalimatGrupDetay", "Talimat", new { id = talimat.TalimatGrupTanimId });
        }


        public ActionResult TalimatSil(int id)
        {
            var item = _db.TalimatTanims.Find(id);

            var idd = item.TalimatGrupTanimId;

            _db.TalimatTanims.Remove(item);
            _db.SaveChanges();

            TempDataCreate(3);

            return RedirectToAction("TalimatGrupDetay", "Talimat", new { id = idd });
        }
    }
}