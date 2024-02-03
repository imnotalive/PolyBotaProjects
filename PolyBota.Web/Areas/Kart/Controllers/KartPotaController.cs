using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using System.Web.Mvc;
using PolyBota.BLL;
using PolyBota.DAL;
using PolyBota.Entities;
using PolyBota.Helpers;
using PolyBota.Web.Areas.Kart.Models;
using PolyBota.Web.Controllers;

namespace PolyBota.Web.Areas.Kart.Controllers
{
    public class KartPotaController : BaseController
    {
        // GET: Kart/KartPota

        private void techizatTuruTanimsizPotakartlariDegistir()
        {
            var squery =
                "SELECT        dbo.PotaKart.Id, dbo.PotaKart.PotaKartAdi FROM            dbo.PotaKart LEFT OUTER JOIN                         dbo.TechizatTuruTanim ON dbo.PotaKart.TechizatTuruId = dbo.TechizatTuruTanim.Id WHERE        (dbo.TechizatTuruTanim.Id IS NULL)";

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

            if (result.Any())
            {
                foreach (var aa in result)
                {
                    var potaKartId = (int)aa.Values.ToList()[0];
                    var potaKart = _db.PotaKarts.Find(potaKartId);

                    //techizat türü diğer Id =4;

                    potaKart.TechizatTuruId = 4;
                    _db.SaveChanges();
                }
            }
        }
        public KartPotaModel TakvimeGoreDurusModelGetir(KartPotaModel model)
        {



            var stokKart = _db.PotaKarts.Find(model.PotaKart.Id);

            model.PotaKart = stokKart;
            var header = DropTakvimTableHeader(model.BaslangisTarihi, model.BitisTarihi, model.GosterimSekli);

            var baslangic = model.BaslangisTarihi.Date;
            var bitis = model.BitisTarihi.AddDays(1).Date;
            var duruslar = _db.StokKartDurus
                .Where(a => a.StokKartId == stokKart.Id && a.DurusBaslangic >=
                    baslangic && a.DurusBaslangic < bitis).OrderBy(a => a.DurusBaslangic)
                .ToList();




            model.StokKartDurus = duruslar;

            model.TableHeaderlar = header;


            model.ArrtTable = new int[1, header.Count, duruslar.Count];

            int i = -1;

            foreach (var dropItem in header)
            {

                i++;
                int j = -1;
                var lst = duruslar.Where(a =>
                    a.DurusBaslangic >= dropItem.DateTime &&
                    a.DurusBaslangic < dropItem.DateTime2).ToList();

                foreach (var operasyon in lst)
                {
                    j++;
                    model.ArrtTable[0, i, j] = operasyon.Id;
                }
            }

            return model;
        }
        public KartPotaModel TakvimeGoreBakimModelGetir(KartPotaModel model)
        {
            var stokKart = _db.PotaKarts.Find(model.PotaKart.Id);

            model.PotaKart = stokKart;
            var header = DropTakvimTableHeader(model.BaslangisTarihi, model.BitisTarihi, model.GosterimSekli);

            var baslangic = model.BaslangisTarihi.Date;
            var bitis = model.BitisTarihi.AddDays(1).Date;
            var operasyonlar = _db.StokKartOperasyons
                .Where(a => a.StokKartId == stokKart.Id && a.PlanlananTarih >=
                    baslangic && a.PlanlananTarih < bitis).OrderBy(a => a.PlanlananTarih)
                .ToList();

            foreach (var operasyon in operasyonlar)
            {
                if (model.KomponentTalimatGrups.Count(a => a.Id == operasyon.KomponentTalimatGrupId) == 0)
                {
                    model.KomponentTalimatGrups.Add(_db.KomponentTalimatGrups.Find(operasyon.KomponentTalimatGrupId));
                }
            }
            model.KomponentTalimatGrups = model.KomponentTalimatGrups.OrderBy(a => a.KomponentTalimatGrupAdi).ToList();

            model.StokKartOperasyons = operasyonlar;

            model.TableHeaderlar = header;


            model.ArrtTable = new int[model.KomponentTalimatGrups.Count, header.Count, operasyonlar.Count];

            int i = -1;

            foreach (var grup in model.KomponentTalimatGrups)
            {
                i++;
                int j = -1;
                foreach (var dropItem in header)
                {
                    j++;

                    int k = -1;
                    var lst = operasyonlar.Where(a =>
                        a.KomponentTalimatGrupId == grup.Id && a.PlanlananTarih >= dropItem.DateTime &&
                        a.PlanlananTarih < dropItem.DateTime2).ToList();

                    foreach (var operasyon in lst)
                    {
                        k++;
                        model.ArrtTable[i, j, k] = operasyon.Id;
                    }
                }
            }

            return model;
        }


        public ActionResult PotaKartListe(int CurrentPage = 1, int PageShowCount = 150, int katId = 0)
        {
            techizatTuruTanimsizPotakartlariDegistir();
            var KomponentTanimGrups = _db.KomponentTanimGrups.OrderBy(a => a.KomponentTanimGrupAdi).ToList();
            var KomponentTanims = _db.KomponentTanims.OrderBy(a => a.KomponentTanimAdi).ToList();
            var secilenTechizat = new TechizatTuruTanim();
            if (katId != 0)
            {
                secilenTechizat = _db.TechizatTuruTanims.Find(katId);
            }
            else
            {
                secilenTechizat = _db.TechizatTuruTanims.First(a => a.BagliOlduguId == 0);
                katId = secilenTechizat.Id;
            }


            var ustTechizatlar = _db.TechizatTuruTanims.Where(a => a.BagliOlduguId == secilenTechizat.BagliOlduguId).ToList();



            var breadCrumbs = new List<BreadCrumb>();


            var model = new KartPotaModel()
            {
                KatId = katId,
                KomponentTanimGrups = KomponentTanimGrups,
                KomponentTanims = KomponentTanims
            };
            model.DropItems = DropTechizatKirilimlar();



            foreach (var techizatTuruTanim in ustTechizatlar)
            {
                var strTanim = techizatTuruTanim.TechizatTuruTanimAdi;
                if (model.DropItems.Any(a => a.IdInt == techizatTuruTanim.Id))
                {
                    strTanim = model.DropItems.First(a => a.IdInt == techizatTuruTanim.Id).Tanim;
                }

                breadCrumbs.Add(new BreadCrumb()
                {
                    ChildId = techizatTuruTanim.Id.ToString(),
                    ChildName = strTanim
                });
            }

            breadCrumbs = breadCrumbs.OrderBy(a => a.ChildName).ToList();
            var squery = string.Format(
                "WITH ct(resultParent, resultId) AS (SELECT        BagliOlduguId, Id FROM dbo.TechizatTuruTanim WHERE        (Id = {0})                                                                          UNION ALL                                                                          SELECT        TechizatTuruTanim_1.BagliOlduguId, TechizatTuruTanim_1.Id                                                                          FROM            dbo.TechizatTuruTanim AS TechizatTuruTanim_1 INNER JOIN                                                                                                   ct AS ct_2 ON TechizatTuruTanim_1.BagliOlduguId = ct_2.resultId)    SELECT        TOP (100) PERCENT dbo.PotaKart.Id     FROM            ct AS ct_1 INNER JOIN                              dbo.PotaKart ON ct_1.resultId = dbo.PotaKart.TechizatTuruId ORDER BY dbo.PotaKart.PotaKartAdi",
                secilenTechizat.Id);

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();
            var itemIdler = new List<int>();
            ;
            foreach (var aa in result)
            {
                var itemId = (int)aa.Values.ToList()[0];
                itemIdler.Add(itemId);
            }

            var pliste = new PagedListBase<int>() { CurrentPage = CurrentPage, PageShowCount = PageShowCount, DataLists = itemIdler };

            var PageListBase = PageListBaseOlustur(pliste);

            model.PagedListSrcn = new PagedListSrcn()
            {
                PageShowCount = PageListBase.PageShowCount,
                PageSizeSelectList = PageListBase.PageSizeSelectList,
                PageNumberList = PageListBase.PageNumberList,
                CurrentPage = PageListBase.CurrentPage
            };

            foreach (var i in PageListBase.DataLists)
            {
                model.PotaKarts.Add(_db.PotaKarts.Find(i));
            }

            model.Bolums = _db.Bolums.ToList();
            model.BreadCrumbs = breadCrumbs;

            return View(model);
        }

        public PartialViewResult PotaKartAra2(string str)
        {

            var model = new KartPotaModel();

            if (!string.IsNullOrEmpty(str))
            {
                var sorgu = string.Format(
                    "SELECT TOP 10 Id, PotaKartAdi, PotaKodu FROM dbo.PotaKart WHERE (PotaKartAdi LIKE '%{0}%') OR (PotaKodu LIKE '%{0}%')",
                    str);

                var result = BllMssql.CustomSql(sorgu, SuaHelper.defaultSql()).ToList();

                foreach (var squery in result.ToList())
                {
                    var lst = squery.Values.ToList();

                    try
                    {
                        int id = Convert.ToInt32(lst[0]);
                        var StokTanimAdi = lst[1]?.ToString();
                        var StokKodu = lst[2]?.ToString();

                        model.PotaKarts.Add(new PotaKart()
                        {
                            Id = id,

                            PotaKartAdi = StokTanimAdi,
                            PotaKodu = StokKodu
                        });
                    }
                    catch (Exception e)
                    {
                        var aa = e.Message;
                    }

                }
            }


            return PartialView("_PotaKartAra2", model);
        }


        public ActionResult BreadTechizatListe()
        {




            var model = new KartPotaModel()
            {
                BreadCrumbs = TechizatBreadCrumbOlustur("0")
            };
            return View(model);
        }

        public JsonResult SilmeTalebiGuncelle(int id, int durumId)
        {

            var potakart = _db.PotaKarts.Find(id);

            potakart.SilinsinMi = durumId;
            _db.SaveChanges();

            TempDataCreate(2);

            return new JsonResult { Data = new { Id = id }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        public ActionResult PotaKartOperasyonSilmeListe(int CurrentPage = 1, int PageShowCount = 150, int katId = 0)
        {
            var model = new KartPotaModel();
            var squery =
                "SELECT DISTINCT TOP (100) PERCENT dbo.StokKartOperasyon.StokKartId, dbo.PotaKart.PotaKartAdi FROM            dbo.StokKartOperasyon INNER JOIN                         dbo.PotaKart ON dbo.StokKartOperasyon.StokKartId = dbo.PotaKart.Id ORDER BY dbo.PotaKart.PotaKartAdi";


            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();

                model.PotaKarts.Add(new PotaKart()
                {
                    Id = (int)lst[0],
                    PotaKartAdi = lst[1]?.ToString()
                });
            }


            squery =
                "SELECT        TOP (100) PERCENT dbo.StokKartOperasyon.Id, dbo.StokKartOperasyon.StokKartId, dbo.StokKartOperasyon.OperasyonDurumu, dbo.StokKartOperasyon.SilinsinMi, dbo.PotaKart.PotaKartAdi,                          dbo.StokKartOperasyon.PlanlananTarih, dbo.KomponentTalimatGrup.KomponentTalimatGrupAdi FROM            dbo.StokKartOperasyon INNER JOIN                         dbo.PotaKart ON dbo.StokKartOperasyon.StokKartId = dbo.PotaKart.Id INNER JOIN                         dbo.KomponentTalimatGrup ON dbo.StokKartOperasyon.KomponentTalimatGrupId = dbo.KomponentTalimatGrup.Id";

            if (katId != 0)
            {
                squery = string.Format("{0} WHERE        (dbo.StokKartOperasyon.StokKartId = {1}) ", squery, katId);
            }


            squery +=
                " ORDER BY dbo.PotaKart.PotaKartAdi, dbo.KomponentTalimatGrup.KomponentTalimatGrupAdi, dbo.StokKartOperasyon.PlanlananTarih DESC";


            result = BLL.BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();
            var drops = new List<DropItem>();
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

                drops.Add(new DropItem()
                {
                    DateTime = PlanlananTarih,
                    Tanim1 = PotaKartAdi,
                    Tanim2 = KomponentTalimatGrupAdi,
                    Id = Id.ToString(),
                    IdInt = StokKartId,
                    IdInt2 = OperasyonDurumu,
                    IdInt3 = SilinsinMi
                });

            }

            var pliste = new PagedListBase<DropItem>() { CurrentPage = CurrentPage, PageShowCount = PageShowCount, DataLists = drops };

            var PageListBase = PageListBaseOlustur(pliste);
            model.PagedListSrcn = new PagedListSrcn()
            {
                PageShowCount = PageListBase.PageShowCount,
                PageSizeSelectList = PageListBase.PageSizeSelectList,
                PageNumberList = PageListBase.PageNumberList,
                CurrentPage = PageListBase.CurrentPage
            };
            model.DropItems = pliste.DataLists;
            return View(model);
        }


        

        public JsonResult OperasyonSilmeTalebiGuncelle(int id, int durumId)
        {

            var operasyon = _db.StokKartOperasyons.Find(id);

            operasyon.SilinsinMi = durumId;
            _db.SaveChanges();

            TempDataCreate(2);

            return new JsonResult { Data = new { Id = id }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #region Pota TEMEL BİLGİ

        public ActionResult PotaKartTemelBilgi(int id = 0)
        {
            var model = new KartPotaModel()
            {
                //TechizatTuruTanims = _db.TechizatTuruTanims.ToList(),
                SicilOzellikHeaderTanims = _db.SicilOzellikHeaderTanims.OrderBy(a => a.SicilOzellikHeaderTanimAdi).ToList(),
                DropKomponentler = DropKomponentTanimlariGetir(),
                SicilOzellikTanims = _db.SicilOzellikTanims.ToList()
            };
            model.TechizatTuruTanims.Add(new TechizatTuruTanim() { Id = 0, TechizatTuruTanimAdi = "Seçiniz" });

            var dropTechizatTurus = DropTechizatKirilimlar();

            foreach (var item in dropTechizatTurus)
            {
                model.TechizatTuruTanims.Add(new TechizatTuruTanim()
                {
                    Id = item.IdInt,
                    TechizatTuruTanimAdi = item.Tanim
                });
            }
            var bolumler = DropBolumKirilimlariGetir();
            model.DropBolumler = bolumler;
            bolumler.Add(new DropItem() { Id = "0", Tanim = "Seçiniz" });

            if (id != 0)
            {
                model.PotaKart = _db.PotaKarts.Find(id);
                model.PotaKartSicilOzelliks = _db.PotaKartSicilOzelliks.Where(a => a.StokKartId == id).ToList();
                foreach (var item in model.SicilOzellikTanims)
                {
                    if (model.PotaKartSicilOzelliks.Any(a => a.SicilOzellikId == item.Id))
                    {
                        var str = model.PotaKartSicilOzelliks.First(a => a.SicilOzellikId == item.Id).OzellikDegeri;
                        item.BaseValue = str;
                        item.SeciliMi = true;
                    }
                }

                model.Medya = new Medya() { BagliOlduguId = id };
                model.Medyas = _db.Medyas.Where(a => a.BagliOlduguTip == 0 && a.BagliOlduguId == id)
                    .OrderBy(a => a.MedyaAdi).ToList();
            }

            model.tabId = 1;
            return View(model);
        }


        [HttpPost]
        public JsonResult PotaKartTemelBilgi(KartPotaModel model)
        {
            int state = 2;
            int tabId = 0;

            var potaKart = model.PotaKart;

            if (potaKart.PotaKartAdi == null)
            {
                state = 0;
            }
            else
            {
                //var komp = _db.KomponentTanims.Find(potaKart.KomponentTanimId);
                if (potaKart.Id == 0)
                {


                    var yeniItem = new PotaKart()
                    {
                        //KomponentTanimGrupId = komp.KomponentTanimGrupId,
                        //KomponentTanimId = komp.Id,
                        BolumId = potaKart.BolumId,
                        PotaKartAdi = potaKart.PotaKartAdi,
                        PotaKodu = potaKart.PotaKodu,
                        TechizatTuruId = potaKart.TechizatTuruId
                    };
                    _db.PotaKarts.Add(yeniItem);
                    _db.SaveChanges();
                    potaKart.Id = yeniItem.Id;
                }
                else
                {
                    var editItem = _db.PotaKarts.Find(potaKart.Id);
                    //editItem.KomponentTanimGrupId = komp.KomponentTanimGrupId;
                    //editItem.KomponentTanimId = komp.Id;
                    editItem.BolumId = potaKart.BolumId;
                    editItem.PotaKartAdi = potaKart.PotaKartAdi;
                    editItem.PotaKodu = potaKart.PotaKodu;
                    editItem.TechizatTuruId = potaKart.TechizatTuruId;
                    _db.SaveChanges();
                }

                TempDataCreate(2);
            }


            return new JsonResult { Data = new { Id = potaKart.Id, state = state, tabId = tabId }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion

        #region Pota Kart Sicil Bilgi
        [HttpPost]
        public ActionResult PotaKartSicilBilgi(KartPotaModel model)
        {
            int state = 2;
            int tabId = 0;

            var potaKart = model.PotaKart;

            var seciliSiciller = model.SicilOzellikTanims.Where(a => a.SeciliMi).ToList();

            var olmasiGerekenSiciller = seciliSiciller.Select(a => new PotaKartSicilOzellik()
            {
                StokKartId = potaKart.Id,
                OzellikDegeri = a.BaseValue,
                SicilOzellikId = a.Id
            });

            var silinecekItemlar = new List<PotaKartSicilOzellik>();
            var eklenecekItemlar = new List<PotaKartSicilOzellik>();
            var kayitliItemlar = _db.PotaKartSicilOzelliks.Where(a => a.StokKartId == potaKart.Id).ToList();


            foreach (var item in olmasiGerekenSiciller)
            {
                if (kayitliItemlar.Count(a => a.SicilOzellikId == item.SicilOzellikId) == 0)
                {
                    eklenecekItemlar.Add(item);
                }
            }

            foreach (var item in kayitliItemlar)
            {
                if (olmasiGerekenSiciller.Count(a => a.SicilOzellikId == item.SicilOzellikId) == 0)
                {
                    silinecekItemlar.Add(item);
                }
            }

            if (eklenecekItemlar.Any())
            {
                _db.PotaKartSicilOzelliks.AddRange(eklenecekItemlar);
                _db.SaveChanges();
            }
            if (silinecekItemlar.Any())
            {
                _db.PotaKartSicilOzelliks.RemoveRange(silinecekItemlar);
                _db.SaveChanges();
            }


            var guncelListe = _db.PotaKartSicilOzelliks.Where(a => a.StokKartId == potaKart.Id).ToList();

            foreach (var item in guncelListe)
            {
                if (olmasiGerekenSiciller.Any(a => a.SicilOzellikId == item.SicilOzellikId))
                {
                    var deger = olmasiGerekenSiciller.First(a => a.SicilOzellikId == item.SicilOzellikId).OzellikDegeri;

                    item.OzellikDegeri = deger;
                    _db.SaveChanges();


                }

            }

            TempDataCreate(2);
            return RedirectToAction("PotaKartTemelBilgi", "KartPota", new { id = potaKart.Id });
        }


        #endregion

        #region Pota Kart Techizat Bilgi
        public ActionResult PotaKartTechizatBilgi(int id = 0)
        {
            var potaKart = _db.PotaKarts.Find(id);

            if (potaKart.TechizatTuruId == 0)
            {
                TempDataCustom(1, "Lütfen Techizat Tipini Seçiniz");
                return RedirectToAction("PotaKartTemelBilgi", "KartPota", new { id = id });
            }
            var model = new KartPotaModel()
            {
                PotaKart = potaKart,
            };

            var techizatBilesenHeaderlar = _db.TechizatTuruBilesenHeaders
                .Where(a => a.TechizatTuruTanimId == potaKart.TechizatTuruId).Select(a => a.BilesenHeaderId).ToList();

            var seciliBilesenItems = _db.PotaKartBilesenItems.Where(a => a.PotaKartId == id).ToList();
            if (techizatBilesenHeaderlar.Any())
            {
                foreach (int i in techizatBilesenHeaderlar)
                {
                    model.BilesenHeaders.Add(_db.BilesenHeaders.Find(i));
                    model.BilesenItems.AddRange(_db.BilesenItems.Where(a => a.BilesenHeaderId == i));
                }

            }

            model.BilesenItems = model.BilesenItems.OrderBy(a => a.Id).ToList();
            model.BilesenHeaders = model.BilesenHeaders.OrderBy(a => a.BilesenHeaderAdi).ToList();


            foreach (var item in model.BilesenHeaders)
            {
                var arItem = new DropItem()
                {
                    Id = item.Id.ToString(),
                    Tanim = item.BilesenHeaderAdi
                };
                arItem.ItemValues.AddRange(model.BilesenItems.Where(a => a.BilesenHeaderId == item.Id).Select(a => new ItemValue() { IdInt = a.Id, Text = a.BilesenDegeri }));
                model.DropTechizatTopluGiris.Add(arItem);
            }
            foreach (var item in model.BilesenItems)
            {
                if (seciliBilesenItems.Any(a => a.BilesenItemId == item.Id))
                {
                    item.SeciliMi = true;
                }
            }
            model.tabId = 2;
            return View(model);
        }
        [HttpPost]
        public ActionResult PotaKartTechizatBilgi(KartPotaModel model)
        {
            var potaKart = model.PotaKart;
            var seciliBilesenItemIdler = model.BilesenItems.Where(a => a.SeciliMi).Select(a => a.Id).ToList();

            var kayitliBilesenler = _db.PotaKartBilesenItems.Where(a => a.PotaKartId == potaKart.Id).ToList();
            var olmasiGerekenBilesenler = new List<PotaKartBilesenItem>();
            var silinecekBilesenler = new List<PotaKartBilesenItem>();
            var eklenecekBilesenler = new List<PotaKartBilesenItem>();
            if (seciliBilesenItemIdler.Any())
            {
                olmasiGerekenBilesenler = seciliBilesenItemIdler.Select(a => new PotaKartBilesenItem()
                { PotaKartId = potaKart.Id, BilesenItemId = a }).ToList();
            }

            foreach (var item in olmasiGerekenBilesenler)
            {
                if (kayitliBilesenler.Count(a => a.BilesenItemId == item.BilesenItemId) == 0)
                {
                    eklenecekBilesenler.Add(item);
                }
            }
            foreach (var item in kayitliBilesenler)
            {
                if (olmasiGerekenBilesenler.Count(a => a.BilesenItemId == item.BilesenItemId) == 0)
                {
                    silinecekBilesenler.Add(item);
                }
            }

            if (silinecekBilesenler.Any())
            {
                _db.PotaKartBilesenItems.RemoveRange(silinecekBilesenler);
                _db.SaveChanges();
            }
            if (eklenecekBilesenler.Any())
            {
                _db.PotaKartBilesenItems.AddRange(eklenecekBilesenler);
                _db.SaveChanges();
            }
            TempDataCreate(2);

            return RedirectToAction("PotaKartTechizatBilgi", "KartPota", new { id = potaKart.Id });
        }


        [HttpPost]
        public ActionResult PotaKartTopluTechizatEkle(KartPotaModel model)
        {
            var potaKart = model.PotaKart;
            var kayitliBilesenler = _db.PotaKartBilesenItems.Where(a => a.PotaKartId == potaKart.Id).ToList();

            _db.PotaKartBilesenItems.RemoveRange(kayitliBilesenler);
            _db.SaveChanges();

            var itemIdler = model.DropTechizatTopluGiris.Select(a => a.IdInt).Distinct().ToList();


            var secilenBilesemItemlr = _db.BilesenItems.Where(a => itemIdler.Any(b => b == a.Id)).ToList();


            var lst = new List<PotaKartBilesenItem>();


            foreach (var bilesenItem in secilenBilesemItemlr)
            {
                var araListe = _db.BilesenItems
                    .Where(a => a.BilesenHeaderId == bilesenItem.BilesenHeaderId && a.Id <= bilesenItem.Id).ToList();
                lst.AddRange(araListe.Select(a => new PotaKartBilesenItem()
                {
                    PotaKartId = potaKart.Id,
                    BilesenItemId = a.Id
                }));
            }

            _db.PotaKartBilesenItems.AddRange(lst);
            _db.SaveChanges();

            TempDataCreate(2);
            return RedirectToAction("PotaKartTechizatBilgi", "KartPota", new { id = potaKart.Id });
        }
        #endregion

        #region Pota Kart EntegreKartlat

        public ActionResult PotaKartEntegreKartlar(int id = 0)
        {
            var potaKart = _db.PotaKarts.Find(id);

            if (potaKart.TechizatTuruId == 0)
            {
                TempDataCustom(1, "Lütfen Techizat Tipini Seçiniz");
                return RedirectToAction("PotaKartTemelBilgi", "KartPota", new { id = id });
            }
            var model = new KartPotaModel()
            {
                PotaKart = potaKart,

            };
            var entegreKartlar = _db.PotaKartEntegreKarts.Where(a => a.PotaKartId == id).ToList();

            foreach (var item in entegreKartlar)
            {
                if (_db.PotaKarts.Any(a => a.Id == item.EntegrePotaKartId))
                {
                    model.PotaKarts.Add(_db.PotaKarts.Find(item.EntegrePotaKartId));
                }
            }

            model.PotaKartEntegreKarts = entegreKartlar;
            model.PotaKarts = model.PotaKarts.OrderBy(a => a.PotaKartAdi).ToList();
            model.tabId = 7;
            return View(model);
        }

        public PartialViewResult PotaKartEntegreIcinAra(int id, string str)
        {

            var model = new KartPotaModel()
            {
                PotaKart = new PotaKart() { Id = id }
            };
            var sorgu = string.Format(
                "SELECT TOP 50 Id, PotaKartAdi, PotaKodu FROM dbo.PotaKart WHERE (PotaKartAdi LIKE '%{0}%') OR (PotaKodu LIKE '%{0}%')",
                str);

            var result = BllMssql.CustomSql(sorgu, SuaHelper.defaultSql()).ToList();

            foreach (var squery in result.ToList())
            {
                var lst = squery.Values.ToList();

                try
                {
                    var potaKartId = lst[0]?.ToString();
                    var StokTanimAdi = lst[1]?.ToString();
                    var StokKodu = lst[2]?.ToString();

                    model.DropItems.Add(new DropItem()
                    {
                        Id = potaKartId,
                        Tanim = string.IsNullOrWhiteSpace(StokKodu) ? StokTanimAdi : string.Format("{0} ({1})", StokTanimAdi, StokKodu)
                    });
                }
                catch (Exception e)
                {
                    var aa = e.Message;
                }

            }

            return PartialView("_PotaKartEntegreIcinSonuc", model);
        }


        [HttpPost]
        public ActionResult PotaKartEntegreKaydet(KartPotaModel model)
        {
            int potaKartId = model.PotaKart.Id;

            var lst = model.DropItems.Where(a => a.SeciliMi).Select(a => a.Id).ToList();


            var eklenecekListe = new List<PotaKartEntegreKart>();

            foreach (var i in lst)
            {
                eklenecekListe.Add(new PotaKartEntegreKart()
                {
                    PotaKartId = potaKartId,
                    EntegrePotaKartId = Convert.ToInt32(i)
                });
            }

            if (eklenecekListe.Any())
            {

                var kayitliListe = _db.PotaKartEntegreKarts.Where(a => a.PotaKartId == potaKartId).ToList()
                    .Select(a => a.EntegrePotaKartId).Distinct().ToList();

                foreach (var i in kayitliListe)
                {
                    eklenecekListe = eklenecekListe.Where(a => a.EntegrePotaKartId != i).ToList();
                }
                _db.PotaKartEntegreKarts.AddRange(eklenecekListe);
                _db.SaveChanges();

                TempDataCreate(2);
            }

            return RedirectToAction("PotaKartEntegreKartlar", "KartPota", new { id = potaKartId });
        }


        public ActionResult PotaKartEntegreSil(int id = 0)
        {
            var potaKartEntegre = _db.PotaKartEntegreKarts.Find(id);
            var idd = potaKartEntegre.PotaKartId;
            _db.PotaKartEntegreKarts.Remove(potaKartEntegre);
            _db.SaveChanges();
            TempDataCreate(3);
            return RedirectToAction("PotaKartEntegreKartlar", "KartPota", new { id = idd });
        }

        #endregion

        #region Pota Kart Ağaç Bilgi
        public ActionResult PotaKartAgacBilgi(int id = 0)
        {
            TempData["theme"] = "gizle";
            var potaKart = _db.PotaKarts.Find(id);


            var model = new KartPotaModel()
            {
                PotaKart = potaKart,
                PotaKartAgacs = _db.PotaKartAgacs.Where(a => a.PotaKartId == id).ToList()
            };

            var itemIdler = model.PotaKartAgacs.Select(a => a.PotaKartIdChild).Distinct().ToList();
            itemIdler.AddRange(model.PotaKartAgacs.Select(a => a.PotaKartIdParent).Distinct().ToList());

            itemIdler = itemIdler.Distinct().ToList();

            model.PotaKarts = _db.PotaKarts.Where(a => itemIdler.Any(b => b == a.Id)).OrderBy(a => a.PotaKartAdi)
                .ToList();
            model.tabId = 3;
            return View(model);
        }


        public PartialViewResult PotaKartAra(string str)
        {

            var model = new KartPotaModel();
            var sorgu = string.Format(
                "SELECT TOP 12 Id, PotaKartAdi, PotaKodu FROM dbo.PotaKart WHERE (PotaKartAdi LIKE '%{0}%') OR (PotaKodu LIKE '%{0}%')",
                str);

            var result = BllMssql.CustomSql(sorgu, SuaHelper.defaultSql()).ToList();

            foreach (var squery in result.ToList())
            {
                var lst = squery.Values.ToList();

                try
                {
                    int id = Convert.ToInt32(lst[0]);
                    var StokTanimAdi = lst[1]?.ToString();
                    var StokKodu = lst[2]?.ToString();

                    model.PotaKarts.Add(new PotaKart()
                    {
                        Id = id,

                        PotaKartAdi = StokTanimAdi,
                        PotaKodu = StokKodu
                    });
                }
                catch (Exception e)
                {
                    var aa = e.Message;
                }

            }

            return PartialView("_PotaKartAra", model);
        }

        public ActionResult PotaKartAgacOlustur(int id, int childId, int parentId)
        {

            if (parentId == -1)
            {
                // silinecek
                var lst = _db.PotaKartAgacs.Where(a => a.PotaKartId == id && a.PotaKartIdChild == childId).ToList();
                _db.PotaKartAgacs.RemoveRange(lst);
                _db.SaveChanges();
            }
            else
            {
                _db.PotaKartAgacs.Add(new PotaKartAgac()
                {
                    PotaKartId = id,
                    PotaKartIdChild = childId,
                    PotaKartIdParent = parentId
                });
                _db.SaveChanges();
            }

            return RedirectToAction("PotaKartAgacBilgi", "KartPota", new { id = id });
        }


        #endregion

        #region Pota Kart Operasyonlar

        public ActionResult PotaKartOperasyon(int id = 0)
        {
            TempData["theme"] = "gizle";
            var potaKart = _db.PotaKarts.Find(id);


            var model = new KartPotaModel()
            {
                PotaKart = potaKart,
                PotaKartAgacs = _db.PotaKartAgacs.Where(a => a.PotaKartId == id).ToList(),
                StokKartOperasyon = new StokKartOperasyon() { PlanlananTarih = DateTime.Now },
                DropKomponentler = _db.KomponentTalimatGrups.OrderBy(a => a.KomponentTalimatGrupAdi).ToList().Select(a => new DropItem()
                {
                    Tanim = a.KomponentTalimatGrupAdi,
                    Id = a.Id.ToString()
                }).ToList()
            };
            model = TakvimeGoreBakimModelGetir(model);



            model.tabId = 5;
            return View(model);
        }

        [HttpPost]
        public PartialViewResult PotaKartOperasyon(KartPotaModel model)
        {
            return PartialView("_OperasyonListesi", TakvimeGoreBakimModelGetir(model));
        }

        [HttpPost]
        public ActionResult KartOperasyonEkle(KartPotaModel model)
        {
            var operasyon = model.StokKartOperasyon;

            var yeniItem = new StokKartOperasyon()
            {
                KomponentTalimatGrupId = operasyon.KomponentTalimatGrupId,
                StokKartId = model.PotaKart.Id,
                OperasyonDurumu = 0,
                PlanlananTarih = operasyon.PlanlananTarih,
                IlkPlanlananTarih = operasyon.PlanlananTarih,
                PlanlananHafta = 0,
                GerceklesenTarih = operasyon.PlanlananTarih,
                KayitYapanId = User.Id,
                YapilanTalimatlarStr = "",
                GerceklesenHafta = 0
            };
            _db.StokKartOperasyons.Add(yeniItem);
            _db.SaveChanges();
            TempDataCreate(1);
            return RedirectToAction("PotaKartOperasyon", "KartPota", new { id = model.PotaKart.Id });
        }

        [HttpPost]

        public ActionResult KartOperasyonPeriyodikEkle(KartPotaModel model)
        {
            var operasyon = model.StokKartOperasyon;

            var tekrarSayisi = model.DropPeriyodikOperasyonItem.IdInt2;
            var kompTalimat = _db.KomponentTalimatGrups.Find(operasyon.KomponentTalimatGrupId);
            var periyot = _db.PeriyotTanims.Find(kompTalimat.PeriyotTanimId);
            var periyotDonemi = periyot.PeriyotDonemi * 7;

            var liste = new List<StokKartOperasyon>();


            for (int i = 0; i < tekrarSayisi; i++)
            {
                var oteleme = i * periyotDonemi;

                var tarih = operasyon.PlanlananTarih.AddDays(oteleme);

                var yeniItem = new StokKartOperasyon()
                {

                    KomponentTalimatGrupId = operasyon.KomponentTalimatGrupId,
                    StokKartId = model.PotaKart.Id,
                    OperasyonDurumu = 0,
                    PlanlananTarih = tarih,
                    IlkPlanlananTarih = tarih,
                    PlanlananHafta = 0,
                    GerceklesenTarih = tarih,
                    KayitYapanId = User.Id,
                    YapilanTalimatlarStr = "",
                    GerceklesenHafta = 0
                };
                liste.Add(yeniItem);
            }


            _db.StokKartOperasyons.AddRange(liste);
            _db.SaveChanges();
            TempDataCreate(1);
            return RedirectToAction("PotaKartOperasyon", "KartPota", new { id = model.PotaKart.Id });
        }
        #region Operasyon Detay ve Malzeme Kullanımı

        public ActionResult KartOperasyonDetay(int id)
        {
            var model = new KartPotaModel()
            {
                StokKartOperasyon = new StokKartOperasyon(),
                KomponentTalimatGrup = new KomponentTalimatGrup(),
                TalimatTanims = new List<TalimatTanim>()
            };

            var operasyon = _db.StokKartOperasyons.Find(id);
            var potaKart = _db.PotaKarts.Find(operasyon.StokKartId);
            var kompTalimatGrup = _db.KomponentTalimatGrups.Find(operasyon.KomponentTalimatGrupId);
            var operasyonMalzemeler = _db.StokKartOperasyonKullanilanMalzemes.Where(a => a.OperasyonId == id)
                .OrderBy(a => a.KayitTarihi).ToList();

            var stokIdler = operasyonMalzemeler.Select(a => a.KullanilanStokKartId).Distinct().ToList();

            var guncelPeriyot = _db.PeriyotTanims.Find(kompTalimatGrup.PeriyotTanimId);

            model.StokKartOperasyon2 = new StokKartOperasyon()
            {
                PlanlananTarih = operasyon.PlanlananTarih.AddDays(7 * guncelPeriyot.PeriyotDonemi),
                KomponentTalimatGrupId = operasyon.KomponentTalimatGrupId
            };



            var talimatTanimIdler = _db.KomponentTalimats.Where(a => a.KomponentTalimatGrupId == kompTalimatGrup.Id)
                .Select(a => a.TalimatTanimId).ToList();

            var talimatTanims = new List<TalimatTanim>();

            foreach (var i in talimatTanimIdler)
            {
                if (_db.TalimatTanims.Any(a => a.Id == i))
                {
                    talimatTanims.Add(_db.TalimatTanims.First(a => a.Id == i));
                }
            }

            model.User = _db.Users.Find(operasyon.KayitYapanId);

            model.PotaKart = potaKart;
            model.StokKartOperasyon = operasyon;
            model.KomponentTalimatGrup = kompTalimatGrup;
            model.TalimatTanims = talimatTanims;
            model.KomponentTalimatGrups = _db.KomponentTalimatGrups.ToList();
            model.StokKartOperasyonKullanilanMalzemes = operasyonMalzemeler;
            model.StokKarts = _db.StokKarts.Where(a => stokIdler.Any(b => b == a.Id)).ToList();


            var strList = operasyon.YapilanTalimatlarStr.StrArrayeCevir("-").ToList();

            foreach (var itt in model.TalimatTanims)
            {
                itt.SeciliMi = strList.Any(a => a == itt.Id.ToString());
            }
            model.tabId = 5;
            return View(model);
        }


        public PartialViewResult OperasyonaMalzemeEkle(int id)
        {
            var model = new KartPotaModel()
            {
                StokKartOperasyon = new StokKartOperasyon() { Id = id }
            };

            var squery =
                "SELECT TOP (100) PERCENT dbo.Ambar.AmbarAdi, dbo.AmbarStokKart.Id, dbo.AmbarStokKart.ToplamMiktar, dbo.StokKart.StokKodu, dbo.StokKart.StokTanimAdi FROM            dbo.StokKart INNER JOIN dbo.AmbarStokKart ON dbo.StokKart.Id = dbo.AmbarStokKart.StokKartId INNER JOIN dbo.Ambar ON dbo.AmbarStokKart.AmbarId = dbo.Ambar.Id WHERE        (dbo.Ambar.AmbarTipi = 1) AND (dbo.AmbarStokKart.ToplamMiktar > 0) ORDER BY dbo.AmbarStokKart.ToplamMiktar DESC, dbo.Ambar.AmbarAdi, dbo.StokKart.StokTanimAdi";

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

            foreach (var itt in result)
            {
                var lst = itt.Values.ToList();

                string ambarAdi = lst[0].ToString();
                string ambarstokKartId = lst[1].ToString();

                decimal toplamMiktar = (decimal)(lst[2] ?? 0);
                string stokKodu = lst[3].ToString();
                string stokTanimAdi = lst[4].ToString();

                var tanim = string.Format("({0}) {1}-{2} -- {3} Adet", ambarAdi, stokTanimAdi, stokKodu, toplamMiktar);



                model.DropItems.Add(new DropItem() { Id = ambarstokKartId, Tanim = tanim });
            }


            return PartialView("_OperasyonaMalzemeEkle", model);
        }

        [HttpPost]
        public JsonResult OperasyonaMalzemeEkle(KartPotaModel model)
        {
            int state = 0;
            string title = "";
            string icon = "warning";

            var operasyon = _db.StokKartOperasyons.Find(model.StokKartOperasyon.Id);
            var kompTalimatGrup = _db.KomponentTalimatGrups.Find(operasyon.KomponentTalimatGrupId);
            var kullanilanMalzeme = model.StokKartOperasyonKullanilanMalzeme;
            var miktar = kullanilanMalzeme.Miktar;

            if (model.KatId != 0 && miktar > 0)
            {
                var ambarStokKart = _db.AmbarStokKarts.Find(model.KatId);

                if (ambarStokKart.ToplamMiktar >= miktar)
                {
                    string aciklama = string.Format("{0} Malzeme Kullanımı - {1}", kompTalimatGrup.KomponentTalimatGrupAdi, User.Name);

                    var hareketId = BotaAmbarStokHareketOlustur(ambarStokKart.Id, 1, miktar, aciklama, 0, 0);

                    var yeniItem = new StokKartOperasyonKullanilanMalzeme()
                    {
                        Miktar = miktar,
                        KayitTarihi = DateTime.Now,
                        KullanilanStokKartId = ambarStokKart.StokKartId,
                        KullanilanAmbar = ambarStokKart.AmbarId,
                        OperasyonId = operasyon.Id,
                        AmbarStokKartHareketId = hareketId
                    };
                    _db.StokKartOperasyonKullanilanMalzemes.Add(yeniItem);
                    _db.SaveChanges();



                    state = 1;
                    TempDataCreate(2);
                }
                else
                {
                    title = "Miktar Yetersiz !!";
                }

            }
            else
            {
                title = "Bilgileri Kontrol Ediniz!!";
            }

            return new JsonResult { Data = new { Id = operasyon.Id, state = state, icon = icon, title = title }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult OperasyonDuzenle(int id, KartPotaModel model)
        {
            var operasyon = model.StokKartOperasyon;


            int state = 0;
            string title = "";
            string icon = "warning";
            var editOperasyon = _db.StokKartOperasyons.Find(operasyon.Id);
            if (id == 1)
            {
                // bilgiler güncellenecek


                if (editOperasyon.KayitYapanId == User.Id)
                {
                    if (editOperasyon.OperasyonDurumu == 1)
                    {
                        // gerçekleştirilmiş
                        title = "Operasyon Gerçekleştiği için Düzenleme yapılamaz";
                    }
                    else
                    {
                        editOperasyon.OperasyonOncesiAciklama = operasyon.OperasyonOncesiAciklama;
                        editOperasyon.PlanlananTarih = operasyon.PlanlananTarih;
                        editOperasyon.KomponentTalimatGrupId = operasyon.KomponentTalimatGrupId;
                        _db.SaveChanges();
                        state = 1;
                        TempDataCreate(2);
                    }
                }
                else
                {
                    title = "Kayıt Yapan olmadığınız için güncelleme yapamazsınız";

                }
            }
            if (id == 2)
            {
                // operasyon tamamlanacak
                var talimatIdler = model.TalimatTanims.Where(a => a.SeciliMi).ToList().Select(a => a.Id);
                if (editOperasyon.OperasyonDurumu == 1)
                {
                    title = "Operasyon Daha Önce Onaylanmıştır";
                }
                else
                {
                    editOperasyon.GerceklesenTarih = operasyon.GerceklesenTarih;
                    editOperasyon.OperasyonSonrasiAciklama = operasyon.OperasyonSonrasiAciklama;
                    editOperasyon.OperasyonDurumu = 1;
                    var strIdler = "";
                    foreach (var i in talimatIdler)
                    {
                        strIdler += i;
                        if (i != talimatIdler.Last())
                        {
                            strIdler += "-";
                        }
                    }

                    editOperasyon.YapilanTalimatlarStr = strIdler;
                    _db.SaveChanges();
                    state = 1;

                    if (model.KatId == 0)
                    {
                        // yeni periyodik Operasyon

                        var periyodikOperasyon = model.StokKartOperasyon2;

                        var yeniItem = new StokKartOperasyon()
                        {
                            PlanlananTarih = periyodikOperasyon.PlanlananTarih,
                            KomponentTalimatGrupId = periyodikOperasyon.KomponentTalimatGrupId,
                            StokKartId = editOperasyon.StokKartId,
                            OperasyonDurumu = 0,
                            PlanlananHafta = 0,
                            GerceklesenTarih = periyodikOperasyon.PlanlananTarih,
                            GerceklesenHafta = 0,
                            IlkPlanlananTarih = periyodikOperasyon.PlanlananTarih,
                            KayitYapanId = User.Id,
                            YapilanTalimatlarStr = ""

                        };
                        _db.StokKartOperasyons.Add(yeniItem);
                        _db.SaveChanges();
                    }
                    TempDataCreate(2);
                }


            }
            return new JsonResult { Data = new { Id = operasyon.Id, state = state, icon = icon, title = title }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion

        #endregion

        #region Pota Kart Klasor
        public ActionResult PotaKartDosyaListe(int id)
        {
            var potaKart = _db.PotaKarts.Find(id);


            var model = new KartPotaModel()
            {
                PotaKart = potaKart,
                PotaKlasorKlasors = _db.PotaKlasorKlasors.Where(a => a.PotaKartId == id).OrderBy(a => a.KlasorAdi).ToList()
            };

            var klasorIdler = model.PotaKlasorKlasors.Select(a => a.Id).ToList();

            model.Medyas = _db.Medyas.Where(a => klasorIdler.Any(b => b == a.BagliOlduguId && a.BagliOlduguTip == 0))
                .ToList();
            model.tabId = 4;
            return View(model);
        }


        [HttpPost]

        public ActionResult PotaKartDosyaEkleDuzenle(KartPotaModel model)
        {
            var kart = model.PotaKart;
            var dosya = model.PotaKlasorKlasor;

            if (dosya.KlasorAdi == null)
            {
                TempDataCreate(4);

            }
            else
            {
                if (dosya.Id == 0)
                {
                    var yeniItem = new PotaKlasorKlasor()
                    {
                        KlasorAdi = dosya.KlasorAdi,
                        BagliOlduguKlasorId = dosya.BagliOlduguKlasorId,
                        PotaKartId = kart.Id
                    };
                    _db.PotaKlasorKlasors.Add(yeniItem);
                    _db.SaveChanges();
                }
                else
                {
                    var editItem = _db.PotaKlasorKlasors.Find(dosya.Id);
                    editItem.KlasorAdi = dosya.KlasorAdi;
                    _db.SaveChanges();
                }
                TempDataCreate(2);
            }
            return RedirectToAction("PotaKartDosyaListe", "KartPota", new { id = kart.Id });
        }

        public ActionResult PotaKartKlasorSil(int id)
        {
            var item = _db.PotaKlasorKlasors.Find(id);
            var idd = item.PotaKartId;
            _db.PotaKlasorKlasors.Remove(item);
            _db.SaveChanges();

            TempDataCreate(3);
            return RedirectToAction("PotaKartDosyaListe", "KartPota", new { id = idd });
        }


        #endregion
        #region Pota kart & Medya

        [HttpPost]
        public ActionResult PotaKartMedyaEkle(HttpPostedFileBase file, KartPotaModel model)
        {
            int state = 0;//0-bilgi eksik, 1-işlem okey, 2-redirect


            int tabId = 4;

            var stokKart = model.PotaKart;
            var klasor = model.PotaKlasorKlasor;

            bool sorunVarmi = false;
            if (file != null)
            {
                if (file.ContentLength < 1)
                {
                    sorunVarmi = true;
                }
            }
            else
            {
                sorunVarmi = true;
            }

            if (sorunVarmi == false)
            {
                sorunVarmi = model.Medya.MedyaAdi == null;
            }
            if (sorunVarmi)
            {

                TempDataCustom(1, "Resim Yükleyiniz");
            }
            else
            {
                state = 1;
                Guid guid = Guid.NewGuid();
                var path = "~/Upload/StokMedya/" + guid.ToString() + file.FileName;

                file.SaveAs(Server.MapPath(path));

                var medya = new Medya()
                {
                    BagliOlduguId = klasor.Id,
                    FileName = path,
                    BagliOlduguTip = 0,
                    MedyaAdi = model.Medya.MedyaAdi
                };
                _db.Medyas.Add(medya);
                _db.SaveChanges();
                TempDataCreate(2);
            }

            return RedirectToAction("PotaKartDosyaListe", "KartPota", new { id = stokKart.Id });
        }

        public ActionResult PotaKartMedyaSil(int id)
        {
            var item = _db.Medyas.Find(id);
            var idd = item.BagliOlduguId;

            _db.Medyas.Remove(item);
            _db.SaveChanges();
            TempDataCreate(3);

            var kartId = _db.PotaKlasorKlasors.Find(idd).PotaKartId;

            return RedirectToAction("PotaKartDosyaListe", "KartPota", new { id = kartId });
        }


        #endregion


        #region Pota Kart Duruş

        public ActionResult PotaKartDurus(int id = 0)
        {
            TempData["theme"] = "gizle";
            var potaKart = _db.PotaKarts.Find(id);


            var model = new KartPotaModel()
            {
                PotaKart = potaKart,
                PotaKartAgacs = _db.PotaKartAgacs.Where(a => a.PotaKartId == id).ToList()
            };
            model = TakvimeGoreDurusModelGetir(model);



            model.tabId = 6;
            return View(model);
        }

        [HttpPost]
        public PartialViewResult PotaKartDurusListesi(KartPotaModel model)
        {
            return PartialView("_PotaKartDurusListesi", TakvimeGoreDurusModelGetir(model));
        }
        public PartialViewResult DurusEkleDuzenle(int id, int ItemId)
        {
            var model = new KartPotaModel
            {
                KatId = ItemId,
                StokKartDuru = new StokKartDuru() { StokKartId = id, DurusBaslangic = DateTime.Now.Date.AddDays(-1), DurusBitis = DateTime.Now.Date },
                DurusGrubuTanims = _db.DurusGrubuTanims.OrderBy(a => a.DurusGrubuTanimAdi).ToList(),
                DurusTipiTanims = _db.DurusTipiTanims.OrderBy(a => a.DurusTipi).ToList(),
                PotaKart = _db.PotaKarts.Find(id)
            };

            if (ItemId != 0)
            {
                model.StokKartDuru = _db.StokKartDurus.Find(ItemId);
            }

            return PartialView("_DurusEkleDuzenle", model);
        }
        [HttpPost]
        public JsonResult DurusEkleDuzenle(KartPotaModel model)
        {
            var durus = model.StokKartDuru;
            var stokKart = model.PotaKart;
            int state = 0;

            TimeSpan span = durus.DurusBitis - durus.DurusBaslangic;

            if (durus.DurusTipi != 0)
            {

                state = 1;

                var totalSure = Convert.ToDecimal(Math.Abs(span.TotalHours));

                var durusTanim = _db.DurusTipiTanims.Find(durus.DurusTipi);

                if (durus.Id == 0)
                {
                    var yeniItem = new StokKartDuru()
                    {
                        StokKartId = stokKart.Id,
                        DurusTipi = durus.DurusTipi,
                        DurusBaslangic = durus.DurusBaslangic,
                        DurusBitis = durus.DurusBitis,
                        DurusGrubuId = durusTanim.DurusGrubu,
                        ToplamSureDk = totalSure
                    };
                    _db.StokKartDurus.Add(yeniItem);
                    _db.SaveChanges();
                }
                else
                {
                    var editItem = _db.StokKartDurus.Find(durus.Id);
                    editItem.DurusTipi = durus.DurusTipi;
                    editItem.DurusBaslangic = durus.DurusBaslangic;
                    editItem.DurusBitis = durus.DurusBitis;
                    editItem.DurusGrubuId = durusTanim.DurusGrubu;
                    editItem.ToplamSureDk = totalSure;
                    _db.SaveChanges();
                }

                if (model.SecimId == 0)
                {
                    // öteleme yapılabilen komponent grup Idler
                    var otelenebilirKompIdler = new List<int>();
                    #region öteleme yapılabilen komponent grup Idler
                    var squery =
                        "SELECT  dbo.KomponentTalimatGrup.Id FROM dbo.PeriyotTanim INNER JOIN dbo.PeriyotTipiTanim ON dbo.PeriyotTanim.PeriyotTipi = dbo.PeriyotTipiTanim.Id INNER JOIN dbo.KomponentTalimatGrup ON dbo.PeriyotTanim.Id = dbo.KomponentTalimatGrup.PeriyotTanimId WHERE        (dbo.PeriyotTipiTanim.OteleYapilabilmDurumu = 1)";


                    var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();


                    foreach (var aa in result)
                    {
                        int kompGrupId = (int)aa.ToList()[0].Value;
                        otelenebilirKompIdler.Add(kompGrupId);
                    }
                    #endregion







                    var lst = _db.StokKartOperasyons
                        .Where(a => a.OperasyonDurumu == 0 && a.StokKartId == stokKart.Id).ToList();

                    var kompIdler = lst.Select(a => a.KomponentTalimatGrupId).Distinct().ToList();

                    var komponentGruplar = _db.KomponentTalimatGrups.Where(a => kompIdler.Any(b => b == a.Id)).ToList();


                    foreach (var stokKartOperasyon in lst)
                    {

                        if (otelenebilirKompIdler.Any(b => b == stokKartOperasyon.KomponentTalimatGrupId))
                        {
                            stokKartOperasyon.PlanlananTarih = stokKartOperasyon.PlanlananTarih.AddHours(Convert.ToDouble(totalSure));
                            _db.SaveChanges();
                        }

                    }
                }
            }
            return new JsonResult { Data = new { Id = stokKart.Id, state = state }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        /*
        public PartialViewResult DurusEkleDuzenle(int id, int ItemId)
        {
            var model = new DurusStokKartModel
            {
                Id = ItemId,
                StokKartDuru = new StokKartDuru() { StokKartId = id, DurusBaslangic = DateTime.Now.Date.AddDays(-1), DurusBitis = DateTime.Now.Date },
                DurusGrubuTanims = _db.DurusGrubuTanims.OrderBy(a => a.DurusGrubuTanimAdi).ToList(),
                DurusTipiTanims = _db.DurusTipiTanims.OrderBy(a => a.DurusTipi).ToList(),
                StokKart = _db.StokKarts.Find(id)
            };

            if (ItemId != 0)
            {
                model.StokKartDuru = _db.StokKartDurus.Find(ItemId);
            }

            return PartialView("_DurusEkleDuzenle", model);
        }
        [HttpPost]
        public JsonResult DurusEkleDuzenle(DurusStokKartModel model)
        {
            var durus = model.StokKartDuru;
            var stokKart = model.StokKart;
            int state = 0;

            TimeSpan span = durus.DurusBitis - durus.DurusBaslangic;

            if (durus.DurusTipi != 0)
            {

                state = 1;

                var totalSure = Convert.ToDecimal(Math.Abs(span.TotalHours));

                var durusTanim = _db.DurusTipiTanims.Find(durus.DurusTipi);

                if (durus.Id == 0)
                {
                    var yeniItem = new StokKartDuru()
                    {
                        StokKartId = stokKart.Id,
                        DurusTipi = durus.DurusTipi,
                        DurusBaslangic = durus.DurusBaslangic,
                        DurusBitis = durus.DurusBitis,
                        DurusGrubuId = durusTanim.DurusGrubu,
                        ToplamSureDk = totalSure
                    };
                    _db.StokKartDurus.Add(yeniItem);
                    _db.SaveChanges();
                }
                else
                {
                    var editItem = _db.StokKartDurus.Find(durus.Id);
                    editItem.DurusTipi = durus.DurusTipi;
                    editItem.DurusBaslangic = durus.DurusBaslangic;
                    editItem.DurusBitis = durus.DurusBitis;
                    editItem.DurusGrubuId = durusTanim.DurusGrubu;
                    editItem.ToplamSureDk = totalSure;
                    _db.SaveChanges();
                }

                if (model.SecimId == 0)
                {
                    var lst = _db.StokKartOperasyons
                        .Where(a => a.OperasyonDurumu == 0 && a.StokKartId == stokKart.Id).ToList();

                    foreach (var stokKartOperasyon in lst)
                    {
                        stokKartOperasyon.PlanlananTarih = stokKartOperasyon.PlanlananTarih.AddHours(Convert.ToDouble(totalSure));
                        _db.SaveChanges();
                    }
                }
            }
            return new JsonResult { Data = new { Id = stokKart.Id, state = state }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        */
        #endregion




        #region Pasife Alınan Kodlar

        /*
              public ActionResult PotaKartStokGrupListe()
        {
            var KomponentTanimGrups = _db.KomponentTanimGrups.OrderBy(a => a.KomponentTanimGrupAdi).ToList();
            var KomponentTanims = _db.KomponentTanims.OrderBy(a => a.KomponentTanimAdi).ToList();



            var model = new KartPotaModel()
            {

                KomponentTanimGrups = KomponentTanimGrups,
                KomponentTanims = KomponentTanims
            };
            return View(model);
        }
         */

        #endregion

    }
}


/*
    public ActionResult OperasyonEkleDuzenle(int id, int operasyonId)
        {
            var model = new KartPotaModel()
            {
                StokKartOperasyon = new StokKartOperasyon()
            };
            model.PotaKart = _db.PotaKarts.Find(id);

            if (operasyonId != 0)
            {
                var operasyon = _db.StokKartOperasyons.Find(id);
                var stokKart = _db.StokKarts.Find(operasyon.StokKartId);
                var kompTalimatGrup = _db.KomponentTalimatGrups.Find(operasyon.KomponentTalimatGrupId);
                var operasyonMalzemeler = _db.StokKartOperasyonKullanilanMalzemes.Where(a => a.OperasyonId == id)
                    .OrderBy(a => a.KayitTarihi).ToList();
                var stokIdler = operasyonMalzemeler.Select(a => a.KullanilanStokKartId).Distinct().ToList();
                var guncelPeriyot = _db.PeriyotTanims.Find(kompTalimatGrup.PeriyotTanimId);
                model.StokKartOperasyon2 = new StokKartOperasyon()
                {
                    PlanlananTarih = operasyon.PlanlananTarih.AddDays(7 * guncelPeriyot.PeriyotDonemi),
                    KomponentTalimatGrupId = operasyon.KomponentTalimatGrupId
                };

                var talimatTanimIdler = _db.KomponentTalimats.Where(a => a.KomponentTalimatGrupId == kompTalimatGrup.Id)
                    .Select(a => a.TalimatTanimId).ToList();
                var talimatTanims = new List<TalimatTanim>();
                foreach (var i in talimatTanimIdler)
                {
                    if (_db.TalimatTanims.Any(a => a.Id == i))
                    {
                        talimatTanims.Add(_db.TalimatTanims.First(a => a.Id == i));
                    }
                }
                model.User = _db.Users.Find(operasyon.KayitYapanId);
                model.StokKartOperasyon = operasyon;
                model.KomponentTalimatGrup = kompTalimatGrup;
                model.TalimatTanims = talimatTanims;
                model.KomponentTalimatGrups = _db.KomponentTalimatGrups.ToList();
                model.StokKartOperasyonKullanilanMalzemes = operasyonMalzemeler;
                model.StokKarts = _db.StokKarts.Where(a => stokIdler.Any(b => b == a.Id)).ToList();
                var strList = operasyon.YapilanTalimatlarStr.StrArrayeCevir("-").ToList();

                foreach (var itt in model.TalimatTanims)
                {
                    itt.SeciliMi = strList.Any(a => a == itt.Id.ToString());
                }
            }


            return View(model);
        }
*/