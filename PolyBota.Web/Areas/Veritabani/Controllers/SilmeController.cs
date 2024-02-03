using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PolyBota.BLL;
using PolyBota.DAL;
using PolyBota.Entities;
using PolyBota.Helpers;
using PolyBota.Web.Areas.Veritabani.Models;
using PolyBota.Web.Controllers;

namespace PolyBota.Web.Areas.Veritabani.Controllers
{
    public class SilmeController : BaseController
    {
        // GET: Veritabani/Silme
        public ActionResult PotaKartListe()
        {
            var potakarts = _db.PotaKarts.Where(a => a.SilinsinMi > 0).ToList().OrderBy(a => a.PotaKartAdi).ToList();

            var model = new SilmeModel
            {
                PotaKarts = potakarts
            };
            return View(model);
        }
        [HttpPost]
        public ActionResult PotaKartListe(SilmeModel model)
        {
            var silIdler = new List<int>();
            var silListe = new List<PotaKart>();
            var revizeListe = new List<PotaKart>();

            var kartoperasyons = new List<StokKartOperasyon>();
            var entegrekartlar = new List<PotaKartEntegreKart>();
            var bilesens = new List<PotaKartBilesenItem>();
            foreach (var potaKart in model.PotaKarts)
            {
                if (potaKart.SilinsinMi == 1)
                {
                    // silme iptali yap
                    revizeListe.Add(potaKart);
                }

                if (potaKart.SilinsinMi == 2)
                {
                    // silme işlemi
                    silIdler.Add(potaKart.Id);
                }
            }

            foreach (var i in silIdler)
            {
                var item = _db.PotaKarts.Find(i);
                silListe.Add(item);
                kartoperasyons.AddRange(_db.StokKartOperasyons.Where(a=>a.StokKartId==i));
                bilesens.AddRange(_db.PotaKartBilesenItems.Where(a=>a.PotaKartId==i));
                entegrekartlar.AddRange(_db.PotaKartEntegreKarts.Where(a=>a.PotaKartId==i));
            }
            foreach (var i in revizeListe)
            {
                var item = _db.PotaKarts.Find(i.Id);
                item.SilinsinMi = 0;
                _db.SaveChanges();
            }

            if (silListe.Any())
            {
                _db.PotaKarts.RemoveRange(silListe);
                _db.SaveChanges();

                _db.StokKartOperasyons.RemoveRange(kartoperasyons);
                _db.SaveChanges();

                _db.PotaKartBilesenItems.RemoveRange(bilesens);
                _db.SaveChanges();

                _db.PotaKartEntegreKarts.RemoveRange(entegrekartlar);
                _db.SaveChanges();
            }
            TempDataCreate(2);
            return RedirectToAction("PotaKartListe");
        }
        public ActionResult ArizaBildirimHeaderListe()
        {
            var arizaHeaders = _db.ArizaBildirimHeaders.Where(a => a.SilinsinMi > 0).ToList().OrderBy(a => a.KayitTarihi).ToList();

            var model = new SilmeModel
            {
               
            };

            var squery =
                "SELECT DISTINCT                          arizaHeaderOzet.Id, arizaHeaderOzet.KayitTarihi, arizaHeaderOzet.KayitYapan, arizaHeaderOzet.AcilanArizaTanimId, arizaHeaderOzet.ArizaTanimAdi, arizaHeaderOzet.Name, arizaHeaderOzet.AcilanArizaNotu,                          tbl.DepartmanId, arizaHeaderOzet.ArizaDurumu FROM            (SELECT DISTINCT TOP (100) PERCENT dbo.ArizaTanimArizaTechizatGrubu.ArizaId, dbo.ArizaAcilabilecekDepartman.DepartmanId                          FROM            dbo.ArizaTanimArizaTechizatGrubu INNER JOIN                                                    dbo.ArizaTanim ON dbo.ArizaTanimArizaTechizatGrubu.ArizaId = dbo.ArizaTanim.Id INNER JOIN                                                    dbo.ArizaTechizatGrubuTanim ON dbo.ArizaTanimArizaTechizatGrubu.ArizaTechizatGrubuId = dbo.ArizaTechizatGrubuTanim.Id INNER JOIN                                                    dbo.ArizaTanimGrup ON dbo.ArizaTanim.ArizaTanimGrupId = dbo.ArizaTanimGrup.Id INNER JOIN                                                    dbo.ArizaAcilabilecekDepartman ON dbo.ArizaTanim.Id = dbo.ArizaAcilabilecekDepartman.ArizaTanimId) AS tbl INNER JOIN                             (SELECT DISTINCT                                                          dbo.ArizaBildirimHeader.SilinsinMi, dbo.ArizaBildirimHeader.Id, dbo.ArizaBildirimHeader.KayitTarihi, dbo.ArizaBildirimHeader.KayitYapan, dbo.ArizaBildirimHeader.AcilanArizaTanimId,                                                          ArizaTanim_1.ArizaTanimAdi, dbo.[User].Name, dbo.ArizaBildirimHeader.AcilanArizaNotu, dbo.ArizaBildirimHeader.ArizaDurumu                               FROM            dbo.ArizaBildirimHeader INNER JOIN                                                         dbo.ArizaTanim AS ArizaTanim_1 ON dbo.ArizaBildirimHeader.AcilanArizaTanimId = ArizaTanim_1.Id INNER JOIN                                                         dbo.[User] ON dbo.ArizaBildirimHeader.KayitYapan = dbo.[User].Id INNER JOIN                                                         dbo.ArizaBildirimItem ON dbo.ArizaBildirimHeader.Id = dbo.ArizaBildirimItem.ArizaBildirimHeaderId INNER JOIN                                                         dbo.PotaKart ON dbo.ArizaBildirimItem.PotaKartId = dbo.PotaKart.Id                               WHERE        (dbo.ArizaBildirimItem.KapananArizaTanimId = 0)) AS arizaHeaderOzet ON tbl.ArizaId = arizaHeaderOzet.AcilanArizaTanimId WHERE        (arizaHeaderOzet.SilinsinMi > 0)";

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();


            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();

                var ArizaBildirimHeaderId = (int)lst[0];
                var KayitTarihi = (DateTime)lst[1];
                var KayitYapan = (int)lst[2];
                var AcilanArizaTanimId = (int)lst[3];
                var ArizaTanimAdi = lst[4].ToString();

                var Name = (string)lst[5];
                var arizaNotu = lst[6]?.ToString();
                var DepartmanId = lst[7]?.ToString();
                var ArizaDurumu = (int)lst[8];

                model.DropItems.Add(new DropItem()
                {
                    IdInt = ArizaBildirimHeaderId,
                    Tanim1 = KayitTarihi.ToString(),
                    Tanim2 = ArizaTanimAdi,
                    Tanim3 = arizaNotu,
                    Tanim4 = Name,
                    IdInt2 = 2
                });
                
            }


            return View(model);
        }

        [HttpPost]
        public ActionResult ArizaBildirimHeaderListe(SilmeModel model)
        {
            var silIdler = new List<int>();
            var silListe = new List<ArizaBildirimHeader>();
            var revizeListe = new List<int>();

            var arizaItems = new List<ArizaBildirimItem>();
            var arizahareketler = new List<ArizaBildirimHareket>();
       
            foreach (var item in model.DropItems)
            {
                var arizaHeaderId = item.IdInt;
                if (item.IdInt2 == 1)
                {
                    // silme iptali yap
                    revizeListe.Add(arizaHeaderId);
                }

                if (item.IdInt2 == 2)
                {
                    // silme işlemi
                    silIdler.Add(arizaHeaderId);
                }
            }

            foreach (var i in silIdler)
            {
                var item = _db.ArizaBildirimHeaders.Find(i);
                silListe.Add(item);
                arizaItems.AddRange(_db.ArizaBildirimItems.Where(a => a.ArizaBildirimHeaderId == i));
                arizahareketler.AddRange(_db.ArizaBildirimHarekets.Where(a => a.ArizaBildirimHeaderId == i));
              
            }
            foreach (var i in revizeListe)
            {
                var item = _db.ArizaBildirimHeaders.Find(i);
                item.SilinsinMi = 0;
                _db.SaveChanges();
            }

            if (silListe.Any())
            {
                _db.ArizaBildirimHeaders.RemoveRange(silListe);
                _db.SaveChanges();

                _db.ArizaBildirimItems.RemoveRange(arizaItems);
                _db.SaveChanges();

                _db.ArizaBildirimHarekets.RemoveRange(arizahareketler);
                _db.SaveChanges();

            
            }
            TempDataCreate(2);
            return RedirectToAction("ArizaBildirimHeaderListe");
        }



        public ActionResult PotaKartOperasyonListe()
        {
            var arizaHeaders = _db.ArizaBildirimHeaders.Where(a => a.SilinsinMi > 0).ToList().OrderBy(a => a.KayitTarihi).ToList();

            var model = new SilmeModel
            {

            };

            var squery =
                "SELECT        TOP (100) PERCENT dbo.StokKartOperasyon.Id, dbo.StokKartOperasyon.StokKartId, dbo.StokKartOperasyon.OperasyonDurumu, dbo.StokKartOperasyon.SilinsinMi, dbo.PotaKart.PotaKartAdi,                          dbo.StokKartOperasyon.PlanlananTarih, dbo.KomponentTalimatGrup.KomponentTalimatGrupAdi FROM            dbo.StokKartOperasyon INNER JOIN                         dbo.PotaKart ON dbo.StokKartOperasyon.StokKartId = dbo.PotaKart.Id INNER JOIN                         dbo.KomponentTalimatGrup ON dbo.StokKartOperasyon.KomponentTalimatGrupId = dbo.KomponentTalimatGrup.Id WHERE        (dbo.StokKartOperasyon.SilinsinMi > 0) ORDER BY dbo.PotaKart.PotaKartAdi, dbo.KomponentTalimatGrup.KomponentTalimatGrupAdi, dbo.StokKartOperasyon.PlanlananTarih DESC";

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();


            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();

                var Id = (int)lst[0];
                var StokKartId = (int)lst[1];
                var OperasyonDurumu = (int)lst[2];
                var SilinsinMi = (int)lst[3];

                var PotaKartAdi = lst[4]?.ToString();
                var PlanlananTarih = (DateTime)lst[5];
                var KomponentTalimatGrupAdi = lst[6]?.ToString();

                model.DropItems.Add(new DropItem()
                {
                    DateTime = PlanlananTarih,
                    Tanim1 = PotaKartAdi,
                    Tanim2 = KomponentTalimatGrupAdi,
                    Id = StokKartId.ToString(),
                    IdInt = Id,
                    IdInt2 = SilinsinMi,
                    IdInt3 = OperasyonDurumu
                });

            }


            return View(model);
        }

        [HttpPost]
        public ActionResult PotaKartOperasyonListe(SilmeModel model)
        {
            var silIdler = new List<int>();
            var silListe = new List<StokKartOperasyon>();
            var revizeListe = new List<int>();

       

            foreach (var item in model.DropItems)
            {
                var operasyonId = item.IdInt;
                if (item.IdInt2 == 1)
                {
                    // silme iptali yap
                    revizeListe.Add(operasyonId);
                }

                if (item.IdInt2 == 2)
                {
                    // silme işlemi
                    silIdler.Add(operasyonId);
                }
            }

            foreach (var i in silIdler)
            {
                var item = _db.StokKartOperasyons.Find(i);
                silListe.Add(item);
           

            }
            foreach (var i in revizeListe)
            {
                var item = _db.StokKartOperasyons.Find(i);
                item.SilinsinMi = 0;
                _db.SaveChanges();
            }

            if (silListe.Any())
            {
                _db.StokKartOperasyons.RemoveRange(silListe);
                _db.SaveChanges();



            }
            TempDataCreate(2);
            return RedirectToAction("PotaKartOperasyonListe");
        }
    }
}