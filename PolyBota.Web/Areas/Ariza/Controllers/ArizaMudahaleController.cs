using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Org.BouncyCastle.Cms;
using PolyBota.BLL;
using PolyBota.DAL;
using PolyBota.Entities;
using PolyBota.Helpers;
using PolyBota.Web.Areas.Ariza.Models;
using PolyBota.Web.Controllers;

namespace PolyBota.Web.Areas.Ariza.Controllers
{
    public class ArizaMudahaleController : BaseController
    {
        // GET: Ariza/ArizaMudahale
        private List<DropItem> DropArizaDurumlar()
        {
            var DropArizaDurumTanims = new List<DropItem>();
            var arizaDurumus = _db.ArizaDurumuTanims.Where(a => a.Id > 0).ToList();

            foreach (var item in arizaDurumus)
            {
                string textClass = "";
                string alertClass = "";
                bool seciliMi = true;
                if (item.Id == 1)
                {
                    //Açık Arıza
                    textClass = "text-danger";
                    alertClass = "alert-danger text-black";
                }
                if (item.Id == 2)
                {
                    //İşlemde / Beklemede
                    textClass = "text-warning";
                    alertClass = "alert-warning text-black";
                }
                if (item.Id == 3)
                {
                    //Arıza Açan Kapatma Onayı Bekleniyor
                    textClass = "text-info";
                    alertClass = "alert-info text-black";
                }
                if (item.Id == 4)
                {
                    //parça tanımı bekleniyor/parça bekliyor
                    textClass = "text-black";
                    alertClass = "alert-primary text-black";
                }
                if (item.Id == 5)
                {
                    //Tamamlandı
                    textClass = "text-black";
                    alertClass = "alert-secondary text-black";
                    seciliMi = false;
                }
                DropArizaDurumTanims.Add(new DropItem()
                {
                    Tanim1 = item.ArizaDurumuAdi,
                    Id = item.Id.ToString(),
                    IdInt = item.Id,
                    Tanim2 = textClass,
                    Tanim3 = alertClass,
                    SeciliMi = item.Id == 1
                });
            }

            return DropArizaDurumTanims;
        }

        public ActionResult ArizaMudahaleListe()
        {
            var model = new ArizaMudahaleModel();

            model.DropArizaDurumTanims = DropArizaDurumlar();

            if (Session["DropArizaDurum"] != null)
            {
                var lst = (List<DropItem>)Session["DropArizaDurum"];

                foreach (var item in model.DropArizaDurumTanims)
                {
                    item.SeciliMi = lst.Any(a => a.Id == item.Id && a.SeciliMi);
                }
            }
            //TempData["theme"] = "gizle";
            return View(model);
        }

        public PartialViewResult ArizaMuhahaleListeGetir(ArizaMudahaleModel model)
        {
            Session["DropArizaDurum"] = model.DropArizaDurumTanims;

            var seciliDurumlar = model.DropArizaDurumTanims.Where(a => a.SeciliMi).ToList().Select(a => a.Id).ToList();

            var DropFiltreKullanici = new DropItem();
            var DropFiltrePotaKart = new DropItem();
            var DropFiltreArizaTanimi = new DropItem();
            var DropFiltreArizaGrubu = new DropItem();
            if (seciliDurumlar.Any())
            {
                /*
                  (dbo.ArizaBildirimHeader.ArizaDurumu = 1) OR
                            (dbo.ArizaBildirimHeader.ArizaDurumu = 2) OR
                            (dbo.ArizaBildirimHeader.ArizaDurumu = 3)
                 */

                var str = "";
                foreach (var i in seciliDurumlar)
                {
                    str += string.Format(" (dbo.ArizaBildirimHeader.ArizaDurumu = {0}) ", i);
                    if (i != seciliDurumlar.Last())
                    {
                        str += " OR ";
                    }
                }
                var squery = string.Format("SELECT DISTINCT dbo.ArizaBildirimHeader.Id, dbo.ArizaBildirimHeader.KayitTarihi, dbo.ArizaBildirimHeader.KayitYapan, dbo.ArizaBildirimHeader.AcilanArizaTanimId, dbo.ArizaTanim.ArizaTanimAdi, dbo.[User].Name, dbo.ArizaBildirimHeader.AcilanArizaNotu, dbo.ArizaBildirimHeader.ArizaDurumu FROM            dbo.ArizaBildirimHeader INNER JOIN dbo.ArizaTanim ON dbo.ArizaBildirimHeader.AcilanArizaTanimId = dbo.ArizaTanim.Id INNER JOIN dbo.[User] ON dbo.ArizaBildirimHeader.KayitYapan = dbo.[User].Id INNER JOIN dbo.ArizaBildirimItem ON dbo.ArizaBildirimHeader.Id = dbo.ArizaBildirimItem.ArizaBildirimHeaderId INNER JOIN dbo.PotaKart ON dbo.ArizaBildirimItem.PotaKartId = dbo.PotaKart.Id WHERE {0} ORDER BY  dbo.ArizaBildirimHeader.KayitTarihi DESC", str);

                var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

                var liste = new List<ArizaDurumModelItem>();

                var kullaniciMudahaleEdebilirArizaHeaderIds = new List<int>();

                #region kullaniciMudahaleEdebilirArizaHeaderIds QUERY

                var kulSquery = string.Format(
                    "SELECT        dbo.ArizaBildirimHeader.Id, dbo.UserDepartman.UserId FROM            (SELECT DISTINCT TOP (100) PERCENT dbo.ArizaTanimArizaTechizatGrubu.ArizaId, dbo.ArizaSorumluDepartman.DepartmanId                          FROM            dbo.ArizaTanimArizaTechizatGrubu INNER JOIN                                                    dbo.ArizaTanim ON dbo.ArizaTanimArizaTechizatGrubu.ArizaId = dbo.ArizaTanim.Id INNER JOIN                                                    dbo.ArizaTechizatGrubuTanim ON dbo.ArizaTanimArizaTechizatGrubu.ArizaTechizatGrubuId = dbo.ArizaTechizatGrubuTanim.Id INNER JOIN                                                    dbo.ArizaTanimGrup ON dbo.ArizaTanim.ArizaTanimGrupId = dbo.ArizaTanimGrup.Id INNER JOIN                                                    dbo.ArizaSorumluDepartman ON dbo.ArizaTanim.Id = dbo.ArizaSorumluDepartman.ArizaTanimId                          ORDER BY dbo.ArizaTanimArizaTechizatGrubu.ArizaId) AS ArizaMudahaleDept INNER JOIN                         dbo.ArizaBildirimHeader ON ArizaMudahaleDept.ArizaId = dbo.ArizaBildirimHeader.AcilanArizaTanimId INNER JOIN                         dbo.UserDepartman ON ArizaMudahaleDept.DepartmanId = dbo.UserDepartman.DepartmanId INNER JOIN                         dbo.[User] ON dbo.UserDepartman.UserId = dbo.[User].Id WHERE        (dbo.[User].ArizaMudahaleYetki = 1) AND (dbo.UserDepartman.UserId = {0})",
                    User.Id);

                var kulResult = BllMssql.CustomSql(kulSquery, SuaHelper.defaultSql()).ToList();

                foreach (var aa in kulResult)
                {
                    var lst = aa.Values.ToList();

                    int arizaHeaderId = (int)lst[0];
                    kullaniciMudahaleEdebilirArizaHeaderIds.Add(arizaHeaderId);
                }

                #endregion kullaniciMudahaleEdebilirArizaHeaderIds QUERY

                foreach (var aa in result)
                {
                    var lst = aa.Values.ToList();
                    var ArizaBildirimHeaderId = (int)lst[0];

                    if (kullaniciMudahaleEdebilirArizaHeaderIds.Any(a => a == ArizaBildirimHeaderId))
                    {
                        var KayitTarihi = (DateTime)lst[1];
                        var KayitYapan = (int)lst[2];
                        var AcilanArizaTanimId = (int)lst[3];
                        var ArizaTanimAdi = lst[4].ToString();

                        var Name = (string)lst[5];
                        var arizaNotu = lst[6]?.ToString();
                        var arizaDurumu = (int)lst[7];
                        var araItem = new ArizaDurumModelItem()
                        {
                            User = new User() { Id = KayitYapan, Name = Name },
                            AcilanArizaTanim = _db.ArizaTanims.Find(AcilanArizaTanimId),
                            ArizaBildirimHeader = _db.ArizaBildirimHeaders.Find(ArizaBildirimHeaderId)
                        };
                        araItem.ArizaTechizatGrubuTanim =
                            _db.ArizaTechizatGrubuTanims.Find(araItem.ArizaBildirimHeader.ArizaTechizatGrubuId);
                        liste.Add(araItem);

                        kullaniciMudahaleEdebilirArizaHeaderIds.Remove(ArizaBildirimHeaderId);
                    }
                }

                model.ArizaDurumModelItems = liste.OrderByDescending(a => a.ArizaBildirimHeader.KayitTarihi).ToList();

                foreach (var item in model.ArizaDurumModelItems)
                {
                    if (DropFiltreKullanici.ItemValues.Count(a => a.IdInt == item.User.Id) == 0)
                    {
                        DropFiltreKullanici.ItemValues.Add(new ItemValue()
                        {
                            IdInt = item.User.Id,
                            Text = item.User.Name
                        });
                    }

                    if (DropFiltreArizaTanimi.ItemValues.Count(a => a.IdInt == item.AcilanArizaTanim.Id) == 0)
                    {
                        DropFiltreArizaTanimi.ItemValues.Add(new ItemValue()
                        {
                            IdInt = item.AcilanArizaTanim.Id,
                            Text = item.AcilanArizaTanim.ArizaTanimAdi
                        });
                    }
                    if (DropFiltreArizaGrubu.ItemValues.Count(a => a.IdInt == item.ArizaTechizatGrubuTanim.Id) == 0)
                    {
                        DropFiltreArizaGrubu.ItemValues.Add(new ItemValue()
                        {
                            IdInt = item.ArizaTechizatGrubuTanim.Id,
                            Text = item.ArizaTechizatGrubuTanim.ArizaTechizatGrubuAdi
                        });
                    }

                    item.ArizaBildirimItems = _db.ArizaBildirimItems
                        .Where(a => a.ArizaBildirimHeaderId == item.ArizaBildirimHeader.Id && a.KapananArizaTanimId == 0).ToList();

                    var potaIdler = item.ArizaBildirimItems.Select(a => a.PotaKartId).Distinct().ToList();

                    foreach (var i in potaIdler)
                    {
                        if (model.PotaKarts.Count(a => a.Id == i) == 0)
                        {
                            model.PotaKarts.Add(_db.PotaKarts.Find(i));
                        }
                    }
                }
            }

            foreach (var item in model.PotaKarts)
            {
                DropFiltrePotaKart.ItemValues.Add(new ItemValue()
                {
                    IdInt = item.Id,
                    Text = item.PotaKartAdi
                });
            }
            model.TechizatTuruTanims = _db.TechizatTuruTanims.AsNoTracking().ToList();

            model.BilesenHeaders = _db.BilesenHeaders.AsNoTracking().ToList();
            model.BilesenItems = _db.BilesenItems.AsNoTracking().ToList();
            model.DropArizaDurumTanims = new List<DropItem>();
            model.DropArizaDurumTanims = DropArizaDurumlar();

            DropFiltreKullanici.ItemValues = DropFiltreKullanici.ItemValues.OrderBy(a => a.Text).ToList();
            DropFiltrePotaKart.ItemValues = DropFiltrePotaKart.ItemValues.OrderBy(a => a.Text).ToList();
            DropFiltreArizaTanimi.ItemValues = DropFiltreArizaTanimi.ItemValues.OrderBy(a => a.Text).ToList();
            DropFiltreArizaGrubu.ItemValues = DropFiltreArizaGrubu.ItemValues.OrderBy(a => a.Text).ToList();

            if (Session["DropFiltreKullanici"] != null)
            {
                DropFiltreKullanici.IntList = (List<int>)Session["DropFiltreKullanici"];
            }

            if (Session["DropFiltreArizaGrubu"] != null)
            {
                DropFiltreArizaGrubu.IntList = (List<int>)Session["DropFiltreArizaGrubu"];
            }

            if (Session["DropFiltreArizaTanimi"] != null)
            {
                DropFiltreArizaTanimi.IntList = (List<int>)Session["DropFiltreArizaTanimi"];
            }

            if (Session["DropFiltrePotaKart"] != null)
            {
                DropFiltrePotaKart.IntList = (List<int>)Session["DropFiltrePotaKart"];
            }

            model.DropFiltreKullanici = DropFiltreKullanici;
            model.DropFiltrePotaKart = DropFiltrePotaKart;
            model.DropFiltreArizaTanimi = DropFiltreArizaTanimi;
            model.DropFiltreArizaGrubu = DropFiltreArizaGrubu;

            Session["ArizaMudahaleModel"] = null;
            Session["CiktiArizaMudahaleModel"] = null;

            Session["ArizaMudahaleModel"] = model;
            Session["CiktiArizaMudahaleModel"] = model;
            return PartialView("_ArizaMuhahaleListeGetir", model);
        }

        public ActionResult ArizaDetay(int id)
        {
            var model = new ArizaMudahaleModel();

            var squery = string.Format("SELECT DISTINCT dbo.ArizaBildirimHeader.Id, dbo.ArizaBildirimHeader.KayitTarihi, dbo.ArizaBildirimHeader.KayitYapan, dbo.ArizaBildirimHeader.AcilanArizaTanimId, dbo.ArizaTanim.ArizaTanimAdi, dbo.[User].Name, dbo.ArizaBildirimHeader.AcilanArizaNotu, dbo.ArizaBildirimHeader.ArizaDurumu FROM            dbo.ArizaBildirimHeader INNER JOIN dbo.ArizaTanim ON dbo.ArizaBildirimHeader.AcilanArizaTanimId = dbo.ArizaTanim.Id INNER JOIN dbo.[User] ON dbo.ArizaBildirimHeader.KayitYapan = dbo.[User].Id INNER JOIN dbo.ArizaBildirimItem ON dbo.ArizaBildirimHeader.Id = dbo.ArizaBildirimItem.ArizaBildirimHeaderId INNER JOIN dbo.PotaKart ON dbo.ArizaBildirimItem.PotaKartId = dbo.PotaKart.Id WHERE (dbo.ArizaBildirimHeader.Id = {0})  ORDER BY  dbo.ArizaBildirimHeader.KayitTarihi DESC", id);

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

            var item = new ArizaDurumModelItem();
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
                var arizaDurumu = (int)lst[7];
                item = new ArizaDurumModelItem()
                {
                    User = new User() { Id = KayitYapan, Name = Name },
                    AcilanArizaTanim = new ArizaTanim() { ArizaTanimAdi = ArizaTanimAdi },
                    ArizaBildirimHeader = new ArizaBildirimHeader() { KayitTarihi = KayitTarihi, KayitYapan = KayitYapan, AcilanArizaTanimId = AcilanArizaTanimId, Id = ArizaBildirimHeaderId, AcilanArizaNotu = arizaNotu, ArizaDurumu = arizaDurumu }
                };
            }

            item.ArizaBildirimHeader = _db.ArizaBildirimHeaders.Find(id);

            if (item.ArizaBildirimHeader.KapananArizaTanimId != 0)
            {
                item.KapananArizaTanim = _db.ArizaTanims.Find(item.ArizaBildirimHeader.KapananArizaTanimId);
            }
            item.ArizaBildirimItems = _db.ArizaBildirimItems
                .Where(a => a.ArizaBildirimHeaderId == item.ArizaBildirimHeader.Id).ToList();

            var potaIdler = item.ArizaBildirimItems.Select(a => a.PotaKartId).Distinct().ToList();

            foreach (var i in potaIdler)
            {
                if (model.PotaKarts.Count(a => a.Id == i) == 0)
                {
                    model.PotaKarts.Add(_db.PotaKarts.Find(i));
                }
            }

            #region Arıza Bildirim Hareketleri

            squery = string.Format(
                "SELECT dbo.ArizaBildirimHareket.Tarih, dbo.[User].Name, dbo.ArizaDurumuTanim.ArizaDurumuAdi, dbo.ArizaBildirimHareket.ArizaHareketNotu FROM            dbo.ArizaBildirimHareket INNER JOIN                         dbo.[User] ON dbo.ArizaBildirimHareket.KayitYapanId = dbo.[User].Id INNER JOIN                         dbo.ArizaDurumuTanim ON dbo.ArizaBildirimHareket.ArizaDurumu = dbo.ArizaDurumuTanim.Id WHERE        (dbo.ArizaBildirimHareket.ArizaBildirimHeaderId = {0}) ORDER BY dbo.ArizaBildirimHareket.Tarih DESC",
                id);
            result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();
            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();
                var tarih = (DateTime)lst[0];
                var isimsoyism = lst[1]?.ToString();
                var arizaBildirimdurumu = lst[2]?.ToString();

                var hareketNotu = lst[3]?.ToString();
                model.DropArizaBildirimDurums.Add(new DropItem()
                {
                    DateTime = tarih,
                    Tanim1 = isimsoyism,
                    Tanim2 = arizaBildirimdurumu,
                    Tanim3 = hareketNotu
                });
            }

            #endregion Arıza Bildirim Hareketleri

            #region ArizaKapatma

            squery = string.Format(
                "SELECT Id, ArizaTanimAdi FROM            dbo.ArizaTanim ORDER BY ArizaTanimAdi");

            result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();

                var arizaTanimId = (int)lst[0];
                var arizaTanimAdi = lst[1]?.ToString();

                model.ArizaTanims.Add(new ArizaTanim()
                {
                    Id = arizaTanimId,
                    ArizaTanimAdi = arizaTanimAdi
                });
            }

            #endregion ArizaKapatma

            if (item.ArizaBildirimHeader.KapananArizaTanimId == 0)
            {
                item.ArizaBildirimHeader.KapananArizaTanimId = item.ArizaBildirimHeader.AcilanArizaTanimId;
            }

            var arizaCozums = _db.ArizaTanimCozums
                .Where(a => a.ArizaTanimId == item.ArizaBildirimHeader.KapananArizaTanimId)
                .OrderBy(a => a.ArizaCozumAdi).ToList();

            var kayitliCozumler = _db.ArizaBildirimCozums.Where(a => a.ArizaBildirimHeaderId == id).ToList();

            foreach (var arizaTanimCozum in arizaCozums)
            {
                arizaTanimCozum.SeciliMi = kayitliCozumler.Any(a => a.ArizaCozumId == arizaTanimCozum.Id);
                model.ArizaTanimCozums.Add(arizaTanimCozum);
            }
            model.ArizaDurumModelItem = item;
            model.TechizatTuruTanims = _db.TechizatTuruTanims.AsNoTracking().ToList();

            model.BilesenHeaders = _db.BilesenHeaders.AsNoTracking().ToList();
            model.BilesenItems = _db.BilesenItems.AsNoTracking().ToList();
            model.DropArizaDurumTanims = new List<DropItem>();
            model.DropArizaDurumTanims = DropArizaDurumlar();

            squery = string.Format("SELECT dbo.ArizaBildirimHeaderKullanilanMalzeme.Id, dbo.ArizaBildirimHeaderKullanilanMalzeme.KayitTarihi, dbo.StokKart.StokTanimAdi, dbo.StokKart.StokKodu, dbo.[User].Name,                          dbo.ArizaBildirimHeaderKullanilanMalzeme.KullanilanMiktar, dbo.ArizaBildirimHeaderKullanilanMalzeme.ArizaBildirimHeaderId FROM            dbo.StokKart INNER JOIN                         dbo.ArizaBildirimHeaderKullanilanMalzeme ON dbo.StokKart.Id = dbo.ArizaBildirimHeaderKullanilanMalzeme.StokKartId INNER JOIN                         dbo.[User] ON dbo.ArizaBildirimHeaderKullanilanMalzeme.KayitYapan = dbo.[User].Id WHERE        (dbo.ArizaBildirimHeaderKullanilanMalzeme.ArizaBildirimHeaderId = {0})   ORDER BY dbo.StokKart.StokTanimAdi", id);

            result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();

                var arizaBildirimHeaderKullanilanMalzemeId = (int)lst[0];
                var KayitTarihi = (DateTime)lst[1];
                var StokTanimAdi = lst[2]?.ToString();
                var StokKodu = lst[3]?.ToString();
                var kayitYapanAdi = lst[4]?.ToString();
                var KullanilanMiktar = (int)lst[5];
                model.DropKullanilanMalzemeler.Add(new DropItem()
                {
                    IdInt = arizaBildirimHeaderKullanilanMalzemeId,
                    Tanim1 = StokTanimAdi,
                    Tanim2 = StokKodu,
                    Tanim3 = kayitYapanAdi,
                    IdInt2 = KullanilanMiktar,
                    DateTime = KayitTarihi,
                });
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult ArizaDetay(ArizaMudahaleModel model)
        {
            var header = model.ArizaDurumModelItem.ArizaBildirimHeader;
            var editItem = _db.ArizaBildirimHeaders.Find(header.Id);
            if (editItem.ArizaDurumu != header.ArizaDurumu)
            {
                editItem.ArizaDurumu = header.ArizaDurumu;
                editItem.KapananArizaNotu = header.KapananArizaNotu;
                _db.SaveChanges();

                _db.ArizaBildirimHarekets.Add(new ArizaBildirimHareket()
                {
                    ArizaBildirimHeaderId = editItem.Id,
                    ArizaDurumu = editItem.ArizaDurumu,
                    KayitYapanId = User.Id,
                    Tarih = DateTime.Now,
                    ArizaHareketNotu = header.KapananArizaNotu
                });
                _db.SaveChanges();
                TempDataCreate(2);
            }
            editItem.KapananArizaNotu = header.KapananArizaNotu;
            _db.SaveChanges();
            var kayitliCozumler = _db.ArizaBildirimCozums.Where(a => a.ArizaBildirimHeaderId == editItem.Id).ToList();
            var olmasiGerekenIdler = model.ArizaTanimCozums.Where(a => a.SeciliMi).ToList();

            var eklenecekListe = new List<ArizaBildirimCozum>();
            var silinecekListe = new List<ArizaBildirimCozum>();

            foreach (var arizaBildirimCozum in kayitliCozumler)
            {
                if (olmasiGerekenIdler.Count(a => a.Id == arizaBildirimCozum.ArizaCozumId) == 0)
                {
                    silinecekListe.Add(arizaBildirimCozum);
                }
            }

            foreach (var arizaTanimCozum in olmasiGerekenIdler)
            {
                if (kayitliCozumler.Count(a => a.ArizaCozumId == arizaTanimCozum.Id) == 0)
                {
                    eklenecekListe.Add(new ArizaBildirimCozum()
                    {
                        ArizaBildirimHeaderId = editItem.Id,
                        ArizaCozumId = arizaTanimCozum.Id
                    });
                }
            }

            if (eklenecekListe.Any())
            {
                _db.ArizaBildirimCozums.AddRange(eklenecekListe);
                _db.SaveChanges();
            }
            if (silinecekListe.Any())
            {
                _db.ArizaBildirimCozums.RemoveRange(silinecekListe);
                _db.SaveChanges();
            }

            return RedirectToAction("ArizaDetay", "ArizaMudahale", new { id = header.Id });
        }

        #region Kapanan Arıza Değişimi

        public PartialViewResult KapananArizaSecimi(int id)
        {
            var model = new ArizaMudahaleModel();

            var squery = string.Format(
                "SELECT DISTINCT TOP (100) PERCENT dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimId, dbo.PotaKart.Id, dbo.PotaKart.PotaKartAdi FROM            dbo.ArizaTanimArizaTechizatGrubu INNER JOIN                         dbo.ArizaTanimGerekliTechizatTuru ON dbo.ArizaTanimArizaTechizatGrubu.Id = dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimArizaTechizatGrupId INNER JOIN                         dbo.PotaKart ON dbo.ArizaTanimGerekliTechizatTuru.TechizatTuruId = dbo.PotaKart.TechizatTuruId WHERE        (dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimId = {0}) ORDER BY dbo.PotaKart.PotaKartAdi",
                id);

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();
            // arızaya uygun kartlar
            model.PotaKarts.Add(new PotaKart()
            {
                Id = 0,
                PotaKartAdi = "Seçiniz"
            });
            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();
                var arizaTanimId = lst[0]?.ToString();
                var potakartAdi = lst[2]?.ToString();
                var potaKartId = (int)lst[1];
                model.PotaKarts.Add(new PotaKart()
                {
                    Id = potaKartId,
                    PotaKartAdi = potakartAdi
                });
            }

            return PartialView("_KapananArizaSecimi", model);
        }

        public PartialViewResult KapananArizaVePkSecimi(int arizaId, int pkId)
        {
            var model = new ArizaMudahaleModel();
            var olasiEntegreKartlar = new List<PotaKart>();

            var referansPotaKart = _db.PotaKarts.Find(pkId);
            var techizatTuruId = referansPotaKart.TechizatTuruId;

            var ArizaTanimArizaTechizatGrupId = _db.ArizaTanimGerekliTechizatTurus.First(a =>
                a.TechizatTuruId == techizatTuruId && a.ArizaTanimId == arizaId &&
                a.ArizaTanimArizaTechizatGrupId != 0).ArizaTanimArizaTechizatGrupId;

            var gerekliTechizatTurleri = _db.ArizaTanimGerekliTechizatTurus
                .Where(a => a.ArizaTanimArizaTechizatGrupId == ArizaTanimArizaTechizatGrupId).ToList();

            #region Entegre Kartlar

            var entegreKartIdler = new List<int>();
            var entegreOlabilecekKartlar = _db.PotaKartEntegreKarts
                .Where(a => a.PotaKartId == referansPotaKart.Id || a.EntegrePotaKartId == referansPotaKart.Id).ToList();

            foreach (var item in entegreOlabilecekKartlar)
            {
                var olasiKartId = 0;
                if (item.PotaKartId == referansPotaKart.Id)
                {
                    olasiKartId = item.EntegrePotaKartId;
                }
                if (item.EntegrePotaKartId == referansPotaKart.Id)
                {
                    olasiKartId = item.PotaKartId;
                }

                if (olasiKartId != 0)
                {
                    if (!entegreKartIdler.Any(a => a == olasiKartId))
                    {
                        entegreKartIdler.Add(olasiKartId);
                    }
                }
            }

            foreach (var i in entegreKartIdler)
            {
                if (_db.PotaKarts.Any(a => a.Id == i))
                {
                    olasiEntegreKartlar.Add(_db.PotaKarts.Find(i));
                }
            }

            #endregion Entegre Kartlar

            var dropArizaTanimArizaTechizatGruplar = new List<DropItem>();
            var ItemValuesTechizatTurleri = new List<ItemValue>();
            var dropSecilenArizaTanimArizaTechizatGrup = new DropItem();

            #region arızayi açabilmek için gerekli ArizaTanimArizaTechizatGrupId ve TechizatTuruId

            foreach (var aa in gerekliTechizatTurleri)
            {
                var TechizatTuruId = aa.TechizatTuruId;

                ItemValuesTechizatTurleri.Add(new ItemValue()
                {
                    IdInt = TechizatTuruId,
                    IdInt2 = ArizaTanimArizaTechizatGrupId
                });

                if (dropArizaTanimArizaTechizatGruplar.Count(a => a.IdInt == ArizaTanimArizaTechizatGrupId) == 0)
                {
                    dropArizaTanimArizaTechizatGruplar.Add(new DropItem()
                    {
                        IdInt = ArizaTanimArizaTechizatGrupId
                    });
                }
            }

            foreach (var dropItem in dropArizaTanimArizaTechizatGruplar)
            {
                dropItem.ItemValues = ItemValuesTechizatTurleri.Where(a => a.IdInt2 == dropItem.IdInt).ToList();
            }

            foreach (var dropItem in dropArizaTanimArizaTechizatGruplar)
            {
                if (dropSecilenArizaTanimArizaTechizatGrup.IdInt != 0)
                {
                    break;
                }
                foreach (var itemValue in dropItem.ItemValues)
                {
                    if (itemValue.IdInt == referansPotaKart.TechizatTuruId)
                    {
                        dropSecilenArizaTanimArizaTechizatGrup = dropItem;
                        break;
                    }
                }
            }

            #endregion arızayi açabilmek için gerekli ArizaTanimArizaTechizatGrupId ve TechizatTuruId

            var ArizaTanimArizaTechizatGrubuId = dropSecilenArizaTanimArizaTechizatGrup.IdInt;
            var dropGerekliTechizatlarGruplar = DropArizaTanimArizaTechizatGruplariGetir(ArizaTanimArizaTechizatGrubuId);

            var bilesenHeaders = _db.BilesenHeaders.AsNoTracking().ToList();
            var bilesenItems = _db.BilesenItems.AsNoTracking().ToList();
            int tt = -1;
            foreach (var dropTechizat in dropGerekliTechizatlarGruplar)
            {
                tt++;
                var araModel = new ArizaPotaKartSecimModel()
                {
                    TechizatTuruTanim = new TechizatTuruTanim() { TechizatTuruTanimAdi = dropTechizat.Tanim, Id = dropTechizat.IdInt },
                    BilesenVarMi = dropTechizat.ItemValues.Any(),
                    i = tt,
                    ArizaTanimArizaTechizatGrubuId = ArizaTanimArizaTechizatGrubuId
                };

                if (referansPotaKart.TechizatTuruId == dropTechizat.IdInt)
                {
                    araModel.PotaKarts.Add(referansPotaKart);
                    araModel.PotaKartId = referansPotaKart.Id;
                    if (araModel.BilesenVarMi)
                    {
                        var potaKartBilesenModels = new List<PotaKartBilesenModel>();
                        var potaKartBilesenItems = _db.PotaKartBilesenItems.Where(a => a.PotaKartId == referansPotaKart.Id)
                            .ToList();

                        foreach (var bilesenHeader in dropTechizat.ItemValues)
                        {
                            if (bilesenHeader.IdInt != 0)
                            {
                                var bilesenItemsByHeader = bilesenItems
                                    .Where(a => a.BilesenHeaderId == bilesenHeader.IdInt)
                                    .OrderBy(a => a.BilesenDegeri).ToList();
                                var dropBilesenItems = new List<DropItem>();
                                dropBilesenItems.Add(new DropItem() { Id = "0", Tanim = "Seçiniz" });
                                foreach (var ii in bilesenItemsByHeader)
                                {
                                    if (potaKartBilesenItems.Any(a => a.BilesenItemId == ii.Id))
                                    {
                                        dropBilesenItems.Add(new DropItem()
                                        {
                                            Id = ii.Id.ToString(),
                                            Tanim = ii.BilesenDegeri
                                        });
                                    }
                                }

                                var ss = new PotaKartBilesenModel()
                                {
                                    BilesenHeader = bilesenHeaders.First(a => a.Id == bilesenHeader.IdInt),
                                    DropPotaKartBilesenItems = dropBilesenItems
                                };

                                potaKartBilesenModels.Add(ss);
                            }
                        }

                        araModel.PotaKartBilesenModels = potaKartBilesenModels;
                    }
                }
                else
                {
                    var araListe = new List<PotaKart>();
                    araListe.Add(new PotaKart() { Id = 0, PotaKartAdi = "Seçiniz" });
                    araListe.AddRange(olasiEntegreKartlar.Where(a => a.TechizatTuruId == dropTechizat.IdInt).ToList());
                    araModel.PotaKarts.AddRange(araListe);
                }
                model.ArizaPotaKartSecimModels.Add(araModel);

                model.ArizaTanim = _db.ArizaTanims.Find(arizaId);
            }
            return PartialView("_PkveArizayaGoreKapatmaKayit", model);
        }

        public PartialViewResult PotaKartaGoreBilesenItemGetir(int id, int arzId, int indexId)
        {
            //arzId =ArizaTanimArizaTechizatGrubuId

            var model = new ArizaMudahaleModel()
            {
                indexId = indexId
            };

            for (int i = 0; i <= indexId; i++)
            {
                model.ArizaPotaKartSecimModels.Add(new ArizaPotaKartSecimModel());
            }

            var bilesenHeaders = _db.BilesenHeaders.AsNoTracking().ToList();
            var bilesenItems = _db.BilesenItems.AsNoTracking().ToList();

            var dropGerekliTechizatlarGruplar = DropArizaTanimArizaTechizatGruplariGetir(arzId);

            var potaKart = _db.PotaKarts.Find(id);

            var araModel = new ArizaPotaKartSecimModel()
            {
                i = indexId,
                PotaKartId = id,
                ArizaTanimArizaTechizatGrubuId = arzId
            };

            foreach (var dropTechizat in dropGerekliTechizatlarGruplar)
            {
                if (potaKart.TechizatTuruId == dropTechizat.IdInt)
                {
                    araModel.BilesenVarMi = dropTechizat.ItemValues.Any();
                    if (araModel.BilesenVarMi)
                    {
                        var potaKartBilesenModels = new List<PotaKartBilesenModel>();
                        var potaKartBilesenItems = _db.PotaKartBilesenItems.Where(a => a.PotaKartId == potaKart.Id)
                            .ToList();

                        foreach (var bilesenHeader in dropTechizat.ItemValues)
                        {
                            if (bilesenHeader.IdInt != 0)
                            {
                                var bilesenItemsByHeader = bilesenItems
                                    .Where(a => a.BilesenHeaderId == bilesenHeader.IdInt)
                                    .OrderBy(a => a.BilesenDegeri).ToList();
                                var dropBilesenItems = new List<DropItem>();
                                dropBilesenItems.Add(new DropItem() { Id = "0", Tanim = "Seçiniz" });
                                foreach (var ii in bilesenItemsByHeader)
                                {
                                    if (potaKartBilesenItems.Any(a => a.BilesenItemId == ii.Id))
                                    {
                                        dropBilesenItems.Add(new DropItem()
                                        {
                                            Id = ii.Id.ToString(),
                                            Tanim = ii.BilesenDegeri
                                        });
                                    }
                                }

                                var ss = new PotaKartBilesenModel()
                                {
                                    BilesenHeader = bilesenHeaders.First(a => a.Id == bilesenHeader.IdInt),
                                    DropPotaKartBilesenItems = dropBilesenItems
                                };

                                potaKartBilesenModels.Add(ss);
                            }
                        }

                        araModel.PotaKartBilesenModels = potaKartBilesenModels;
                    }
                }
            }

            model.ArizaPotaKartSecimModels[indexId] = araModel;
            return PartialView("_PkveArizayaGoreKapatmaKayitItem", model);
        }

        public List<DropItem> DropArizaTanimArizaTechizatGruplariGetir(int ArizaTanimArizaTechizatGrubuId)
        {
            var dropGerekliTechizatlarGruplar = new List<DropItem>();
            var itemValuesGerekliBilesenHeaderlar = new List<ItemValue>();

            #region ArizaTanimArizaTechizatGrup için gerekli techizatTuru ve Bilesen Headerlar

            // ArizaTanimArizaTechizatGrup için gerekli techizatTuru ve Bilesen Headerlar
            var squery = string.Format(
                  "SELECT        TOP (100) PERCENT dbo.ArizaTanimGerekliTechizatTuru.TechizatTuruId, ISNULL(dbo.ArizaTanimGerekliBilesenHeader.BilesenHeaderId, 0) AS BilesenHeaderId, dbo.BilesenHeader.BilesenHeaderAdi,                          dbo.TechizatTuruTanim.TechizatTuruTanimAdi FROM            dbo.TechizatTuruTanim INNER JOIN                         dbo.ArizaTanimArizaTechizatGrubu INNER JOIN                         dbo.ArizaTanimGerekliTechizatTuru ON dbo.ArizaTanimArizaTechizatGrubu.Id = dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimArizaTechizatGrupId AND                          dbo.ArizaTanimArizaTechizatGrubu.ArizaId = dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimId ON dbo.TechizatTuruTanim.Id = dbo.ArizaTanimGerekliTechizatTuru.TechizatTuruId LEFT OUTER JOIN                         dbo.BilesenHeader INNER JOIN                         dbo.ArizaTanimGerekliBilesenHeader ON dbo.BilesenHeader.Id = dbo.ArizaTanimGerekliBilesenHeader.BilesenHeaderId ON                          dbo.ArizaTanimGerekliTechizatTuru.TechizatTuruId = dbo.ArizaTanimGerekliBilesenHeader.TechizatTuruId AND                          dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimArizaTechizatGrupId = dbo.ArizaTanimGerekliBilesenHeader.ArizaTanimArizaTechizatGrupId AND                          dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimId = dbo.ArizaTanimGerekliBilesenHeader.ArizaTanimId WHERE        (dbo.ArizaTanimArizaTechizatGrubu.Id = {0}) ORDER BY dbo.BilesenHeader.BilesenHeaderAdi",
                  ArizaTanimArizaTechizatGrubuId);

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();

                var TechizatTuruId = (int)lst[0];
                var BilesenHeaderId = (int)lst[1];
                var BilesenHeaderAdi = lst[2]?.ToString();
                var TechizatTuruTanimAdi = lst[3]?.ToString();
                if (dropGerekliTechizatlarGruplar.Count(a => a.IdInt == TechizatTuruId) == 0)
                {
                    dropGerekliTechizatlarGruplar.Add(new DropItem()
                    {
                        IdInt = TechizatTuruId,
                        Tanim = TechizatTuruTanimAdi
                    });
                }

                if (BilesenHeaderId != 0)
                {
                    itemValuesGerekliBilesenHeaderlar.Add(new ItemValue()
                    {
                        IdInt = BilesenHeaderId,
                        IdInt2 = TechizatTuruId,
                        Text = BilesenHeaderAdi
                    });
                }
            }

            foreach (var item in dropGerekliTechizatlarGruplar)
            {
                item.ItemValues = itemValuesGerekliBilesenHeaderlar.Where(a => a.IdInt2 == item.IdInt)
                    .ToList();
            }

            #endregion ArizaTanimArizaTechizatGrup için gerekli techizatTuru ve Bilesen Headerlar

            return dropGerekliTechizatlarGruplar;
        }

        [HttpPost]
        public JsonResult KapananArizaKaydet(ArizaMudahaleModel model)
        {
            var arizaBildirimHeaderId = model.ArizaDurumModelItem.ArizaBildirimHeader.Id;
            int state = 0; // 0- bilgiler eksik, 1- kayıt okey

            string icon = "";
            string title = "";
            int arizaDurumu = 0;

            var liste = model.ArizaPotaKartSecimModels.ToList();

            bool SorunVarmi = liste.Any(a => a.PotaKartId == 0);

            if (!SorunVarmi)
            {
                foreach (var item in liste)
                {
                    if (item.BilesenVarMi)
                    {
                        SorunVarmi = item.PotaKartBilesenModels.Any(a => a.SecilenBilesenItemId == 0);
                    }

                    if (SorunVarmi)
                    {
                        break;
                    }
                }
            }

            if (SorunVarmi)
            {
                icon = "warning";
                title = "bilgileri kontrol ediniz";
            }
            else
            {
                state = 1;

                var kapananArizaTanimId = model.ArizaTanim.Id;

                var arizaBildirimHeader = _db.ArizaBildirimHeaders.Find(arizaBildirimHeaderId);
                arizaBildirimHeader.KapananArizaTanimId = kapananArizaTanimId;
                _db.SaveChanges();

                var eskiKapananItems = _db.ArizaBildirimItems.Where(a =>
                    a.ArizaBildirimHeaderId == arizaBildirimHeaderId && a.AcilanArizaTanimId == 0).ToList();
                _db.ArizaBildirimItems.RemoveRange(eskiKapananItems);
                _db.SaveChanges();

                var items = new List<ArizaBildirimItem>();

                foreach (var item in liste)
                {
                    if (item.BilesenVarMi)
                    {
                        foreach (var itemPotaKartBilesenModel in item.PotaKartBilesenModels)
                        {
                            var secilenBilesenItem =
                                _db.BilesenItems.Find(itemPotaKartBilesenModel.SecilenBilesenItemId);

                            var yeniArizaItemAcilanKapanan = new ArizaBildirimItem()
                            {
                                TechizatTuruId = item.TechizatTuruTanim.Id,
                                PotaKartId = item.PotaKartId,
                                ArizaBildirimHeaderId = arizaBildirimHeaderId,
                                BilesenHeaderId = secilenBilesenItem.BilesenHeaderId,
                                BilesenItemId = secilenBilesenItem.Id,
                                AcilanArizaTanimId = 0,
                                KapananArizaTanimId = kapananArizaTanimId
                            };

                            items.Add(yeniArizaItemAcilanKapanan);
                        }
                    }
                    else
                    {
                        var yeniArizaItemAcilanKapanan = new ArizaBildirimItem()
                        {
                            TechizatTuruId = item.TechizatTuruTanim.Id,
                            PotaKartId = item.PotaKartId,
                            ArizaBildirimHeaderId = arizaBildirimHeaderId,
                            KapananArizaTanimId = kapananArizaTanimId
                        };

                        items.Add(yeniArizaItemAcilanKapanan);
                    }
                }

                _db.ArizaBildirimItems.AddRange(items);
                _db.SaveChanges();

                TempDataCreate(1);

                if (model.DropArizaDegistirme.IdInt == 1)
                {
                    var yeniArizaBildirimHeader = new ArizaBildirimHeader()
                    {
                        AcilanArizaNotu = arizaBildirimHeader.AcilanArizaNotu + " " + model.DropArizaDegistirme.Tanim1,
                        AcilanArizaTanimId = kapananArizaTanimId,
                        AktarilanArizaHeaderId = 0,
                        ArizaCozumId = 0,
                        ArizaDurumu = 1, // açık arıza
                        ArizaTechizatGrubuId = arizaBildirimHeader.ArizaTechizatGrubuId,
                        KapananArizaNotu = model.DropArizaDegistirme.Tanim1,
                        KapananArizaTanimId = kapananArizaTanimId,
                        KayitTarihi = DateTime.Now,
                        KayitYapan = arizaBildirimHeader.KayitYapan
                    };
                    _db.ArizaBildirimHeaders.Add(yeniArizaBildirimHeader);
                    _db.SaveChanges();
                    var yeniArizaBildirimItems = new List<ArizaBildirimItem>();

                    foreach (var arizaBildirimItem in items)
                    {
                        yeniArizaBildirimItems.Add(new ArizaBildirimItem()
                        {
                            AcilanArizaTanimId = arizaBildirimItem.KapananArizaTanimId,
                            ArizaBildirimHeaderId = yeniArizaBildirimHeader.Id,
                            BilesenHeaderId = arizaBildirimItem.BilesenHeaderId,
                            BilesenItemId = arizaBildirimItem.BilesenItemId,
                            KapananArizaTanimId = 0,
                            PotaKartId = arizaBildirimItem.PotaKartId,
                            TechizatTuruId = arizaBildirimItem.TechizatTuruId
                        });
                        yeniArizaBildirimItems.Add(new ArizaBildirimItem()
                        {
                            AcilanArizaTanimId = 0,
                            ArizaBildirimHeaderId = yeniArizaBildirimHeader.Id,
                            BilesenHeaderId = arizaBildirimItem.BilesenHeaderId,
                            BilesenItemId = arizaBildirimItem.BilesenItemId,
                            KapananArizaTanimId = arizaBildirimItem.KapananArizaTanimId,
                            PotaKartId = arizaBildirimItem.PotaKartId,
                            TechizatTuruId = arizaBildirimItem.TechizatTuruId
                        });
                    }

                    _db.ArizaBildirimItems.AddRange(yeniArizaBildirimItems);
                    _db.SaveChanges();

                    _db.ArizaBildirimHarekets.Add(new ArizaBildirimHareket()
                    {
                        ArizaBildirimHeaderId = yeniArizaBildirimHeader.Id,
                        ArizaDurumu = yeniArizaBildirimHeader.ArizaDurumu,
                        KayitYapanId = User.Id,
                        Tarih = DateTime.Now,
                        ArizaHareketNotu = string.Format("{0} : Açılan Arıza İlgisiz birimden ilgili birime güncellenmiştir", User.Name)
                    });
                    _db.SaveChanges();

                    var editEskiHeader = _db.ArizaBildirimHeaders.Find(arizaBildirimHeaderId);
                    editEskiHeader.ArizaDurumu = 5;
                    editEskiHeader.AktarilanArizaHeaderId = yeniArizaBildirimHeader.Id;
                    _db.SaveChanges();

                    _db.ArizaBildirimHarekets.Add(new ArizaBildirimHareket()
                    {
                        ArizaBildirimHeaderId = editEskiHeader.Id,
                        ArizaDurumu = editEskiHeader.ArizaDurumu,
                        KayitYapanId = User.Id,
                        Tarih = DateTime.Now,
                        ArizaHareketNotu = string.Format("{0} : Arıza Bu Birime ait değildir. Kapatılmıştır", User.Name)
                    });
                    _db.SaveChanges();

                    // ilgisiz birime aktarma yapılacak

                    /*
                       _db.ArizaBildirimHarekets.Add(new ArizaBildirimHareket()
                {
                    ArizaBildirimHeaderId = editItem.Id,
                    ArizaDurumu = editItem.ArizaDurumu,
                    KayitYapanId = User.Id,
                    Tarih = DateTime.Now,
                    ArizaHareketNotu = header.KapananArizaNotu
                });
                     */
                }
            }

            return new JsonResult { Data = new { Id = arizaBildirimHeaderId, state = state, icon = icon, title = title }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion Kapanan Arıza Değişimi

        [HttpPost]
        public PartialViewResult ArizaMudahaleThFiltreYap(ArizaMudahaleModel model)
        {
            var ArizaDurumModelItems = new List<ArizaDurumModelItem>();

            var sessionModel = new ArizaMudahaleModel();
            if (Session["ArizaMudahaleModel"] != null)
            {
                sessionModel = (ArizaMudahaleModel)Session["ArizaMudahaleModel"];
                foreach (var item in sessionModel.ArizaDurumModelItems)
                {
                    ArizaDurumModelItems.Add(new ArizaDurumModelItem()
                    {
                        User = item.User,
                        ArizaBildirimHeader = item.ArizaBildirimHeader,
                        AcilanArizaTanim = item.AcilanArizaTanim,
                        ArizaTechizatGrubuTanim = item.ArizaTechizatGrubuTanim,
                        ArizaBildirimItems = item.ArizaBildirimItems,
                        DropArizaOzet = item.DropArizaOzet,
                        KapananArizaTanim = item.KapananArizaTanim
                    });
                }
            }

            var kullaniciFiltre = model.DropFiltreKullanici.IntList;
            var ArizaGrubuFiltre = model.DropFiltreArizaGrubu.IntList;

            var ArizaTanimiFiltre = model.DropFiltreArizaTanimi.IntList;

            var PotaKartFiltre = model.DropFiltrePotaKart.IntList;

            Session["DropFiltreKullanici"] = kullaniciFiltre;
            Session["DropFiltreArizaGrubu"] = ArizaGrubuFiltre;
            Session["DropFiltreArizaTanimi"] = ArizaTanimiFiltre;
            Session["DropFiltrePotaKart"] = PotaKartFiltre;

            if (kullaniciFiltre.Any())
            {
                var gerekenArizaBildirmHeaderIdler = new List<int>();
                foreach (var i in kullaniciFiltre)
                {
                    var idLer = ArizaDurumModelItems.Where(a => a.ArizaBildirimHeader.KayitYapan == i)
                        .Select(a => a.ArizaBildirimHeader.Id).Distinct().ToList();

                    gerekenArizaBildirmHeaderIdler.AddRange(idLer);
                }
                gerekenArizaBildirmHeaderIdler = gerekenArizaBildirmHeaderIdler.Distinct().ToList();
                var filtreliListe = new List<ArizaDurumModelItem>();
                foreach (var i in gerekenArizaBildirmHeaderIdler)
                {
                    filtreliListe.AddRange(ArizaDurumModelItems.Where(a => a.ArizaBildirimHeader.Id == i).ToList());
                }

                ArizaDurumModelItems = filtreliListe;
            }

            if (ArizaGrubuFiltre.Any())
            {
                var gerekenArizaBildirmHeaderIdler = new List<int>();
                foreach (var i in ArizaGrubuFiltre)
                {
                    var idLer = ArizaDurumModelItems.Where(a => a.ArizaTechizatGrubuTanim.Id == i)
                        .Select(a => a.ArizaBildirimHeader.Id).Distinct().ToList();

                    gerekenArizaBildirmHeaderIdler.AddRange(idLer);
                }

                gerekenArizaBildirmHeaderIdler = gerekenArizaBildirmHeaderIdler.Distinct().ToList();
                var filtreliListe = new List<ArizaDurumModelItem>();
                foreach (var i in gerekenArizaBildirmHeaderIdler)
                {
                    filtreliListe.AddRange(ArizaDurumModelItems.Where(a => a.ArizaBildirimHeader.Id == i).ToList());
                }

                ArizaDurumModelItems = filtreliListe;
            }

            if (ArizaTanimiFiltre.Any())
            {
                var gerekenArizaBildirmHeaderIdler = new List<int>();
                foreach (var i in ArizaTanimiFiltre)
                {
                    var idLer = ArizaDurumModelItems.Where(a => a.AcilanArizaTanim.Id == i)
                        .Select(a => a.ArizaBildirimHeader.Id).Distinct().ToList();

                    gerekenArizaBildirmHeaderIdler.AddRange(idLer);
                }
                gerekenArizaBildirmHeaderIdler = gerekenArizaBildirmHeaderIdler.Distinct().ToList();
                var filtreliListe = new List<ArizaDurumModelItem>();
                foreach (var i in gerekenArizaBildirmHeaderIdler)
                {
                    filtreliListe.AddRange(ArizaDurumModelItems.Where(a => a.ArizaBildirimHeader.Id == i).ToList());
                }

                ArizaDurumModelItems = filtreliListe;
            }

            if (PotaKartFiltre.Any())
            {
                var gerekenArizaBildirmHeaderIdler = new List<int>();
                foreach (var i in PotaKartFiltre)
                {
                    foreach (var araItem in ArizaDurumModelItems)
                    {
                        if (araItem.ArizaBildirimItems.Any(a => a.PotaKartId == i))
                        {
                            gerekenArizaBildirmHeaderIdler.Add(araItem.ArizaBildirimHeader.Id);
                        }
                    }
                }
                gerekenArizaBildirmHeaderIdler = gerekenArizaBildirmHeaderIdler.Distinct().ToList();
                var filtreliListe = new List<ArizaDurumModelItem>();
                foreach (var i in gerekenArizaBildirmHeaderIdler)
                {
                    filtreliListe.AddRange(ArizaDurumModelItems.Where(a => a.ArizaBildirimHeader.Id == i).ToList());
                }

                ArizaDurumModelItems = filtreliListe;
            }

            model.ArizaDurumModelItems = ArizaDurumModelItems.ToList();

            model = new ArizaMudahaleModel()
            {
                BilesenHeaders = sessionModel.BilesenHeaders,
                BilesenItems = sessionModel.BilesenItems,
                DropArizaDurumTanims = sessionModel.DropArizaDurumTanims,
                PotaKarts = sessionModel.PotaKarts,
                ArizaDurumModelItems = ArizaDurumModelItems.ToList()
            };
            Session["CiktiArizaMudahaleModel"] = null;
            Session["CiktiArizaMudahaleModel"] = model;
            return PartialView("_ArizaMudahaleTbody", model);
        }

        public JsonResult UserArizaSayilariGetir()
        {
            var squery = string.Format(
                "SELECT        COUNT(Id) AS toplam, ArizaDurumu FROM            (SELECT        dbo.ArizaBildirimHeader.Id, dbo.UserDepartman.UserId, dbo.ArizaBildirimHeader.ArizaDurumu                          FROM            (SELECT DISTINCT TOP (100) PERCENT dbo.ArizaTanimArizaTechizatGrubu.ArizaId, dbo.ArizaSorumluDepartman.DepartmanId                                                    FROM            dbo.ArizaTanimArizaTechizatGrubu INNER JOIN                                                                              dbo.ArizaTanim ON dbo.ArizaTanimArizaTechizatGrubu.ArizaId = dbo.ArizaTanim.Id INNER JOIN                                                                              dbo.ArizaTechizatGrubuTanim ON dbo.ArizaTanimArizaTechizatGrubu.ArizaTechizatGrubuId = dbo.ArizaTechizatGrubuTanim.Id INNER JOIN                                                                              dbo.ArizaTanimGrup ON dbo.ArizaTanim.ArizaTanimGrupId = dbo.ArizaTanimGrup.Id INNER JOIN                                                                              dbo.ArizaSorumluDepartman ON dbo.ArizaTanim.Id = dbo.ArizaSorumluDepartman.ArizaTanimId                                                    ORDER BY dbo.ArizaTanimArizaTechizatGrubu.ArizaId) AS ArizaMudahaleDept INNER JOIN                                                    dbo.ArizaBildirimHeader ON ArizaMudahaleDept.ArizaId = dbo.ArizaBildirimHeader.AcilanArizaTanimId INNER JOIN                                                    dbo.UserDepartman ON ArizaMudahaleDept.DepartmanId = dbo.UserDepartman.DepartmanId INNER JOIN                                                    dbo.[User] ON dbo.UserDepartman.UserId = dbo.[User].Id                          WHERE        (dbo.[User].ArizaMudahaleYetki = 1) AND (dbo.UserDepartman.UserId = {0})) AS tbl GROUP BY ArizaDurumu",
                User.Id);

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

            var drops = new List<DropItem>();

            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();

                var toplam = (int)lst[0];
                var arizaDurumu = (int)lst[1];
                drops.Add(new DropItem()
                {
                    IdInt = arizaDurumu,
                    IdInt2 = toplam
                });
            }

            var dropArizaDurumlar = DropArizaDurumlar();

            foreach (var dropItem in dropArizaDurumlar)
            {
                var total = drops.Where(a => a.IdInt == dropItem.IdInt).Sum(a => a.IdInt2);

                dropItem.Tanim = string.Format("{0} ({1})", dropItem.Tanim1, total);
            }

            return new JsonResult { Data = dropArizaDurumlar, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public PartialViewResult ArizaMudahaleCikti()
        {
            var model = new ArizaMudahaleModel();
            if (Session["CiktiArizaMudahaleModel"] != null)
            {
                model = Session["CiktiArizaMudahaleModel"] as ArizaMudahaleModel;
            }

            return PartialView("_ArizaMudahaleCikti", model);
        }

        #region Ariza Müdahale Malzeme Ekle

        [HttpPost]
        public PartialViewResult StokKartAra(string id)
        {
            var model = new ArizaMudahaleModel();

            var squery =
                string.Format(
                    "SELECT        TOP (10) Id, StokTanimAdi, StokKodu, LotNo FROM dbo.StokKart  WHERE(StokTanimAdi LIKE N'%{0}%') OR (StokKodu LIKE N'%{0}%') ORDER BY StokTanimAdi",
                    id);

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();

                model.StokKarts.Add(new StokKart()
                {
                    Id = (int)lst[0],
                    StokTanimAdi = lst[1]?.ToString(),
                    StokKodu = lst[2]?.ToString(),
                    LogoId = lst[3]?.ToString()
                });
            }
            return PartialView("_KullanilanMalzemeStokKartAra", model);
        }

        [HttpPost]
        public ActionResult MalzemeEkle(ArizaMudahaleModel model)
        {
            var headerId = model.ArizaDurumModelItem.ArizaBildirimHeader.Id;

            var seciliStoks = model.StokKarts.Where(a => a.Miktar > 0).ToList();

            var kayitliStoks = _db.ArizaBildirimHeaderKullanilanMalzemes.Where(a => a.ArizaBildirimHeaderId == headerId)
                .ToList();

            var eklenecekStoks = new List<ArizaBildirimHeaderKullanilanMalzeme>();
            var guncellenecekStoks = new List<ArizaBildirimHeaderKullanilanMalzeme>();

            foreach (var item in kayitliStoks)
            {
                if (seciliStoks.Any(a => a.Id == item.StokKartId))
                {
                    var miktar = seciliStoks.First(a => a.Id == item.StokKartId).Miktar;
                    guncellenecekStoks.Add(new ArizaBildirimHeaderKullanilanMalzeme()
                    {
                        Id = item.Id,
                        KullanilanMiktar = item.KullanilanMiktar + miktar
                    });
                }
            }
            foreach (var item in seciliStoks)
            {
                if (kayitliStoks.Count(a => a.StokKartId == item.Id) == 0)
                {
                    eklenecekStoks.Add(new ArizaBildirimHeaderKullanilanMalzeme()
                    {
                        StokKartId = item.Id,
                        KullanilanMiktar = item.Miktar,
                        KayitYapan = User.Id,
                        KayitTarihi = DateTime.Now,
                        ArizaBildirimHeaderId = headerId
                    });
                }
            }

            if (eklenecekStoks.Any())
            {
                _db.ArizaBildirimHeaderKullanilanMalzemes.AddRange(eklenecekStoks);
                _db.SaveChanges();
            }

            if (guncellenecekStoks.Any())
            {
                foreach (var item in guncellenecekStoks)
                {
                    var editItem = _db.ArizaBildirimHeaderKullanilanMalzemes.Find(item.Id);
                    editItem.KullanilanMiktar = item.KullanilanMiktar;
                    editItem.KayitTarihi = DateTime.Now;
                    editItem.KayitYapan = User.Id;
                    _db.SaveChanges();
                }
            }
            return RedirectToAction("ArizaDetay", "ArizaMudahale", new { id = headerId });
        }

        [HttpPost]
        public ActionResult MalzemeMiktarGuncelle(ArizaMudahaleModel model)
        {
            var headerId = model.ArizaDurumModelItem.ArizaBildirimHeader.Id;

            var seciliMiktlar = model.DropKullanilanMalzemeler.Where(a => a.IdInt2 > 0).ToList();

            foreach (var dropItem in seciliMiktlar)
            {
                var editItem = _db.ArizaBildirimHeaderKullanilanMalzemes.Find(dropItem.IdInt);

                if (editItem.KullanilanMiktar != dropItem.IdInt2)
                {
                    editItem.KullanilanMiktar = dropItem.IdInt2;
                    editItem.KayitTarihi = DateTime.Now;
                    editItem.KayitYapan = User.Id;
                    _db.SaveChanges();
                }
            }

            TempDataCreate(2);
            return RedirectToAction("ArizaDetay", "ArizaMudahale", new { id = headerId });
        }

        #endregion Ariza Müdahale Malzeme Ekle
    }
}