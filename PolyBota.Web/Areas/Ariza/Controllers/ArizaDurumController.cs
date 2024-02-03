using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PolyBota.BLL;
using PolyBota.DAL;
using PolyBota.Entities;
using PolyBota.Helpers;
using PolyBota.Web.Areas.Ariza.Models;
using PolyBota.Web.Controllers;

namespace PolyBota.Web.Areas.Ariza.Controllers
{
    public class ArizaDurumController : BaseController
    {
        // GET: Ariza/ArizaDurum

        private List<ArizaDurumModelItem> arizaDurumModelItemGetir(List<Dictionary<string, object>> result)
        {
            var liste = new List<ArizaDurumModelItem>();

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
                var araItem = new ArizaDurumModelItem()
                {
                    User = new User() { Id = KayitYapan, Name = Name },
                    AcilanArizaTanim = new ArizaTanim() { ArizaTanimAdi = ArizaTanimAdi },
                    ArizaBildirimHeader = _db.ArizaBildirimHeaders.Find(ArizaBildirimHeaderId)

                };
                liste.Add(araItem);
            }

            return liste;
        }
        public ActionResult ArizaDurumListe(int CurrentPage = 1, int PageShowCount = 50, int katId = 1)
        {

            var model = new ArizaDurumModel()
            {
                KatId = katId,
                ArizaDurumuTanims = _db.ArizaDurumuTanims.ToList(),
                TechizatTuruTanims = _db.TechizatTuruTanims.ToList(),
                BilesenHeaders = _db.BilesenHeaders.ToList(),
                BilesenItems = _db.BilesenItems.ToList()
            };


            var squery = string.Format("SELECT DISTINCT arizaHeaderOzet.Id, arizaHeaderOzet.KayitTarihi, arizaHeaderOzet.KayitYapan, arizaHeaderOzet.AcilanArizaTanimId, arizaHeaderOzet.ArizaTanimAdi, arizaHeaderOzet.Name, arizaHeaderOzet.AcilanArizaNotu,                          tbl.DepartmanId, arizaHeaderOzet.ArizaDurumu FROM            (SELECT DISTINCT TOP (100) PERCENT dbo.ArizaTanimArizaTechizatGrubu.ArizaId, dbo.ArizaAcilabilecekDepartman.DepartmanId                          FROM            dbo.ArizaTanimArizaTechizatGrubu INNER JOIN                                                    dbo.ArizaTanim ON dbo.ArizaTanimArizaTechizatGrubu.ArizaId = dbo.ArizaTanim.Id INNER JOIN                                                    dbo.ArizaTechizatGrubuTanim ON dbo.ArizaTanimArizaTechizatGrubu.ArizaTechizatGrubuId = dbo.ArizaTechizatGrubuTanim.Id INNER JOIN                                                    dbo.ArizaTanimGrup ON dbo.ArizaTanim.ArizaTanimGrupId = dbo.ArizaTanimGrup.Id INNER JOIN dbo.ArizaAcilabilecekDepartman ON dbo.ArizaTanim.Id = dbo.ArizaAcilabilecekDepartman.ArizaTanimId) AS tbl INNER JOIN                             (SELECT DISTINCT                                                          dbo.ArizaBildirimHeader.Id, dbo.ArizaBildirimHeader.KayitTarihi, dbo.ArizaBildirimHeader.KayitYapan, dbo.ArizaBildirimHeader.AcilanArizaTanimId, ArizaTanim_1.ArizaTanimAdi, dbo.[User].Name,                                                          dbo.ArizaBildirimHeader.AcilanArizaNotu, dbo.ArizaBildirimHeader.ArizaDurumu                               FROM            dbo.ArizaBildirimHeader INNER JOIN                                                         dbo.ArizaTanim AS ArizaTanim_1 ON dbo.ArizaBildirimHeader.AcilanArizaTanimId = ArizaTanim_1.Id INNER JOIN                                                         dbo.[User] ON dbo.ArizaBildirimHeader.KayitYapan = dbo.[User].Id INNER JOIN                                                         dbo.ArizaBildirimItem ON dbo.ArizaBildirimHeader.Id = dbo.ArizaBildirimItem.ArizaBildirimHeaderId INNER JOIN                                                         dbo.PotaKart ON dbo.ArizaBildirimItem.PotaKartId = dbo.PotaKart.Id                               WHERE        (dbo.ArizaBildirimItem.KapananArizaTanimId = 0)) AS arizaHeaderOzet ON tbl.ArizaId = arizaHeaderOzet.AcilanArizaTanimId");

            //, dbo.ArizaBildirimHeader.ArizaDurumu
            var hamListe = new List<ArizaDurumModelItem>();

            if (User.ArizaBildirimYetki == 0)
            {
                // user sadece kendiniGorebilir

                squery = string.Format("{0} WHERE        (arizaHeaderOzet.KayitYapan = {1}) AND (arizaHeaderOzet.ArizaDurumu = {2}) ", squery, User.Id, katId);


                var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();
                hamListe.AddRange(arizaDurumModelItemGetir(result));
            }
            else
            {
                // user departmanlarini görebilir

                var userDepts = _db.UserDepartmen.Where(a => a.UserId == User.Id).ToList().Select(a => a.DepartmanId).Distinct().ToList();


                foreach (var i in userDepts)
                {
                    var araSquery = string.Format("{0} WHERE        (tbl.DepartmanId = {1}) AND (arizaHeaderOzet.ArizaDurumu = {2}) ", squery, i,katId);
                    araSquery += " ORDER BY arizaHeaderOzet.KayitTarihi DESC";
                    var result = BllMssql.CustomSql(araSquery, SuaHelper.defaultSql()).ToList();
                    hamListe.AddRange(arizaDurumModelItemGetir(result));
                }


            }

        

            var liste = new List<ArizaDurumModelItem>();
            hamListe = hamListe.OrderByDescending(a => a.ArizaBildirimHeader.KayitTarihi).ToList();

            foreach (var arizaDurumModelItem in hamListe)
            {
                if (liste.Count(a=>a.ArizaBildirimHeader.Id== arizaDurumModelItem.ArizaBildirimHeader.Id)==0)
                {
                    liste.Add(arizaDurumModelItem);
                }
            }






            var pliste = new PagedListBase<ArizaDurumModelItem>() { CurrentPage = CurrentPage, PageShowCount = PageShowCount, DataLists = liste };

            var PageListBase = PageListBaseOlustur(pliste);

            model.PagedListSrcn = new PagedListSrcn()
            {
                PageShowCount = PageListBase.PageShowCount,
                PageSizeSelectList = PageListBase.PageSizeSelectList,
                PageNumberList = PageListBase.PageNumberList,
                CurrentPage = PageListBase.CurrentPage
            };
            model.ArizaDurumModelItems = pliste.DataLists;

            foreach (var item in model.ArizaDurumModelItems)
            {
                item.ArizaBildirimItems = _db.ArizaBildirimItems
                    .Where(a => a.ArizaBildirimHeaderId == item.ArizaBildirimHeader.Id && a.KapananArizaTanimId==0).ToList();

                var potaIdler = item.ArizaBildirimItems.Select(a => a.PotaKartId).Distinct().ToList();

                foreach (var i in potaIdler)
                {
                    if (model.PotaKarts.Count(a => a.Id == i) == 0)
                    {
                        model.PotaKarts.Add(_db.PotaKarts.Find(i));
                    }
                }
            }

            model.ArizaDurumuTanims = _db.ArizaDurumuTanims.ToList();
            return View(model);
        }


        public JsonResult SilmeTalebiGuncelle(int id, int durumId)
        {

            var item = _db.ArizaBildirimHeaders.Find(id);

            item.SilinsinMi = durumId;
            _db.SaveChanges();

            TempDataCreate(2);

            return new JsonResult { Data = new { Id = id }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult ArizaDurumDegistir(int id, int durumId)
        {

            var item = _db.ArizaBildirimHeaders.Find(id);

            item.ArizaDurumu = durumId;
            _db.SaveChanges();
            _db.ArizaBildirimHarekets.Add(new ArizaBildirimHareket()
            {
                ArizaBildirimHeaderId = item.Id,
                ArizaDurumu = durumId,
                KayitYapanId = User.Id,
                Tarih = DateTime.Now
            });
            _db.SaveChanges();

            TempDataCreate(2);

            return new JsonResult { Data = new { Id = id }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        

    }
}