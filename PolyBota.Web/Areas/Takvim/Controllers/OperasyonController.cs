using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PolyBota.BLL;
using PolyBota.DAL;
using PolyBota.Entities;
using PolyBota.Helpers;
using PolyBota.Web.Areas.Takvim.Model;
using PolyBota.Web.Controllers;

namespace PolyBota.Web.Areas.Takvim.Controllers
{
    public class OperasyonController : BaseController
    {
        // GET: Takvim/Operasyon

        public List<DropItem> TemelListe()
        {
            var model = new List<DropItem>();
            model.Add(new DropItem() { Tanim = "Techizat Türüne Göre", Id = "1" });
            //model.Add(new DropItem() { Tanim = "Stok Grubuna Göre", Id = "2" });
            model.Add(new DropItem() { Tanim = "Lokasyona Göre", Id = "3" });
            model.Add(new DropItem() { Tanim = "Operasyon Talimata Göre", Id = "4" });

            return model;
        }
        public TakvimOperasyonModel TakvimOperasyonModeliAnalizYapListe(TakvimOperasyonModel model)
        {

            var idler = model.PotaKarts.Where(a => a.SeciliMi).Select(a => a.Id).ToList();

            var baslangic = model.BaslangisTarihi.Date;
            var bitis = model.BitisTarihi.AddDays(1).Date;

            //var header = DropTakvimTableHeader(model.BaslangisTarihi, model.BitisTarihi, model.GosterimSekli);

            var kartListe = _db.PotaKarts.Where(a => idler.Any(b => b == a.Id)).OrderBy(a => a.PotaKartAdi).ToList();

            var operasyonlar = new List<StokKartOperasyon>();
            var duruslar = new List<StokKartDuru>();
            var araliktakiOperasyonlar = _db.StokKartOperasyons
                .Where(a => a.PlanlananTarih >= baslangic && a.PlanlananTarih <= bitis).OrderBy(a => a.PlanlananTarih)
                .ToList();

            foreach (var potaKart in kartListe)
            {
                var araListeOperasyon = araliktakiOperasyonlar
                      .Where(a => a.StokKartId == potaKart.Id )
                      .ToList();

                operasyonlar.AddRange(araListeOperasyon);

                
            }

            model.PotaKarts = kartListe;
            model.StokKartOperasyons = operasyonlar.OrderBy(a => a.PlanlananTarih).ToList();
            model.KomponentTalimatGrups = _db.KomponentTalimatGrups.ToList();
          
            model.PeriyotTanims = _db.PeriyotTanims.ToList();
            return model;
        }
        public TakvimOperasyonModel TakvimOperasyonModeliAnalizYap(TakvimOperasyonModel model)
        {

            var idler = model.PotaKarts.Where(a => a.SeciliMi).Select(a => a.Id).ToList();

            var baslangic = model.BaslangisTarihi.Date;
            var bitis = model.BitisTarihi.AddDays(1).Date;

            var header = DropTakvimTableHeader(model.BaslangisTarihi, model.BitisTarihi, model.GosterimSekli);

            var kartListe = _db.PotaKarts.Where(a => idler.Any(b => b == a.Id)).OrderBy(a => a.PotaKartAdi).ToList();

            var operasyonlar = new List<StokKartOperasyon>();
            var duruslar = new List<StokKartDuru>();

            foreach (var potaKart in kartListe)
            {
                var araListeOperasyon = _db.StokKartOperasyons
                      .Where(a => a.StokKartId == potaKart.Id && a.PlanlananTarih >=
                          baslangic && a.PlanlananTarih < bitis).OrderBy(a => a.PlanlananTarih)
                      .ToList();
                operasyonlar.AddRange(araListeOperasyon);

                var araListeDuruslar = _db.StokKartDurus
                    .Where(a => a.StokKartId == potaKart.Id && a.DurusBaslangic >=
                        baslangic && a.DurusBaslangic < bitis).OrderBy(a => a.DurusBaslangic)
                    .ToList();
                duruslar.AddRange(araListeDuruslar);
            }

            model.ArrTable = new int[kartListe.Count, header.Count, 2, operasyonlar.Count];

            int i = -1;



            foreach (var potaKart in kartListe)
            {
                i++;
                int j = -1;
                var kartOperasyonlari = operasyonlar.Where(a => a.StokKartId == potaKart.Id).ToList();
                var kartDuruslari = duruslar.Where(a => a.StokKartId == potaKart.Id).ToList();
                foreach (var dropItem in header)
                {
                    j++;

                    int t = -1;

                    var lstOperasyon = kartOperasyonlari.Where(a =>
                        a.PlanlananTarih >= dropItem.DateTime &&
                        a.PlanlananTarih < dropItem.DateTime2).ToList();
                    var lstDuruslar = kartDuruslari.Where(a =>
                        a.DurusBaslangic >= dropItem.DateTime &&
                        a.DurusBaslangic < dropItem.DateTime2).ToList();
                    foreach (var operasyon in lstOperasyon)
                    {
                        t++;
                        model.ArrTable[i, j, 0, t] = operasyon.Id;
                    }
                    foreach (var durus in lstDuruslar)
                    {
                        t++;
                        model.ArrTable[i, j, 1, t] = durus.Id;
                    }
                }
            }

            model.PotaKarts = kartListe;
            model.StokKartOperasyons = operasyonlar;
            model.KomponentTalimatGrups = _db.KomponentTalimatGrups.ToList();
            model.TableHeaderlar = header;
            model.PeriyotTanims = _db.PeriyotTanims.ToList();
            return model;
        }
        public ActionResult Secim(int id = 0)
        {
            var model = new TakvimOperasyonModel() { SecimDurumu = id };
            model.DropBreadCrumbs.Add(new DropItem() { Id = "0", Tanim = "Anasayfa" });
            var headerListe = TemelListe();
            if (id == 0)
            {
                model.DropTakvimTipler = headerListe;
            }
            else
            {
                // TempData["theme"] = "gizle";
                model.DropBreadCrumbs.Add(headerListe.First(a => a.Id == id.ToString()));
                if (id == 1)
                {
                    //techizat türüne Göre

                    var breadCrumbs = TechizatBreadCrumbOlustur("0");
                    model.BreadCrumbs = breadCrumbs;

                    var techizatTurleri = _db.TechizatTuruTanims.ToList();

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

                            model.DropTakvimTipler.Add(new DropItem()
                            {
                                IdInt = item.Id,
                                Tanim = str,
                                Id = item.Id.ToString(),
                                BagliOlduguId = item.BagliOlduguId
                            });
                        }
                    }

                    model.DropTakvimTipler = model.DropTakvimTipler.OrderBy(a => a.Tanim).ToList();
                    /*
                    model.DropTakvimTipler = _db.TechizatTuruTanims.OrderBy(a => a.TechizatTuruTanimAdi).Select(a =>
                        new DropItem()
                        {
                            Tanim = a.TechizatTuruTanimAdi,
                            Id = a.Id.ToString()
                        }).ToList();
                    */
                }

                if (id == 2)
                {
                    //Stok Grubuna Göre

                    model.DropTakvimTipler = _db.KomponentTanimGrups.OrderBy(a => a.KomponentTanimGrupAdi).Select(a =>
                        new DropItem()
                        {
                            Tanim = a.KomponentTanimGrupAdi,
                            Id = a.Id.ToString()
                        }).ToList();
                }

                if (id == 3)
                {
                    //Lokasyona Göre
                    model.DropTakvimTipler = DropBolumKirilimlariGetir();
                }
                if (id == 4)
                {
                    //Operasyon Talimata Göre
                    model.DropTakvimTipler = _db.KomponentTalimatGrups.OrderBy(a => a.KomponentTalimatGrupAdi).Select(a =>
                        new DropItem()
                        {
                            Tanim = a.KomponentTalimatGrupAdi,
                            Id = a.Id.ToString()
                        }).ToList();
                }
            }

            return View(model);
        }

        [HttpPost]
        public PartialViewResult AnaliziHazirla(TakvimOperasyonModel model)
        {
            var secim = model.SecimDurumu;

            var idler = model.DropTakvimTipler.Where(a => a.SeciliMi).Select(a => a.Id).ToList();
            var kartListe = new List<PotaKart>();

            if (secim == 1)
            {

                // techizat Türüne Göre
                idler = model.BreadCrumbs.Where(a => a.SeciliMi).Select(a => a.ChildId).ToList();
                foreach (var iStr in idler)
                {
                    var id = Convert.ToInt32(iStr);
                    kartListe.AddRange(_db.PotaKarts.Where(a => a.TechizatTuruId == id));
                }
            }
            if (secim == 2)
            {
                // Stok Grubuna Göre
                foreach (var iStr in idler)
                {
                    var id = Convert.ToInt32(iStr);
                    kartListe.AddRange(_db.PotaKarts.Where(a => a.KomponentTanimGrupId == id));
                }
            }
            if (secim == 3)
            {
                // lokasyona göre
                foreach (var iStr in idler)
                {
                    var id = Convert.ToInt32(iStr);
                    kartListe.AddRange(_db.PotaKarts.Where(a => a.BolumId == id));
                }
            }
            if (secim == 4)
            {
                // Operasyon Talimata Göre
                var potaKartIdler = new List<int>();
                foreach (var iStr in idler)
                {
                    var id = Convert.ToInt32(iStr);
                    potaKartIdler.AddRange(_db.StokKartOperasyons
                       .Where(a => a.KomponentTalimatGrupId == id).Select(a => a.StokKartId).Distinct().ToList());
                }
                potaKartIdler = potaKartIdler.Distinct().ToList();
                foreach (var i in potaKartIdler)
                {
                    kartListe.AddRange(_db.PotaKarts.Where(a => a.Id == i));
                }
            }
            model.PotaKarts = kartListe.OrderBy(a => a.PotaKartAdi).ToList();
            return PartialView("_AnaliziHazirla", model);
        }


        [HttpPost]
        public PartialViewResult AnaliziYap(TakvimOperasyonModel model)
        {
            var secim = model.SecimDurumu;
            if (model.ListelemeSekli==0)
            {
                // table
                model = TakvimOperasyonModeliAnalizYap(model);
                model.PeriyotTipiTanims = _db.PeriyotTipiTanims.OrderBy(a => a.PeriyotTipiAdi).ToList();
                return PartialView("_AnaliziYap", model);
            }
            else
            {
                model = TakvimOperasyonModeliAnalizYapListe(model);
                model.PeriyotTipiTanims = _db.PeriyotTipiTanims.OrderBy(a => a.PeriyotTipiAdi).ToList();
                return PartialView("_AnaliziYapListe", model);
            }
          
        }


        public ActionResult OperasyonDragDropListe()
        {
            var model = new TakvimOperasyonModel();
            var squery =
                "SELECT dbo.StokKartOperasyon.PlanlananTarih, dbo.StokKartOperasyon.Id, dbo.StokKartOperasyon.StokKartId, dbo.StokKartOperasyon.KomponentTalimatGrupId, dbo.StokKartOperasyon.OperasyonDurumu,                          dbo.KomponentTalimatGrup.PeriyotTanimId, dbo.StokKart.StokTanimAdi, dbo.KomponentTalimatGrup.KomponentTalimatGrupAdi, dbo.PeriyotTanim.PeriyotTanimAdi, dbo.PeriyotTanim.PeriyotDonemi FROM            dbo.StokKart INNER JOIN                         dbo.StokKartOperasyon ON dbo.StokKart.Id = dbo.StokKartOperasyon.StokKartId INNER JOIN                         dbo.KomponentTalimatGrup ON dbo.StokKartOperasyon.KomponentTalimatGrupId = dbo.KomponentTalimatGrup.Id INNER JOIN                         dbo.PeriyotTanim ON dbo.KomponentTalimatGrup.PeriyotTanimId = dbo.PeriyotTanim.Id ORDER BY dbo.StokKart.StokTanimAdi, dbo.StokKartOperasyon.PlanlananTarih";

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

            var detayListe = new List<TakvimOperasyonItemDetayModel>();
            var distKomponentTalimatGrusps = new List<KomponentTalimatGrup>();
            var distPeriyotlar = new List<PeriyotTanim>();
            var distPotakartlar = new List<PotaKart>();
            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();

                var PlanlananTarih = (DateTime)lst[0];
                var operasyonId = (int)lst[1];
                var PotaKartId = (int)lst[2];
                var KomponentTalimatGrupId = (int)lst[3];
                var OperasyonDurumu = (int)lst[4];
                var PeriyotTanimId = (int)lst[5];

                var PotaKartAdi = lst[6]?.ToString();
                var KomponentTalimatGrupAdi = lst[7]?.ToString();
                var PeriyotTanimAdi = lst[8]?.ToString();
                var PeriyotDonemi = (int)lst[9];
                if (distKomponentTalimatGrusps.Count(a => a.Id == KomponentTalimatGrupId) == 0)
                {
                    distKomponentTalimatGrusps.Add(new KomponentTalimatGrup()
                    {
                        Id = KomponentTalimatGrupId,
                        KomponentTalimatGrupAdi = KomponentTalimatGrupAdi,
                        PeriyotTanimId = PeriyotTanimId
                    });

                }
                if (distPeriyotlar.Count(a => a.Id == PeriyotTanimId) == 0)
                {
                    distPeriyotlar.Add(new PeriyotTanim()
                    {
                        Id = PeriyotTanimId,
                        PeriyotTanimAdi = PeriyotTanimAdi
                    });

                }
                if (distPotakartlar.Count(a => a.Id == PotaKartId) == 0)
                {
                    distPotakartlar.Add(new PotaKart()
                    {
                        Id = PotaKartId,
                        PotaKartAdi = PotaKartAdi
                    });

                }

                detayListe.Add(new TakvimOperasyonItemDetayModel()
                {
                    KomponentTalimatGrupId = KomponentTalimatGrupId,
                    KomponentTalimatGrupAdi = KomponentTalimatGrupAdi,
                    OperasyonDurumu = OperasyonDurumu,
                    OperasyonId = operasyonId,
                    PeriyotDonemi = PeriyotDonemi,
                    PeriyotTanimAdi = PeriyotTanimAdi,
                    PeriyotTanimId = PeriyotTanimId,
                    PlanlananTarih = PlanlananTarih,
                    PotaKartId = PotaKartId,
                    PotaKartAdi = PotaKartAdi
                });




            }

            var basTarih = detayListe.Min(a => a.PlanlananTarih);
            var bitTarih = detayListe.Max(a => a.PlanlananTarih);
            var header = DropTakvimTableHeader(basTarih, bitTarih, 1);

            model.PotaKarts = distPotakartlar;
            
            var sutunSayisi = header.Count + 1;
            var rowSayisi = detayListe.Count - 1;
            var array = new int[rowSayisi, sutunSayisi,1,1];
            var satirI = -1;
         
            foreach (var potaKart in distPotakartlar)
            {
                var PotaKartOperasyonlar = detayListe.Where(a => a.PotaKartId == potaKart.Id).ToList().OrderBy(a => a.KomponentTalimatGrupAdi).ToList();

                var disPotaKartOperasyonlar =
                    PotaKartOperasyonlar.Select(a => a.KomponentTalimatGrupId).Distinct().ToList();

                foreach (var i in disPotaKartOperasyonlar)
                {
                    satirI++;
                    var resultOperasyonDetaylar =
                        PotaKartOperasyonlar.Where(a => a.KomponentTalimatGrupId == i).ToList();

                    foreach (var detayItem in resultOperasyonDetaylar)
                    {
                        var sutunJ = -1;
                        foreach (var dropItem in header)
                        {
                            sutunJ++;

                            if (dropItem.DateTime >= detayItem.PlanlananTarih && detayItem.PlanlananTarih < dropItem.DateTime2)
                            {

                                array[satirI, sutunJ,0,0] = detayItem.OperasyonId;
                                break;
                                ;
                            }
                        }


                    }
                }
                return View();
            }

            model.TableHeaderlar = header;
            model.ArrTable = array;
            model.TakvimOperasyonItemDetayModels = detayListe;

            return View(model);
        }
    }
}