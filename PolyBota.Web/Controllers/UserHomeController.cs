using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PolyBota.BLL;
using PolyBota.DAL;
using PolyBota.Entities;
using PolyBota.Helpers;
using PolyBota.Web.Models;

namespace PolyBota.Web.Controllers
{
    public class UserHomeController : BaseController
    {
        // GET: UserHome
        public ActionResult Info()
        {
            return View();
        }

        public ActionResult Bildirim()
        {
            return View();
        }
        public PartialViewResult HizliArama(string kel)
        {
            var model = new LayoutModel(){DropHizliArama = new List<DropItem>()};
            List<UserHeaderItem> KullaniciLinkleri = new List<UserHeaderItem>();

            if (Session["kulLink"] == null)
            {
                KullaniciLinkleri = KullaniciLinkleriOlustur();
            }
            else
            {
                KullaniciLinkleri = (List<UserHeaderItem>)Session["kulLink"];
            }

            foreach (var linkModul in KullaniciLinkleri)
            {
                foreach (var item in linkModul.UserLinkList.Where(a => a.DetayLinkDurumu == 0).ToList())
                {
                   

                    
                        if (item.LinkTanimAdi.ToLower().Contains(kel.ToLower()))
                        {
                            model.DropHizliArama.Add(new DropItem()
                            {
                                Tanim = item.LinkTanimAdi,
                                Id = string.IsNullOrWhiteSpace(item.AreaName) ? string.Format("/{0}/{1}", item.ControllerName, item.ActionName) : string.Format("/{0}/{1}/{2}", item.AreaName, item.ControllerName, item.ActionName)
                            });
                        }
                   
                }
            }

            model.DropHizliArama = model.DropHizliArama.OrderBy(a => a.Tanim).ToList();


            return PartialView("_HizliArama", model);
        }
        public PartialViewResult BildirimKontrol()
        {
            var model = new LayoutModel() { DropHizliArama = new List<DropItem>() };


            bool arizaBildirimiVarmi = false;
            bool arizaMudahaleVarmi = false;

            int arizaKapatmaOnayiBekleniyorDurumu = 3;
            int arizaTamamlandiDurumu = 5;
            if (User.ArizaBildirimYetki==0)
            {
                // sadece kendi bildirimleri
                arizaBildirimiVarmi = _db.ViewArizaGuncelDurums.Any(a =>
                    a.UserId == User.Id && a.ArizaDurumu == arizaKapatmaOnayiBekleniyorDurumu);
            }
            else
            {
                // departman linkleri
                var userDepts = _db.UserDepartmen.Where(a => a.UserId == User.Id).ToList();
                foreach (var userDepartman in userDepts)
                {
                    if (!arizaBildirimiVarmi)
                    {
                        var durum = _db.ViewArizaGuncelDurums.Any(a => a.DepartmanId == userDepartman.DepartmanId && a.ArizaDurumu== arizaKapatmaOnayiBekleniyorDurumu);

                        if (durum)
                        {
                            arizaBildirimiVarmi = true;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (User.ArizaMudahaleYetki == 1)
            {
                // arıza kapatma onayı beklemeyen açık kayıtlar


                /*
                 SELECT DISTINCT TOP (100) PERCENT dbo.ArizaTanimArizaTechizatGrubu.ArizaId, dbo.ArizaSorumluDepartman.DepartmanId
FROM            dbo.ArizaTanimArizaTechizatGrubu INNER JOIN
                         dbo.ArizaTanim ON dbo.ArizaTanimArizaTechizatGrubu.ArizaId = dbo.ArizaTanim.Id INNER JOIN
                         dbo.ArizaTechizatGrubuTanim ON dbo.ArizaTanimArizaTechizatGrubu.ArizaTechizatGrubuId = dbo.ArizaTechizatGrubuTanim.Id INNER JOIN
                         dbo.ArizaTanimGrup ON dbo.ArizaTanim.ArizaTanimGrupId = dbo.ArizaTanimGrup.Id INNER JOIN
                         dbo.ArizaSorumluDepartman ON dbo.ArizaTanim.Id = dbo.ArizaSorumluDepartman.ArizaTanimId ORDER BY dbo.ArizaTanimArizaTechizatGrubu.ArizaId
                 */
                var squery = string.Format(
                   "SELECT        COUNT(dbo.ArizaBildirimHeader.Id) AS Expr1 FROM            (SELECT DISTINCT TOP (100) PERCENT dbo.ArizaTanimArizaTechizatGrubu.ArizaId, dbo.ArizaSorumluDepartman.DepartmanId FROM            dbo.ArizaTanimArizaTechizatGrubu INNER JOIN                         dbo.ArizaTanim ON dbo.ArizaTanimArizaTechizatGrubu.ArizaId = dbo.ArizaTanim.Id INNER JOIN                         dbo.ArizaTechizatGrubuTanim ON dbo.ArizaTanimArizaTechizatGrubu.ArizaTechizatGrubuId = dbo.ArizaTechizatGrubuTanim.Id INNER JOIN                         dbo.ArizaTanimGrup ON dbo.ArizaTanim.ArizaTanimGrupId = dbo.ArizaTanimGrup.Id INNER JOIN                         dbo.ArizaSorumluDepartman ON dbo.ArizaTanim.Id = dbo.ArizaSorumluDepartman.ArizaTanimId ORDER BY dbo.ArizaTanimArizaTechizatGrubu.ArizaId) AS ArizaMudahaleDept INNER JOIN                         dbo.ArizaBildirimHeader ON ArizaMudahaleDept.ArizaId = dbo.ArizaBildirimHeader.AcilanArizaTanimId INNER JOIN                         dbo.UserDepartman ON ArizaMudahaleDept.DepartmanId = dbo.UserDepartman.DepartmanId INNER JOIN                         dbo.[User] ON dbo.UserDepartman.UserId = dbo.[User].Id WHERE        (dbo.[User].ArizaMudahaleYetki = 1) AND (dbo.ArizaBildirimHeader.ArizaDurumu <> {0}) AND  (dbo.ArizaBildirimHeader.ArizaDurumu <> {1}) AND (dbo.UserDepartman.UserId = {2})",
                   arizaKapatmaOnayiBekleniyorDurumu, arizaTamamlandiDurumu, User.Id);

               var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

               var sonuc = 0;

               foreach (var aa in result)
               {
                   var lst = aa.Values.ToList();

                   sonuc += (int) lst[0];
               }


               arizaMudahaleVarmi = sonuc > 0;

            }

            if (arizaBildirimiVarmi)
            {
                model.DropBildirimLinkler.Add(new DropItem(){Id = string.Format("/Ariza/ArizaDurum/ArizaDurumListe?katId={0}",arizaKapatmaOnayiBekleniyorDurumu), Tanim = "~/Resimler/red-belly.png" });
            }
            if (arizaMudahaleVarmi)
            {
                model.DropBildirimLinkler.Add(new DropItem() { Id = "/Ariza/ArizaMudahale/ArizaMudahaleListe", Tanim = "~/Resimler/fail.svg" });
            }

            model.User = User;
            return PartialView("_BildirimLinkler", model);
        }

        public ActionResult KisiselBilgi()
        {
            return View(User);
        }

        [HttpPost]
        public ActionResult KisiselBilgi(User model)
        {
            var editUser = _db.Users.Find(User.Id);

            editUser.EmailAdres = model.EmailAdres;
            editUser.Password = model.Password;
            editUser.Name = model.Name;
            editUser.Title = model.Title;
            _db.SaveChanges();
            Session["kul"] = null;

            TempDataCreate(2);
           return RedirectToAction("KisiselBilgi");
        }
    }
}