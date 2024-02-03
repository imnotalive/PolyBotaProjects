using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PolyBota.DAL;
using PolyBota.Entities;
using PolyBota.Web.Areas.Tanimlamalar.Models;
using PolyBota.Web.Controllers;

namespace PolyBota.Web.Areas.Tanimlamalar.Controllers
{
    public class TanimlamaTechizatTuruController : BaseController
    {
        // GET: Tanimlamalar/TanimlamaTechizatTuru

        public List<DropItem> DropListe { get; set; }

        public void RecTechizatlariOlustur(int id)
        {
            var lst = _db.TechizatTuruTanims.Where(a => a.BagliOlduguId == id).OrderBy(a => a.TechizatTuruTanimAdi)
                .ToList();
            
            if (lst.Any())
            {
                foreach (var item in lst)
                {
                    DropListe.Add(new DropItem()
                    {
                        Id = item.Id.ToString(),
                        Tanim = item.TechizatTuruTanimAdi
                    });
                    RecTechizatlariOlustur(item.Id);
                }
            }
        }
        public ActionResult TechizatTuruListe()
        {

            var model = new TanimlamaTechizatTuruModel()
            {
                DropTechizatTuruTanims = DropTechizatKirilimlar()
            };
            return View(model);
        }
        public ActionResult TechizatTuruDetay(int id=0)
        {

            var model = new TanimlamaTechizatTuruModel()
            {
             
              TechizatTuruTanim = id==0 ? new TechizatTuruTanim() : _db.TechizatTuruTanims.Find(id)
            };
            model.DropTechizatTuruTanims.Add(new DropItem() { Id = "0", Tanim = "Ana Tanım" });
            var techizatTurleri = _db.TechizatTuruTanims.Where(a => a.Id != id).ToList();

            foreach (var item in techizatTurleri)
            {
                var parentListe = DropKoktenTechizatlariniGetir(item.Id);
                if (parentListe.Any())
                {
                    var str = "";

                    foreach (var dropItem in parentListe)
                    {
                        str += dropItem.Tanim;
                        if (dropItem != parentListe.Last())
                        {
                            str += " >> ";
                        }
                    }

                    model.DropTechizatTuruTanims.Add(new DropItem()
                    {
                        IdInt = item.Id,
                        Tanim = str,
                        Id = item.Id.ToString(),
                        BagliOlduguId = item.BagliOlduguId
                    });
                }
            }

         
           
            var BilesenHeaders = _db.BilesenHeaders.AsNoTracking().OrderBy(a => a.BilesenHeaderAdi).ToList();

            var kayitliBilesnHeaderlar = _db.TechizatTuruBilesenHeaders.Where(a=>a.TechizatTuruTanimId==id).ToList();

            foreach (var bilesenHeader in BilesenHeaders)
            {

                bool state = kayitliBilesnHeaderlar.Any(a => a.BilesenHeaderId == bilesenHeader.Id);

                model.BilesenHeaders.Add(new BilesenHeader()
                {
                    SeciliMi = state,
                    BilesenHeaderAdi = bilesenHeader.BilesenHeaderAdi,
                    Id = bilesenHeader.Id
                });
            }

            model.DropTechizatTuruTanims = model.DropTechizatTuruTanims.OrderBy(a => a.Tanim).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult TechizatTuruDetay(TanimlamaTechizatTuruModel model)
        {

            var item = model.TechizatTuruTanim;

            if (string.IsNullOrWhiteSpace(item.TechizatTuruTanimAdi))
            {
                TempDataCreate(4);
            }
            else
            {
                if (item.Id==0)
                {
                    _db.TechizatTuruTanims.Add(item);
                    _db.SaveChanges();

                }
                else
                {
                    var editItem = _db.TechizatTuruTanims.Find(item.Id);
                    editItem.TechizatTuruTanimAdi = item.TechizatTuruTanimAdi;
                    editItem.BagliOlduguId = item.BagliOlduguId;
                    _db.SaveChanges();
                }

                var guncelBilesenHeaderIdler = model.BilesenHeaders.Where(a => a.SeciliMi).Select(a => a.Id).ToList();


                var kayitliBilesenler = _db.TechizatTuruBilesenHeaders.Where(a => a.TechizatTuruTanimId == item.Id)
                    .ToList();
                var silinecekListe = new List<TechizatTuruBilesenHeader>();
                var eklenecekListe = new List<TechizatTuruBilesenHeader>();
                foreach (var itt in kayitliBilesenler)
                {
                    if (guncelBilesenHeaderIdler.Count(a=>a==itt.BilesenHeaderId)==0)
                    {
                        silinecekListe.Add(itt);
                    }   
                }
                foreach (var i in guncelBilesenHeaderIdler)
                {
                    if (kayitliBilesenler.Count(a => a.BilesenHeaderId == i) == 0)
                    {
                        eklenecekListe.Add(new TechizatTuruBilesenHeader()
                        {
                            BilesenHeaderId = i,
                            TechizatTuruTanimId = item.Id
                        });
                    }
                }

                if (eklenecekListe.Any())
                {
                    _db.TechizatTuruBilesenHeaders.AddRange(eklenecekListe);
                    _db.SaveChanges();
                }
                if (silinecekListe.Any())
                {
                    _db.TechizatTuruBilesenHeaders.RemoveRange(silinecekListe);
                    _db.SaveChanges();
                }
                TempDataCreate(2);
            }


            return RedirectToAction("TechizatTuruDetay", "TanimlamaTechizatTuru", new {id = item.Id});
        }

        public ActionResult TechizatDetaySil(int id)
        {
            var item = _db.TechizatTuruTanims.Find(id);
            _db.TechizatTuruTanims.Remove(item);
            _db.SaveChanges();
            TempDataCreate(3);

            return RedirectToAction("TechizatTuruListe");
        }
}
}