using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
    public class HomeController : BaseController
    {
        public void EpostaEkle()
        {
            var squery =
                "SELECT        dbo.[User].Id, POTA_PTKS.dbo.Personel.ElektronikPosta FROM            POTA_PTKS.dbo.Personel INNER JOIN                         dbo.[User] ON POTA_PTKS.dbo.Personel.KullaniciKodu = dbo.[User].UserName WHERE        (POTA_PTKS.dbo.Personel.ElektronikPosta <> '')";

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();

                var userId = (int)lst[0];
                var eposta = lst[1]?.ToString();

                var editUser = _db.Users.Find(userId);
                editUser.EmailAdres = eposta;
                _db.SaveChanges();
            }
        }

        public void arizaAcabilecekDeptEsle()
        {
            var ArizaTechizatGrubuId = 2;// tekstüre
            var dropListe = new List<DropItem>();

            /*
            dropListe.Add(new DropItem() { Tanim = "Düze Arızası", Id = "8", IdInt = 43 }); // Düze Arızası 2-> epartmanId 43 üretim
            dropListe.Add(new DropItem() { Tanim = "Fantazi-Büküm  Arızaları", Id = "4", IdInt = 46 }); // Fantazi Arızası 2-> DepartmanId 46 Fantazi
            dropListe.Add(new DropItem() { Tanim = "LABORATUVAR", Id = "6", IdInt = 50 }); // LABORATUVAR 2-> DepartmanId 50 LABORATUVAR
            dropListe.Add(new DropItem() { Tanim = "Teknik Tekstil Arızası", Id = "3", IdInt = 45 });
            dropListe.Add(new DropItem(){Tanim = "Tekstüre Arızası", Id="2", IdInt = 44});

            dropListe.Add(new DropItem() { Tanim = "Üretim Arıza", Id = "1", IdInt = 43 }); // Tekstüre Arızası 2-> DepartmanId 43 üretim
            */
            dropListe.Add(new DropItem() { Tanim = "Üretim Arıza", Id = "7", IdInt = 54 }); // Tekstüre Arızası 2-> DepartmanId 43 üretim

            foreach (var dropItem in dropListe)
            {
                var squery = string.Format(
                    "SELECT        tbl.ArizaId, tbl.ArizaTechizatGrubuAdi, tbl.ArizaTanimGrupAdi, tbl.ArizaTanimAdi, tbl.ArizaTechizatGrubuId, dbo.ArizaAcilabilecekDepartman.DepartmanId FROM            (SELECT DISTINCT                                                     TOP (100) PERCENT dbo.ArizaTechizatGrubuTanim.ArizaTechizatGrubuAdi, dbo.ArizaTanimGrup.ArizaTanimGrupAdi, dbo.ArizaTanim.ArizaTanimAdi, dbo.ArizaTanimArizaTechizatGrubu.ArizaTechizatGrubuId,                                                     dbo.ArizaTanimArizaTechizatGrubu.ArizaId                          FROM            dbo.ArizaTanimArizaTechizatGrubu INNER JOIN                                                    dbo.ArizaTanim ON dbo.ArizaTanimArizaTechizatGrubu.ArizaId = dbo.ArizaTanim.Id INNER JOIN                                                    dbo.ArizaTechizatGrubuTanim ON dbo.ArizaTanimArizaTechizatGrubu.ArizaTechizatGrubuId = dbo.ArizaTechizatGrubuTanim.Id INNER JOIN                                                    dbo.ArizaTanimGrup ON dbo.ArizaTanim.ArizaTanimGrupId = dbo.ArizaTanimGrup.Id                          ORDER BY dbo.ArizaTechizatGrubuTanim.ArizaTechizatGrubuAdi) AS tbl LEFT OUTER JOIN                         dbo.ArizaAcilabilecekDepartman ON tbl.ArizaId = dbo.ArizaAcilabilecekDepartman.ArizaTanimId WHERE        (tbl.ArizaTechizatGrubuId = {0}) AND (dbo.ArizaAcilabilecekDepartman.Id IS NULL)", dropItem.Id);

                var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

                var eklenecekListe = new List<ArizaAcilabilecekDepartman>();
                foreach (var aa in result)
                {
                    var lst = aa.Values.ToList();

                    var arizaId = (int)lst[0];
                    eklenecekListe.Add(new ArizaAcilabilecekDepartman()
                    {
                        ArizaTanimId = arizaId,
                        DepartmanId = dropItem.IdInt,
                        SeciliMi = true
                    });
                }

                _db.ArizaAcilabilecekDepartmen.AddRange(eklenecekListe);
                _db.SaveChanges();
            }
        }

        public void yeniKullaniciKontrol()
        {
            var squery =
                "SELECT        TOP (100) PERCENT SrcnKullanici.KullaniciKodu, SrcnKullanici.PersonelKodu, SrcnKullanici.Sifre, SrcnKullanici.IsimSoyisim FROM            dbo.[User] RIGHT OUTER JOIN                             (SELECT        KullaniciId, KullaniciKodu, PersonelKodu, Sifre, IsimSoyisim, Resim                               FROM            POTA_PTKS.dbo.SrcnKullanicis WITH (NOLOCK)) AS SrcnKullanici ON dbo.[User].Name = SrcnKullanici.IsimSoyisim AND dbo.[User].PersonelCode = SrcnKullanici.PersonelKodu WHERE        (dbo.[User].Password IS NULL) ORDER BY SrcnKullanici.IsimSoyisim";
            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();
            var yeniListe = new List<User>();

            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();

                var KullaniciKodu = lst[0]?.ToString();
                var PersonelKodu = lst[1]?.ToString();
                var Sifre = lst[2]?.ToString();
                var IsimSoyisim = lst[3]?.ToString();
                yeniListe.Add(new User()
                {
                    Id = 0,

                    Name = IsimSoyisim,
                    Password = Sifre,
                    PersonelCode = PersonelKodu,
                    UserName = KullaniciKodu
                });
            }

            if (yeniListe.Any())
            {
                _db.Users.AddRange(yeniListe);
                _db.SaveChanges();
            }
        }

        public void pk()
        {
            var lst = new List<PotaKart>();

            #region MyRegion

            lst.Add(new PotaKart() { TechizatTuruId = 111, PotaKartAdi = "FK-5 & W1 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 111, PotaKartAdi = "FK-6 & W1 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 111, PotaKartAdi = "FK-7 & W1 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 111, PotaKartAdi = "FK-8 & W1 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 111, PotaKartAdi = "FK-9 & W1 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 111, PotaKartAdi = "FK-10 & W1 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 111, PotaKartAdi = "FK-11 & W1 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 111, PotaKartAdi = "FK-12 & W1 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 111, PotaKartAdi = "FK-13 & W1 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 112, PotaKartAdi = "FK-5 & W2 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 112, PotaKartAdi = "FK-6 & W2 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 112, PotaKartAdi = "FK-7 & W2 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 112, PotaKartAdi = "FK-8 & W2 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 112, PotaKartAdi = "FK-9 & W2 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 112, PotaKartAdi = "FK-10 & W2 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 112, PotaKartAdi = "FK-11 & W2 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 112, PotaKartAdi = "FK-12 & W2 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 112, PotaKartAdi = "FK-13 & W2 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 113, PotaKartAdi = "FK-5 & W3 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 113, PotaKartAdi = "FK-6 & W3 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 113, PotaKartAdi = "FK-7 & W3 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 113, PotaKartAdi = "FK-8 & W3 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 113, PotaKartAdi = "FK-9 & W3 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 113, PotaKartAdi = "FK-10 & W3 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 113, PotaKartAdi = "FK-11 & W3 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 113, PotaKartAdi = "FK-12 & W3 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 113, PotaKartAdi = "FK-13 & W3 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 114, PotaKartAdi = "FK-5 & W4 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 114, PotaKartAdi = "FK-6 & W4 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 114, PotaKartAdi = "FK-7 & W4 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 114, PotaKartAdi = "FK-8 & W4 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 114, PotaKartAdi = "FK-9 & W4 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 114, PotaKartAdi = "FK-10 & W4 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 114, PotaKartAdi = "FK-11 & W4 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 114, PotaKartAdi = "FK-12 & W4 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 114, PotaKartAdi = "FK-13 & W4 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 109, PotaKartAdi = "FK-5 & W5 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 109, PotaKartAdi = "FK-6 & W5 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 109, PotaKartAdi = "FK-7 & W5 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 109, PotaKartAdi = "FK-8 & W5 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 109, PotaKartAdi = "FK-9 & W5 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 109, PotaKartAdi = "FK-10 & W5 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 109, PotaKartAdi = "FK-11 & W5 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 109, PotaKartAdi = "FK-12 & W5 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 109, PotaKartAdi = "FK-13 & W5 MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 115, PotaKartAdi = "FK-5 & W6 MOTORU" });
            lst.Add(new PotaKart() { TechizatTuruId = 115, PotaKartAdi = "FK-6 & W6 MOTORU" });
            lst.Add(new PotaKart() { TechizatTuruId = 115, PotaKartAdi = "FK-7 & W6 MOTORU" });
            lst.Add(new PotaKart() { TechizatTuruId = 115, PotaKartAdi = "FK-8 & W6 MOTORU" });
            lst.Add(new PotaKart() { TechizatTuruId = 115, PotaKartAdi = "FK-9 & W6 MOTORU" });
            lst.Add(new PotaKart() { TechizatTuruId = 115, PotaKartAdi = "FK-10 & W6 MOTORU" });
            lst.Add(new PotaKart() { TechizatTuruId = 115, PotaKartAdi = "FK-11 & W6 MOTORU" });
            lst.Add(new PotaKart() { TechizatTuruId = 115, PotaKartAdi = "FK-12 & W6 MOTORU" });
            lst.Add(new PotaKart() { TechizatTuruId = 115, PotaKartAdi = "FK-13 & W6 MOTORU" });
            lst.Add(new PotaKart() { TechizatTuruId = 116, PotaKartAdi = "FK-5 & ATMUNG MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 116, PotaKartAdi = "FK-6 & ATMUNG MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 116, PotaKartAdi = "FK-7 & ATMUNG MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 116, PotaKartAdi = "FK-8 & ATMUNG MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 116, PotaKartAdi = "FK-9 & ATMUNG MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 116, PotaKartAdi = "FK-10 & ATMUNG MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 116, PotaKartAdi = "FK-11 & ATMUNG MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 116, PotaKartAdi = "FK-12 & ATMUNG MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 116, PotaKartAdi = "FK-13 & ATMUNG MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 117, PotaKartAdi = "FK-5 & İPLİK EMİŞ MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 117, PotaKartAdi = "FK-6 & İPLİK EMİŞ MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 117, PotaKartAdi = "FK-7 & İPLİK EMİŞ MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 117, PotaKartAdi = "FK-8 & İPLİK EMİŞ MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 117, PotaKartAdi = "FK-9 & İPLİK EMİŞ MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 117, PotaKartAdi = "FK-10 & İPLİK EMİŞ MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 117, PotaKartAdi = "FK-11 & İPLİK EMİŞ MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 117, PotaKartAdi = "FK-12 & İPLİK EMİŞ MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 117, PotaKartAdi = "FK-13 & İPLİK EMİŞ MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 118, PotaKartAdi = "FK-5 & BUHAR EMİŞ MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 118, PotaKartAdi = "FK-6 & BUHAR EMİŞ MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 118, PotaKartAdi = "FK-7 & BUHAR EMİŞ MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 118, PotaKartAdi = "FK-8 & BUHAR EMİŞ MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 118, PotaKartAdi = "FK-9 & BUHAR EMİŞ MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 118, PotaKartAdi = "FK-10 & BUHAR EMİŞ MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 118, PotaKartAdi = "FK-11 & BUHAR EMİŞ MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 118, PotaKartAdi = "FK-12 & BUHAR EMİŞ MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 118, PotaKartAdi = "FK-13 & BUHAR EMİŞ MOTORU " });
            lst.Add(new PotaKart() { TechizatTuruId = 119, PotaKartAdi = "FK-5 & SALYANGOZ FAN MOTORU" });
            lst.Add(new PotaKart() { TechizatTuruId = 119, PotaKartAdi = "FK-6 & SALYANGOZ FAN MOTORU" });
            lst.Add(new PotaKart() { TechizatTuruId = 119, PotaKartAdi = "FK-7 & SALYANGOZ FAN MOTORU" });
            lst.Add(new PotaKart() { TechizatTuruId = 119, PotaKartAdi = "FK-8 & SALYANGOZ FAN MOTORU" });
            lst.Add(new PotaKart() { TechizatTuruId = 119, PotaKartAdi = "FK-9 & SALYANGOZ FAN MOTORU" });
            lst.Add(new PotaKart() { TechizatTuruId = 119, PotaKartAdi = "FK-10 & SALYANGOZ FAN MOTORU" });
            lst.Add(new PotaKart() { TechizatTuruId = 119, PotaKartAdi = "FK-11 & SALYANGOZ FAN MOTORU" });
            lst.Add(new PotaKart() { TechizatTuruId = 119, PotaKartAdi = "FK-12 & SALYANGOZ FAN MOTORU" });
            lst.Add(new PotaKart() { TechizatTuruId = 119, PotaKartAdi = "FK-13 & SALYANGOZ FAN MOTORU" });

            #endregion MyRegion

            _db.PotaKarts.AddRange(lst);
            _db.SaveChanges();
        }

        public void userDuzelt()
        {
            var squery =
                "SELECT        TOP (100) PERCENT PersonelCode, UserName, COUNT(Id) AS Expr1 FROM            dbo.[User] GROUP BY PersonelCode, UserName HAVING        (COUNT(Id) > 1)";

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

            var yeniListe = new List<User>();

            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();

                var PersonelCode = lst[0]?.ToString();
                var UserName = lst[1]?.ToString();

                var kayitliListe = _db.Users.Where(a => a.PersonelCode == PersonelCode && a.UserName == UserName)
                    .OrderBy(a => a.Id).ToList();

                if (kayitliListe.Count > 1)
                {
                    for (int i = 1; i < kayitliListe.Count; i++)
                    {
                        yeniListe.Add(kayitliListe[i]);
                    }
                }
            }

            if (yeniListe.Any())
            {
                _db.Users.RemoveRange(yeniListe);
                _db.SaveChanges();
            }
        }

        // GET: Home
        public ActionResult Index()
        {
            Session["kul"] = null;
            Session["userRoles"] = null;
            Session["UserHeaderItemList"] = null;
            Session["kulLink"] = null;

            var mailListe = new List<string>();
            mailListe.Add("developer@suacreative.com");
            //            GenelDurumMailGonder(null, mailListe, "sercan içerik maili", "mesaj konu");

            //EpostaEkle();
            PtksStokKartAktar();

            var model = new AnasayfaModel()
            {
                Aciklama = User.Name
            };

            return View(model);
        }
    }
}