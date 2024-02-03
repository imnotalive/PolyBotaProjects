using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using PolyBota.BLL;
using PolyBota.DAL;
using PolyBota.Entities;
using PolyBota.Helpers;
using PolyBota.Web.Helpers;
using PolyBota.Web.Models;

namespace PolyBota.Web.Controllers
{
    [CookieLogin]
    public class BaseController : Controller
    {
        public string LangShortDef { get; set; }
        public User User { get; set; }

        public POTA_BOTAEntities _db { get; set; }

        private List<DropItem> DropItemGenel { get; set; }
        #region Base Metotlar
        private bool LinkErisim(BaseUserLink currentLink, List<UserHeaderItem> kullaniciLinkleri, List<SystemBaseUrlLink> baseLinkList)
        {
            var state = false;

            if (baseLinkList.Any(a => a.ActionName == currentLink.ActionName && a.ControllerName == currentLink.ControllerName))
            {
                state = kullaniciLinkleri.Any(a => a.UserLinkList.Any(
                    b => b.ActionName == currentLink.ActionName && b.ControllerName == currentLink.ControllerName));
            }
            else
            {
                state = true;
            }


            return state;
        }
        public List<UserHeaderItem> KullaniciLinkleriOlustur()
        {
            var model = new List<UserHeaderItem>();

            var UserYetkiSorguItems = new List<UserYetkiSorguItem>();
            var linkGruplar = new List<SystemLinkGrupTanim>();
            var rolIdler = User.UserRoleIds.StrArrayeCevir("-");


            foreach (var i in rolIdler)
            {
                if (!string.IsNullOrWhiteSpace(i))
                {
                    var roleId = Convert.ToInt32(i);
                    var sliste = _db.SpRoleLinkleri(roleId);


                    foreach (var lst in sliste)
                    {
                        var aa = new UserYetkiSorguItem()
                        {
                            RoleTanimId = roleId,
                            LinkGrupTanimId = lst.LinkGrupTanimId,
                            LinkGrupTanimAdi = lst.LinkGrupTanimAdi,
                            LinkGrupTanimAdiEng = lst.LinkGrupTanimAdiEng,
                            IclassName = lst.IclassName,
                            AreaName = lst.AreaName,
                            ControllerName = lst.ControllerName,
                            ActionName = lst.ActionName,
                            LinkTanimAdi = lst.LinkTanimAdi,
                            LinkTanimAdiEng = lst.LinkTanimAdiEng,
                            DetayLinkDurumu = lst.DetayLinkDurumu
                        };

                        if (linkGruplar.Count(a => a.Id == aa.LinkGrupTanimId) == 0)
                        {
                            linkGruplar.Add(new SystemLinkGrupTanim()
                            {
                                IclassName = aa.IclassName,
                                LinkGrupTanimAdi = aa.LinkGrupTanimAdi,
                                LinkGrupTanimAdiEng = aa.LinkGrupTanimAdiEng,
                                Id = aa.LinkGrupTanimId
                            });
                        }
                        UserYetkiSorguItems.Add(aa);
                    }
                }
          

            }

            if (UserYetkiSorguItems.Any())
            {
                foreach (var linkGrup in linkGruplar.OrderBy(a => a.LinkGrupTanimAdi).ToList())
                {
                    var araItem = new UserHeaderItem();
                    araItem.LinkGrupTanim = linkGrup;
                    var altListe = UserYetkiSorguItems.Where(a => a.LinkGrupTanimId == linkGrup.Id)
                        .ToList();

                    foreach (var aa in altListe)
                    {
                        if (araItem.UserLinkList.Count(a => a.AreaName == aa.AreaName && a.ControllerName == aa.ControllerName && a.ActionName == aa.ActionName) == 0)
                        {
                            araItem.UserLinkList.Add(new BaseUserLink()
                            {
                                AreaName = aa.AreaName,
                                ControllerName = aa.ControllerName,
                                ActionName = aa.ActionName,
                                LinkTanimAdi = aa.LinkTanimAdi,
                                LinkTanimAdiEng = aa.LinkTanimAdiEng,
                                DetayLinkDurumu = aa.DetayLinkDurumu
                            });
                        }
                    }
                    model.Add(araItem);
                }
            }

            return model;
        }
        public void TempDataCustom(int state, string message)
        {
            bool OlumLuMu = false;

            TempData["class"] = "danger";

            if (state == 1)
            {
                OlumLuMu = true;
            }

            TempData["Msg"] = message;
        }

        /// <summary>
        /// 1-create,2-update,3-delete,4-missing data
        /// </summary>
        /// <param name="state"></param>
        public void TempDataCreate(int state)
        {
            var Mesaj = "";
            bool OlumLuMu = false;
            TempData["Msg"] = Mesaj;
            TempData["class"] = "danger";

            if (state == 1)
            {
                OlumLuMu = true;
                if (this.LangShortDef == "en")
                {
                    Mesaj = "Record Process is done";
                }
                else
                {
                    Mesaj = "Kayıt İşlemi Yapılmıştır";
                }
            }
            if (state == 2)
            {
                OlumLuMu = true;
                if (this.LangShortDef == "en")
                {
                    Mesaj = "Update Process is done";
                }
                else
                {
                    Mesaj = "Güncelleme İşlemi Yapılmıştır";
                }
            }
            if (state == 3)
            {
                OlumLuMu = true;
                if (this.LangShortDef == "en")
                {
                    Mesaj = "Delete Process is done";
                }
                else
                {
                    Mesaj = "Silme İşlemi Yapılmıştır";
                }
            }
            if (state == 4)
            {
                OlumLuMu = false;
                if (this.LangShortDef == "en")
                {
                    Mesaj = "Your data is wrong or missed, please re-check";
                }
                else
                {
                    Mesaj = "Bilgileri eksik girdiniz kontrol ediniz";
                }
            }
            if (OlumLuMu)
            {
                TempData["class"] = "success";
            }

            TempData["Msg"] = Mesaj;
        }

        public PagedListBase<T> PageListBaseOlustur<T>(PagedListBase<T> model)
        {
            #region Sayfada gösterim Sayısı

            var liste = new List<DropItem>();
            for (int i = 10; i < 160; i += 10)
            {
                liste.Add(new DropItem() { IdInt = i, Tanim = i.ToString() });
            }
            model.PageSizeSelectList = liste;

            #endregion Sayfada gösterim Sayısı

            var dataSize = model.DataLists.Count;

            if (model.CurrentPage == -1)
            {
                var arakusurat = dataSize % model.PageShowCount;

                var deger = (dataSize - arakusurat) / model.PageShowCount + 1;
                model.CurrentPage = deger;
            }
            else
            {
                if (model.CurrentPage < 1)
                {
                    model.CurrentPage = 1;
                }
            }
            int bitis = model.CurrentPage * model.PageShowCount; // 1 * 5

            int baslangic = bitis - model.PageShowCount; // 5-5 =0

            int TotalRangeCount = 0;

            var kusurat = dataSize % model.PageShowCount;
            if (kusurat != 0)
            {
                TotalRangeCount++;
                dataSize -= kusurat;
            }
            TotalRangeCount += (dataSize / model.PageShowCount);

            if (TotalRangeCount < 11)
            {
                for (int i = 1; i <= TotalRangeCount; i++)
                {
                    model.PageNumberList.Add(i);
                }
            }
            else
            {
                if (model.CurrentPage < 10)
                {
                    for (int i = 1; i < 11; i++)
                    {
                        model.PageNumberList.Add(i);
                    }
                }
                else
                {
                    for (int i = model.CurrentPage - 5; i < model.CurrentPage; i++)
                    {
                        model.PageNumberList.Add(i);
                    }
                    for (int i = model.CurrentPage; i < model.CurrentPage + 6; i++)
                    {
                        if (i <= TotalRangeCount)
                        {
                            model.PageNumberList.Add(i);
                        }
                    }
                }
            }

            if (dataSize < bitis)
            {
                // 10-500
                model.DataLists = model.DataLists.Skip(baslangic).ToList();
            }
            else
            {
                model.DataLists = model.DataLists.Skip(baslangic).ToList();

                if (model.DataLists.Count > model.PageShowCount)
                {
                    model.DataLists = model.DataLists.Take(model.PageShowCount).ToList();
                }
            }

            return new PagedListBase<T>() { CurrentPage = model.CurrentPage, PageShowCount = model.PageShowCount, DataLists = model.DataLists, PageNumberList = model.PageNumberList, PageSizeSelectList = model.PageSizeSelectList };
        }
        #endregion

        #region Projeye Özel Metotlar

        public string tarihiOlustur(DateTime tarih)
        {
            var ay = "";
            if (tarih.Month < 10)
            {
                ay = "0";
            }

            ay += tarih.Month;

            var gun = "";

            if (tarih.Day < 10)
            {
                gun = "0";
            }

            gun += tarih.Day;

            return string.Format("{0}-{1}-{2} 00:00:00", tarih.Year, ay, gun);
        }
        public long BotaAmbarStokHareketOlustur(int ambarStokKartId, int HareketTipi, decimal miktar, string Aciklama,int PtksFisNo, int PtksSiraNo )
        {

            var ambarStokKart = _db.AmbarStokKarts.Find(ambarStokKartId);

            if (HareketTipi==0)
            {
                // stok girişi
                ambarStokKart.ToplamMiktar += miktar;
            }
            else
            {
                // stok çıkışı
                ambarStokKart.ToplamMiktar -= miktar;
            }

            _db.SaveChanges();

            var hareket = new AmbarStokKartHareket()
            {
                StokKartId = ambarStokKart.StokKartId,
                Miktar = miktar,
                KayitTarihi = DateTime.Now,
                AmbarHareketTipi = HareketTipi,// stok girişi - stok çıkışı,
                AmbarStokKartId = ambarStokKart.Id,
                PtksFisNo = PtksFisNo, 
                PtksSiraNo = PtksSiraNo,
                UserId = User.Id,
                Aciklama = Aciklama
            };
            
            _db.AmbarStokKartHarekets.Add(hareket);
            _db.SaveChanges();
            return hareket.Id;
        }

        public void PtksAmbarTuketim(PtksAmbarStokModel ambarModel)
        {
            var ptksDb = new POTA_PTKSEntities();

            var fisNo = ptksDb.StokHareketAnas.Max(a => a.FisNo) + 1;

            var stokHareketAna = new StokHareketAna()
            {
                Aciklama = "BOTA TÜKETİM",
                BeyanEdilenMiktar2 = 0,// hareketId
                BeyanEdilenMiktar3 = 0, // diger Id
                Tarih = DateTime.Now,
                FisiDuzenleyen = "OTOMATIK",
                FisTuru = "US",
                CikisAmbarNo = ambarModel.Ambar.PotaAmbarNo
            };

            var stokhareketler = new List<StokHareket>();

            int sira = 0;

            foreach (var item in ambarModel.StokKoduMiktars)
            {
                sira++;
                string stokKodu = item.Tanim1;
                string lotNo = item.Tanim2;
                decimal miktar = item.TanimDecimal;

                stokhareketler.Add(new StokHareket()
                {
                    TanimlandigiTarih = DateTime.Now,
                    LotNo = lotNo,
                    StokKodu = stokKodu,
                    Miktar1 = miktar,
                    SiraNo = sira,
                    FisNo = fisNo,
                    DokumSiraNo = sira,
                    Aciklama = "BOTA Hareketi",
                    MaliyetMerkezNo = "",// seçimden gelecek
                    StokTuru = "",// bll
                    OlcuBirimi1 = "",//bll
                    OlcuBirimi2 = "",
                    OlcuBirimi3 = "",
                    SeriNo = "",
                    Fiyat = 0,
                    Miktar2 = 0,
                    Miktar3 = 0,
                    DovizBirimi = "TL",
                    KDVOrani = 0,
                    CariLotNo = "",
                    BrutNetKatsayisi = 1,
                    SiparisNo = "",
                    SiparisSiraNo = 0,
                    PaletNo = "",
                    PartiNo = "",
                    IslemGrubu = "",
                    SatinalmaSiparisNo = "",
                    SatinalmaSiparisSiraNo = 0,
                    IslemSiraNo = 0,
                    DaraTipNo = "",
                    KDVDahilmi = "0",
                    YerelFiyat = 0,
                    UretimSiraNo = 0,
                    SevkIsEmriNo = 0,
                    SevkiyatPlanNo = 0,
                    Kabul = "H",
                    DokumTarihi = null,
                    Kur = 1,
                    BagliOlduguFisNo = 0,
                    BagliOlduguSiraNo = 0,


                });
            }

        }
        public void PtksStokKartAktar()
        {

            var squery = BllMssql.CustomSql("SELECT * FROM ViewEksikListeStokKart", SuaHelper.defaultSql()).ToList();

            if (squery.Any())
            {

                var eklenecekListe = new List<StokKart>();



                foreach (var item in squery.ToList())
                {
                    var lst = item.Values.ToList();

                    eklenecekListe.Add(new StokKart()
                    {
                        StokTanimAdi = lst[1].ToString(),
                        LotNo = lst[2].ToString(),
                        StokKodu = lst[0].ToString(),
                        //KomponentTanimId = 625,
                        //KomponentTanimGrupId = 28
                    });

                }

                _db.StokKarts.AddRange(eklenecekListe);
                _db.SaveChanges();
            }




        }
        public List<DropItem> DropTakvimTableHeader(DateTime baslangic, DateTime bitis, int gosterimSekli)
        {
            var lst = new List<DropItem>();
            var tarih = baslangic;
            if (gosterimSekli == 0)
            {
                // aylık
                var ayBaslangic = new DateTime(baslangic.Year, baslangic.Month, 1);
                var aybitis = new DateTime(bitis.Year, bitis.Month, 1);
                tarih = ayBaslangic;
                while (tarih <= aybitis)
                {
                    var siradakiTarih = tarih.AddMonths(1);
                    lst.Add(new DropItem()
                    {
                        Tanim = string.Format("{0:MMMM} - {1}", tarih, tarih.Year),
                        DateTime = tarih,
                        DateTime2 = siradakiTarih
                    });
                    tarih = siradakiTarih;
                }
            }
            if (gosterimSekli == 1)
            {
                // haftalık
                while (tarih <= bitis)
                {
                    var siradakiTarih = tarih.AddDays(7);
                    var yilsonu2 = tarih.Year % 2000;
                    lst.Add(new DropItem()
                    {
                        Tanim = string.Format("{0}.H - {1}", SecilenHaftaNoGetir(tarih), yilsonu2),
                        DateTime = tarih,
                        DateTime2 = siradakiTarih
                    });

                    tarih = siradakiTarih;
                }
            }

            if (gosterimSekli == 2)
            {
                // günlük
                while (tarih <= bitis)
                {
                    var siradakiTarih = tarih.AddDays(1);
                    lst.Add(new DropItem()
                    {
                        Tanim = tarih.ToShortDateString(),
                        DateTime = tarih,
                        DateTime2 = siradakiTarih
                    });
                    tarih = siradakiTarih;
                }

            }

            return lst;
        }

        public void DepartmanGetir(int id)
        {

            if (_db.Departmen.Any(a => a.Id == id))
            {
                var dep = _db.Departmen.Find(id);
                Departmans.Add(dep);

                if (dep.BagliOlduguId != 0)
                {
                    DepartmanGetir(dep.BagliOlduguId);
                }
            }

        }

       
        public void BolumGetir(int id)
        {

            var dep = _db.Bolums.Find(id);
            Bolums.Add(dep);

            if (dep.BagliOlduguId != 0)
            {
                BolumGetir(dep.BagliOlduguId);
            }
        }

        public List<DropItem> DropBolumKirilimlariGetir()
        {
            var bolumler = _db.Bolums.OrderBy(a => a.BolumAdi).ToList();
            var DropKirilimliBolumler = new List<DropItem>();
            foreach (var item in bolumler)
            {
                Departmans = new List<Departman>();
                DepartmanGetir(item.DepartmanId);
                Departmans.Reverse();
                if (Departmans.Any())
                {
                    Bolums = new List<Bolum>();
                    BolumGetir(item.Id);
                    Bolums.Reverse();

                    var drop = new DropItem()
                    {
                        Id = item.Id.ToString(),
                        IdInt = item.Id
                    };

                    var str = "";
                    foreach (var dep in Departmans)
                    {
                        str += dep.DepartmanAdi + " >> ";
                    }

                    foreach (var bolum in Bolums)
                    {
                        str += bolum.BolumAdi;
                        if (bolum != Bolums.Last())
                        {
                            str += " >> ";
                        }
                    }
                    drop.Tanim = str;
                    DropKirilimliBolumler.Add(drop);
                }

            }

            return DropKirilimliBolumler.OrderBy(a => a.Tanim).ToList();
        }

        public List<DropItem> DropDepartmanKirilimlariGetir()
        {

            var bagimsizDepartmanlar = _db.ViewKokDepartmanBagimsizs.ToList();

            var kokDepartmanlar = _db.ViewKokDepartmanlars.ToList();

            var DropKirilimliDepartmanlar = new List<DropItem>();

            foreach (var item in bagimsizDepartmanlar)
            {
                DropKirilimliDepartmanlar.Add(new DropItem()
                {
                    Id = item.Id.ToString(),
                    Tanim = item.DepartmanAdi,
                    IdInt = item.Id
                });
            }

            foreach (var item in kokDepartmanlar)
            {
                Departmans = new List<Departman>();
                DepartmanGetir(item.Id);
                Departmans.Reverse();
                if (Departmans.Any())
                {
                   
                    var drop = new DropItem()
                    {
                        Id = item.Id.ToString(),
                        IdInt = item.Id
                    };

                    var str = "";
                    foreach (var dep in Departmans)
                    {
                        str += dep.DepartmanAdi ;
                        if (dep!= Departmans.Last())
                        {
                            str += " >> ";
                        }
                    }

                   
                    drop.Tanim = str;
                    DropKirilimliDepartmanlar.Add(drop);
                }

            }

            return DropKirilimliDepartmanlar.OrderBy(a => a.Tanim).ToList();
        }
        public void KokTechizattanParentOlustur(int id)
        {
            if (id!=0)
            {
                var item = _db.TechizatTuruTanims.Find(id);

                DropItemGenel.Add(new DropItem()
                {
                    Id = item.Id.ToString(),
                    Tanim = item.TechizatTuruTanimAdi,
                    IdInt = item.Id,
                    BagliOlduguId = item.BagliOlduguId
                });
                KokTechizattanParentOlustur(item.BagliOlduguId);
            }
        }
        public List<DropItem> DropKoktenTechizatlariniGetir(int id)
        {
            DropItemGenel = new List<DropItem>();
            
            KokTechizattanParentOlustur(id);
            DropItemGenel.Reverse();
            return DropItemGenel.ToList();
        }
        public List<DropItem> DropTechizatKirilimlar()
        {
            var squery =
                "SELECT dbo.TechizatTuruTanim.Id FROM dbo.TechizatTuruTanim LEFT OUTER JOIN dbo.TechizatTuruTanim AS lst ON dbo.TechizatTuruTanim.Id = lst.BagliOlduguId WHERE(lst.Id IS NULL)";
            var resultDrop = new List<DropItem>();

          

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();
          
            foreach (var aa in result)
            {
                int kokId = (int) aa.Values.ToList()[0];

                
                DropItemGenel = new List<DropItem>();
                KokTechizattanParentOlustur(kokId);
                DropItemGenel.Reverse();
                var str = "";

                foreach (var item in DropItemGenel)
                {
                    str += item.Tanim;
                    if (item!= DropItemGenel.Last())
                    {
                        str += " >> ";
                    }
                }

                resultDrop.Add(new DropItem()
                {
                    Id = kokId.ToString(),
                    Tanim = str,
                    IdInt = kokId
                });

            }
            return resultDrop.OrderBy(a=>a.Tanim).ToList();
        }
        public List<DropItem> DropKomponentTanimlariGetir()
        {
            var lst = new List<DropItem>();

            var kompItems = _db.KomponentTanims.ToList();

            var kompGrups = _db.KomponentTanimGrups.OrderBy(a => a.KomponentTanimGrupAdi).ToList();

            foreach (var grup in kompGrups)
            {
                var araListe = kompItems.Where(a => a.KomponentTanimGrupId == grup.Id).OrderBy(a => a.KomponentTanimAdi)
                    .ToList();

                foreach (var item in araListe)
                {
                    lst.Add(new DropItem()
                    {
                        Id = item.Id.ToString(),
                        IdInt = item.Id,
                        IdInt2 = item.KomponentTanimGrupId,
                        Tanim = string.Format("{0} - {1}", grup.KomponentTanimGrupAdi, item.KomponentTanimAdi)
                    });
                }
            }

            return lst.OrderBy(a => a.Tanim).ToList();



        }
        public List<DropItem> DropSicilTanimlariGetir()
        {
            var lst = new List<DropItem>();

            var sicilItems = _db.SicilOzellikTanims.ToList();

            var sicilHeaders = _db.SicilOzellikHeaderTanims.OrderBy(a => a.SicilOzellikHeaderTanimAdi).ToList();

            foreach (var grup in sicilHeaders)
            {
                var araListe = sicilItems.Where(a => a.SicilOzellikHeaderTanimId == grup.Id).OrderBy(a => a.SicilOzellikTanimAdi)
                    .ToList();

                foreach (var item in araListe)
                {
                    lst.Add(new DropItem()
                    {
                        Id = item.Id.ToString(),
                        IdInt = item.Id,
                        Tanim = string.Format("{0} - {1}", grup.SicilOzellikHeaderTanimAdi, item.SicilOzellikTanimAdi)
                    });
                }
            }

            return lst;



        }
        public List<Departman> Departmans { get; set; }
        public List<Bolum> Bolums { get; set; }
        public DateTime HaftadanGunGetir(int year, int weekOfYear, bool IlkMi)
        {
            DateTime jan1 = new DateTime(year, 1, 1);

            int daysOffset = (int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;

            DateTime firstMonday = jan1.AddDays(daysOffset);

            int firstWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(jan1, CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule, CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);

            if (firstWeek <= 1)
            {
                weekOfYear -= 1;
            }

            var result = firstMonday.AddDays(weekOfYear * 7);
            if (!IlkMi)
            {
                result = result.AddDays(6);
            }
            return result;
        }


        public int SecilenHaftaNoGetir(DateTime time)
        {
            var currentCulture = CultureInfo.CurrentCulture;
            return currentCulture.Calendar.GetWeekOfYear(
                time,
                CalendarWeekRule.FirstDay,
                DayOfWeek.Monday);
        }

        public List<BreadCrumb> TechizatBreadCrumbOlustur(string parentId)
        {
            var breadListe = new List<BreadCrumb>();
            var squery = string.Format(
              "WITH ct(resultParent, resultId) AS (SELECT        BagliOlduguId, Id                                                                          FROM            dbo.TechizatTuruTanim                                                                          WHERE        (BagliOlduguId = {0})                                                                          UNION ALL                                                                          SELECT        TechizatTuruTanim_1.BagliOlduguId, TechizatTuruTanim_1.Id                                                                          FROM            dbo.TechizatTuruTanim AS TechizatTuruTanim_1 INNER JOIN                                                                                                   ct AS ct_2 ON TechizatTuruTanim_1.BagliOlduguId = ct_2.resultId)    SELECT        TOP (100) PERCENT ct_1.resultParent, ct_1.resultId, TechizatTuruTanim_2.TechizatTuruTanimAdi AS ParentTanim, TechizatTuruTanim_3.TechizatTuruTanimAdi     FROM            ct AS ct_1 INNER JOIN                              dbo.TechizatTuruTanim AS TechizatTuruTanim_3 ON ct_1.resultId = TechizatTuruTanim_3.Id LEFT OUTER JOIN                              dbo.TechizatTuruTanim AS TechizatTuruTanim_2 ON ct_1.resultParent = TechizatTuruTanim_2.Id      ORDER BY ct_1.resultParent", parentId);

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();


            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();
                var parentIdd = lst[0]?.ToString();
                var childId = lst[1]?.ToString();
                var childTanim = lst[3]?.ToString();
                var parentTanim = lst[2]?.ToString();

                breadListe.Add(new BreadCrumb()
                {
                    ChildId = childId,
                    ChildName = childTanim,
                    ParentId = parentIdd,
                    ParentName = parentTanim
                });

            }

            return breadListe;
        }


        public bool GenelDurumMailGonder(string ccKullanici, List<string> Gonderilecekler, string mailIcerik, string konu)
        {
            string GonderenMail = "pota_bilgi@polyteks.com.tr";
            string MailSifre = "ptks1986";


            var body = mailIcerik;
            var message = new MailMessage();


            if (Gonderilecekler.Any())
            {
                foreach (var s in Gonderilecekler)
                {
                    message.To.Add(new MailAddress(s));

                }
            }

            if (!string.IsNullOrWhiteSpace(ccKullanici))
            {
                message.CC.Add(new MailAddress(ccKullanici));
            }
            message.From = new MailAddress(GonderenMail);
            message.Subject = konu;
            message.Body = body;
            message.Priority = MailPriority.High;
            message.IsBodyHtml = true;

            try
            {
                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = GonderenMail,
                        Password = MailSifre
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "10.10.1.5";
                    smtp.Port = 25;
                    smtp.EnableSsl = false;
                    smtp.Send(message);
                    return true;
                }
            }
            catch (Exception ex)
            {

                string msg = ex.Message;
                return false;
            }
        }

        #endregion


        public List<int> HaftaGetir(DateTime time)
        {

            var startDate = new DateTime(time.Year, time.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);


            var ilkHafta = SecilenHaftaNoGetir(startDate);
            var secilenHafta = SecilenHaftaNoGetir(time);
            var sonHafta = SecilenHaftaNoGetir(endDate);


            var liste = new List<int>();
            liste.Add(ilkHafta);
            liste.Add(secilenHafta);
            liste.Add(sonHafta);
            return liste;
        }

        // GET: Base
        protected override IAsyncResult BeginExecute(RequestContext requestContext, AsyncCallback callback, object state)
        {
            var KullaniciLinkleri = new List<UserHeaderItem>();
            var httpContext = requestContext.HttpContext;

            User = new User();

            var LayoutModel = new LayoutModel();

            _db = new POTA_BOTAEntities();

            

            #region Current Link

            var areaList = requestContext.RouteData.DataTokens.ToList();
            string controllerName = requestContext.RouteData.Values["controller"].ToString();
            string actionName = requestContext.RouteData.Values["action"].ToString();

            string id = "";
            if (requestContext.RouteData.Values.Count > 2)
            {
                var strId = requestContext.RouteData.Values["id"].ToString();

            }

            string areaName = "";
            if (areaList.Any())
            {
                areaName = areaList[1].Value.ToString();
            }


            var currentLink = new BaseUserLink
            {
                AreaName = areaName,
                ControllerName = controllerName,
                ActionName = actionName,
                LinkGrupTanimAdi = id
            };

            #endregion

            #region Default Dil Belirleme
            if (httpContext.Session["lang"] == null)
            {
                httpContext.Session["lang"] = "tr";
            }

            LayoutModel.LangShortDef = (string)httpContext.Session["lang"];
            #endregion

            
           

            #region Kullanıcı Kontrol

            bool UserSessionStatus = httpContext.Session["kul"] != null;
            if (UserSessionStatus)
            {
                User = (User)httpContext.Session["kul"];
            }
            else
            {
                // User = _db.Users.Find(2);
                var reqCookie = requestContext.HttpContext.Request.Cookies["sua"];
                if (reqCookie != null)
                {

                    int decodedUserId = Convert.ToInt32(reqCookie.Value);

                    User = this._db.Users.First(a => a.Id == decodedUserId);
                    httpContext.Session["kul"] = User;


                }

            }
            #endregion

            LayoutModel.User = User;

            if (httpContext.Session["kulLink"] != null)
            {
                KullaniciLinkleri = (List<UserHeaderItem>)httpContext.Session["kulLink"];
            }
            else
            {
                KullaniciLinkleri = KullaniciLinkleriOlustur();

                if (KullaniciLinkleri.Any())
                {
                    httpContext.Session["kulLink"] = KullaniciLinkleri;
                }

            }

            LayoutModel.UserHeaderItemListe = KullaniciLinkleri;
            LayoutModel.ResimUrl = User.ImageUrl;

            LayoutModel.Haftalar = HaftaGetir(DateTime.Now);

            LayoutModel.Gunler.Add(HaftadanGunGetir(DateTime.Now.Year, LayoutModel.Haftalar[0], true));
            LayoutModel.Gunler.Add(DateTime.Now);
            LayoutModel.Gunler.Add(HaftadanGunGetir(DateTime.Now.Year, LayoutModel.Haftalar[2], false));



            ViewBag.LayoutModel = LayoutModel;

            LangShortDef = LayoutModel.LangShortDef;

            httpContext.Session["unauthorize"] = null;

            var baseLinkListe = new List<SystemBaseUrlLink>();
            if (httpContext.Session["baseUrlLink"] == null)
            {
                httpContext.Session["baseUrlLink"] = _db.SystemBaseUrlLinks.ToList();
            }

            baseLinkListe = (List<SystemBaseUrlLink>)httpContext.Session["baseUrlLink"];
            if (!LinkErisim(currentLink, KullaniciLinkleri, baseLinkListe))
            {
                httpContext.Session["unauthorize"] = "hopp";
            }

            return base.BeginExecute(requestContext, callback, state);
        }
    }
}