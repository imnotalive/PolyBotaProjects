using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PolyBota.DAL;
using PolyBota.Entities;
using PolyBota.Web.Areas.Takip.Models;
using PolyBota.Web.Controllers;

namespace PolyBota.Web.Areas.Takip.Controllers
{
    public class MakineController : BaseController
    {
        // GET: Takip/Makine



        public void BreadCrumbOlustur(int id)
        {
            if (id != 0)
            {
                var dept = _db.Departmen.Find(id);
                BreadCrumbs.Add(new DropItem()
                {
                    Tanim = dept.DepartmanAdi,
                    Id = dept.Id.ToString(),
                    IdInt = dept.Id
                });
                BreadCrumbOlustur(dept.BagliOlduguId);
            }




        }
        public List<DropItem> BreadCrumbs { get; set; }
        public ActionResult MakineListe(int id = 0)
        {
            var model = new MakineTakipModel()
            {
                Id = id,
                Departmans = _db.Departmen.Where(a => a.BagliOlduguId == id).OrderBy(a => a.DepartmanAdi).ToList()
            };
            BreadCrumbs = new List<DropItem>();



            if (id != 0)
            {
                model.Bolums = _db.Bolums.Where(a => a.DepartmanId == id).OrderBy(a => a.BolumAdi).ToList();
                BreadCrumbOlustur(id);
            }
            BreadCrumbs.Add(new DropItem() { Tanim = "Anasayfa", Id = "0", IdInt = 0 });
            BreadCrumbs.Reverse();

            model.DropBreadCrumbs = BreadCrumbs;

            return View(model);
        }


        public PartialViewResult OperasyonOzet(int id)
        {
            var model = new MakineTakipModel
            {
                StokKart = new StokKart(),
                KomponentTalimat = new KomponentTalimat(),
                StokKartOperasyon = new StokKartOperasyon(),
                PeriyotTanim = new PeriyotTanim()
            };
            var operasyon = _db.StokKartOperasyons.Find(id);
            var kompTalimat = _db.KomponentTalimats.Find(operasyon.KomponentTalimatGrupId);
            var periyotTanim = _db.PeriyotTanims.Find(kompTalimat.TalimatTanimId);
            var stokKart = _db.StokKarts.Find(operasyon.StokKartId);
            model.StokKart = stokKart;
            model.KomponentTalimat = kompTalimat;
            model.StokKartOperasyon = operasyon;
            model.PeriyotTanim = periyotTanim;
            return PartialView("_OperasyonOzet", model);

        }
        public ActionResult MakineBolumListe(int id)
        {
            TempData["theme"] = "gizle";
            var model = new MakineTakipModel()
            {
                Id = id,

                Bolum = _db.Bolums.Find(id)
            };
            BreadCrumbs = new List<DropItem>();

            model.PeriyotTanims = _db.PeriyotTanims.OrderBy(a => a.PeriyotTanimAdi).ToList();
            model.PeriyotTipiTanims = _db.PeriyotTipiTanims.OrderBy(a => a.PeriyotTipiAdi).ToList();

            foreach (var itt in model.PeriyotTanims)
            {
                itt.SeciliMi = true;
            }

            BreadCrumbOlustur(model.Bolum.DepartmanId);

            BreadCrumbs.Add(new DropItem() { Tanim = "Anasayfa", Id = "0", IdInt = 0 });
            BreadCrumbs.Reverse();
            model.DropBreadCrumbs = BreadCrumbs;

            model.StokKarts = _db.StokKarts.Where(a => a.BolumId == id).OrderBy(a => a.StokTanimAdi).ToList();


            return View(model);
        }

        [HttpPost]

        public PartialViewResult MakineBolumAnaliz(MakineTakipModel model)
        {
           

            var seciliPeriyotlar = model.PeriyotTanims.Where(a => a.SeciliMi).Select(a => a.Id).ToList();
            model.PeriyotTanims = _db.PeriyotTanims.OrderBy(a => a.PeriyotTanimAdi).ToList();
            model.PeriyotTipiTanims = _db.PeriyotTipiTanims.OrderBy(a => a.PeriyotTipiAdi).ToList();

            var BaslangisTarihi = model.BaslangisTarihi;

            var BitisTarihi = model.BitisTarihi;
            var tableHeaders = DropTakvimTableHeader(BaslangisTarihi, BitisTarihi, model.GosterimSekli);

            model.TableHeaderlar = tableHeaders;
            model.TakipTakvimModels = new List<List<TakipTakvimModel>>();
            model.KomponentTalimatGrups = new List<KomponentTalimatGrup>();
            model.DurusTipiTanims = new List<DurusTipiTanim>();

            model.DurusTipiTanims = _db.DurusTipiTanims.ToList();
            var takvimModel = new List<List<TakipTakvimModel>>();

            var secilenStokIdler = model.StokKarts.Where(a => a.SeciliMi).Select(a => a.Id).ToList();

            var secilenStoks = new List<StokKart>();
            foreach (var i in secilenStokIdler)
            {
                secilenStoks.Add(_db.StokKarts.Find(i));
            }

            secilenStoks = secilenStoks.OrderBy(a => a.StokTanimAdi).ToList();

            model.KomponentTalimatGrups = _db.KomponentTalimatGrups.ToList();


            var seciliKomponentTalimatGrups = model.KomponentTalimatGrups
                .Where(a => seciliPeriyotlar.Any(b => b == a.PeriyotTanimId)).ToList();

            foreach (var stok in secilenStoks)
            {
                var araModel = new List<TakipTakvimModel>();
                var secilenStok = _db.StokKarts.Find(stok.Id);

                araModel.Add(new TakipTakvimModel()
                {
                    StokKart = stok
                });
                var operasyonlar = _db.StokKartOperasyons.Where(a =>
                        a.PlanlananTarih >= BaslangisTarihi && a.PlanlananTarih <= BitisTarihi && a.StokKartId == stok.Id)
                    .ToList();
                var duruslar = _db.StokKartDurus.Where(a =>
                        a.DurusBaslangic >= BaslangisTarihi && a.DurusBaslangic <= BitisTarihi && a.StokKartId == stok.Id)
                    .ToList();

                foreach (var tableHeader in tableHeaders)
                {
                    var araItem = new TakipTakvimModel();
                    araItem.StokKart = stok;

                    var aralikOperasyonlar = operasyonlar.Where(a =>
                        a.PlanlananTarih >= tableHeader.DateTime && a.PlanlananTarih <= tableHeader.DateTime2 && seciliKomponentTalimatGrups.Any(b=>b.Id==a.KomponentTalimatGrupId)).ToList();
                   


                    
                   var aralikDuruslar = duruslar.Where(a =>
                        a.DurusBaslangic >= tableHeader.DateTime && a.DurusBaslangic <= tableHeader.DateTime2).ToList();
                    if (aralikOperasyonlar.Any())
                    {
                        araItem.StokKartOperasyons = aralikOperasyonlar;
                    }
                    else
                    {
                        araItem.StokKartOperasyons.Add(new StokKartOperasyon());
                    }
                    if (aralikDuruslar.Any())
                    {
                        araItem.StokKartDurus = aralikDuruslar;
                    }
                    else
                    {
                        araItem.StokKartDurus.Add(new StokKartDuru());
                    }
                   
                    araModel.Add(araItem);
                }


                takvimModel.Add(araModel);
            }

            model.TakipTakvimModels = takvimModel;

            return PartialView("_MakineBolumAnaliz", model);
        }
    }
}