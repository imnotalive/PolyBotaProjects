using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PolyBota.BLL;
using PolyBota.DAL;
using PolyBota.Entities;
using PolyBota.Helpers;
using PolyBota.Web.Areas.Endustri.Models;
using PolyBota.Web.Controllers;

namespace PolyBota.Web.Areas.Endustri.Controllers
{
    public class ZamanEtuduController : BaseController
    {
        // GET: Endustri/ZamanEtudu

        public List<PotaKart> PotaKartlar()
        {
            var squery =
                "SELECT    Id, PotaKartAdi FROM            dbo.PotaKart ORDER BY PotaKartAdi";
            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();
            var potakarts = new List<PotaKart>();
            potakarts.Add(new PotaKart() { Id = 0, PotaKartAdi = "Seçiniz" });
            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();
                potakarts.Add(new PotaKart()
                {
                    Id = (int)lst[0],
                    PotaKartAdi = lst[1]?.ToString()

                });

            }

            return potakarts;
        }

        public List<DropItem> DropPotaKartBilesens(int id)
        {
            var squery = string.Format(
                "SELECT     dbo.PotaKartBilesenItems.BilesenItemId, dbo.BilesenHeader.BilesenHeaderAdi, dbo.BilesenItem.BilesenDegeri FROM            dbo.PotaKartBilesenItems INNER JOIN                         dbo.BilesenItem ON dbo.PotaKartBilesenItems.BilesenItemId = dbo.BilesenItem.Id INNER JOIN                         dbo.BilesenHeader ON dbo.BilesenItem.BilesenHeaderId = dbo.BilesenHeader.Id WHERE        (dbo.PotaKartBilesenItems.PotaKartId = {0}) ORDER BY dbo.BilesenHeader.BilesenHeaderAdi, dbo.BilesenItem.BilesenDegeri", id);

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();
            var drops = new List<DropItem>();
            drops.Add(new DropItem() { Id = "0", Tanim = "Seçiniz" });

            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();
                drops.Add(new DropItem()
                {
                    Id = lst[0]?.ToString(),
                    Tanim = string.Format("{0} - {1}", lst[1], lst[2])
                });

            }

            return drops;
        }

        public ActionResult ZeItemDetailListe(int CurrentPage = 1, int PageShowCount = 50, int katId = 1)
        {
            var model = new ZamanEtuduModel();

            var squery =
                "SELECT        TOP (100) PERCENT dbo.ZamanEtuduItemDetail.Id, dbo.ZamanEtuduItemDetail.Tarih, dbo.PotaKart.PotaKartAdi, dbo.BilesenItem.BilesenDegeri, dbo.BilesenHeader.BilesenHeaderAdi, dbo.ZamanEtuduItemDetail.OlcumDegeri, dbo.ZamanEtuduItemDetail.Aciklama, dbo.[User].Name, User_1.Name AS Expr1, dbo.ZamanEtuduItemDetail.BilesenItemId, dbo.ZamanEtuduKategori.KategoriAdi FROM            dbo.[User] AS User_1 INNER JOIN dbo.ZamanEtuduItemDetail INNER JOIN dbo.PotaKart ON dbo.ZamanEtuduItemDetail.PotaKartId = dbo.PotaKart.Id INNER JOIN dbo.[User] ON dbo.ZamanEtuduItemDetail.UserId = dbo.[User].Id INNER JOIN dbo.ZamanEtuduKategori ON dbo.ZamanEtuduItemDetail.ZamanEtuduKategoriId = dbo.ZamanEtuduKategori.Id ON User_1.Id = dbo.ZamanEtuduItemDetail.KayitYapanId LEFT OUTER JOIN dbo.BilesenHeader INNER JOIN dbo.BilesenItem ON dbo.BilesenHeader.Id = dbo.BilesenItem.BilesenHeaderId ON dbo.ZamanEtuduItemDetail.BilesenItemId = dbo.BilesenItem.Id ORDER BY dbo.ZamanEtuduItemDetail.Tarih DESC, dbo.PotaKart.PotaKartAdi";

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

            var drops = new List<DropItem>();
            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();

                var Id = (int)lst[0];
                var Tarih = (DateTime)lst[1];
                var PotaKartAdi = lst[2]?.ToString();
                var BilesenDegeri = lst[3]?.ToString();
                var BilesenHeaderAdi = lst[4]?.ToString();
                var OlcumDegeri = lst[5]?.ToString();
                var Aciklama = lst[6]?.ToString();
                var Name = lst[7]?.ToString();
                var kayitYapan = lst[8]?.ToString();
                var BilesenItemId = (int)lst[9];
                var ZeKategoriAdi = lst[10]?.ToString();
                drops.Add(new DropItem()
                {
                    Id = Id.ToString(),
                    IdInt = BilesenItemId,

                    DateTime = Tarih,
                    Tanim1 = Tarih.ToShortDateString(),
                    Tanim2 = PotaKartAdi,
                    Tanim3 = BilesenHeaderAdi +" - "+ BilesenDegeri,
                    Tanim4 = OlcumDegeri,
                    Tanim5 = Aciklama,
                    Tanim6 = Name,
                    Tanim7=kayitYapan,
                    Tanim = ZeKategoriAdi
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
        public PartialViewResult ZeItemDetailEkleDuzenle(int id)
        {
            var model = new ZamanEtuduModel();
            model.PotaKarts = PotaKartlar();
            model.Users = _db.Users.OrderBy(a => a.Name).ToList();
            model.ZamanEtuduKategoris = _db.ZamanEtuduKategoris.OrderBy(a => a.KategoriAdi).ToList();





            if (id != 0)
            {
                model.ZamanEtuduItemDetail = _db.ZamanEtuduItemDetails.Find(id);
                model.ZamanEtuduModelItem.DropItems = DropPotaKartBilesens(model.ZamanEtuduItemDetail.PotaKartId);
            }

         
            return PartialView("_ZeItemDetailEkleDuzenle", model);
        }


        public ActionResult ZeItemDetailSil(int id)
        {
            var item = _db.ZamanEtuduItemDetails.Find(id);
            _db.ZamanEtuduItemDetails.Remove(item);
            _db.SaveChanges();

            TempDataCreate(3);
            return RedirectToAction("ZeItemDetailListe");
        }
        public PartialViewResult ZeItemDetailBilesenGetir(int id, int pkId)
        {
            var model = new ZamanEtuduModel();
            model.PotaKarts = PotaKartlar();


            if (id != 0)
            {
                model.ZamanEtuduModelItem.ZamanEtuduItem = _db.ZamanEtuduItems.Find(id);
            }
            if (pkId != 0)
            {
                model.ZamanEtuduModelItem.DropItems = DropPotaKartBilesens(pkId);

            }
            return PartialView("_ZeItemDetailBilesenGetir", model);
        }

        [HttpPost]
        public ActionResult ZeItemDetailEkleDuzenle(ZamanEtuduModel model)
        {
            var item = model.ZamanEtuduItemDetail;
            item.KayitYapanId = User.Id;

            if (item.Id==0)
            {
                var yeniItem = new ZamanEtuduItemDetail()
                {
                    Aciklama = item.Aciklama,
                    BilesenItemId = item.BilesenItemId,
                    KayitYapanId = User.Id,
                    OlcumDegeri = item.OlcumDegeri,
                    PotaKartId = item.PotaKartId,
                    Tarih = item.Tarih,
                    UserId = item.UserId,
                    ZamanEtuduKategoriId = item.ZamanEtuduKategoriId
                };
                _db.ZamanEtuduItemDetails.Add(yeniItem);
                _db.SaveChanges();

            }
            else
            {
                var editItem = _db.ZamanEtuduItemDetails.Find(item.Id);
                editItem.Aciklama = item.Aciklama;
                editItem.BilesenItemId = item.BilesenItemId;
                editItem.UserId = item.UserId;
                editItem.OlcumDegeri = item.OlcumDegeri;
                editItem.PotaKartId = item.PotaKartId;
                editItem.Tarih = item.Tarih;
                editItem.ZamanEtuduKategoriId = item.ZamanEtuduKategoriId;
                _db.SaveChanges();
            }

            TempDataCreate(2);

            return RedirectToAction("ZeItemDetailListe");

        }

        #region Toplu Çalışma

        public ActionResult CalismaListe(int CurrentPage = 1, int PageShowCount = 50, int katId = 1)
        {
            var model = new ZamanEtuduModel();

            var squery =
                "SELECT        TOP (100) PERCENT dbo.ZamanEtuduHeader.Id, dbo.ZamanEtuduHeader.Tarih, dbo.ZamanEtuduHeader.EtutNo, dbo.ZamanEtuduHeader.KayitYapan, dbo.ZamanEtuduHeader.GozlemSayisi, dbo.[User].Name FROM            dbo.ZamanEtuduHeader INNER JOIN                         dbo.[User] ON dbo.ZamanEtuduHeader.KayitYapan = dbo.[User].Id ORDER BY dbo.ZamanEtuduHeader.Tarih DESC";

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

            var drops = new List<DropItem>();
            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();

                var Id = (int)lst[0];
                var Tarih = (DateTime)lst[1];
                var EtutNo = lst[2]?.ToString();
                var KayitYapan = (int)lst[3];
                var GozlemSayisi = (int)lst[4];
                var Name = lst[5]?.ToString();

                drops.Add(new DropItem()
                {
                    Id = Id.ToString(),
                    DateTime = Tarih,
                    Tanim1 = Tarih.ToShortDateString(),
                    Tanim2 = EtutNo,
                    Tanim3 = GozlemSayisi.ToString(),
                    Tanim4 = Name,
                    Tanim5 = Name
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

        #region Çalışma Header
        public ActionResult CalismaEkleDuzenle(int id = 0)
        {
            var model = new ZamanEtuduModel()
            {
                Users = _db.Users.OrderBy(a => a.Name).ToList()
            };
            model.PotaKarts = PotaKartlar();

            if (id != 0)
            {
                model.ZamanEtuduHeader = _db.ZamanEtuduHeaders.Find(id);

                var gozlemSayisi = model.ZamanEtuduHeader.GozlemSayisi;

                for (int i = 0; i < gozlemSayisi; i++)
                {
                    model.DropItem.ItemValues.Add(new ItemValue()
                    {
                        IdInt = (i + 1)
                    });
                }


                var squery = string.Format(
                    "SELECT        TOP (100) PERCENT dbo.ZamanEtuduItem.Id, dbo.ZamanEtuduItem.BilesenItemId, dbo.PotaKart.PotaKartAdi, dbo.BilesenHeader.BilesenHeaderAdi, dbo.BilesenItem.BilesenDegeri, dbo.ZamanEtuduItem.Aciklama FROM            dbo.ZamanEtuduItem INNER JOIN                         dbo.PotaKart ON dbo.ZamanEtuduItem.PotaKartId = dbo.PotaKart.Id LEFT OUTER JOIN                         dbo.BilesenHeader INNER JOIN                         dbo.BilesenItem ON dbo.BilesenHeader.Id = dbo.BilesenItem.BilesenHeaderId ON dbo.ZamanEtuduItem.BilesenItemId = dbo.BilesenItem.Id WHERE        (dbo.ZamanEtuduItem.ZamanEtuduHeaderId = {0}) ORDER BY dbo.PotaKart.PotaKartAdi, dbo.BilesenHeader.BilesenHeaderAdi, dbo.BilesenItem.BilesenDegeri",
                    id);

                var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

                var modelItems = new List<ZamanEtuduModelItem>();

                foreach (var aa in result)
                {
                    var lst = aa.Values.ToList();

                    var zeItemId = (int)lst[0];
                    var BilesenItemId = (int)lst[1];
                    var PotaKartAdi = lst[2]?.ToString();
                    var BilesenHeaderAdi = lst[3]?.ToString();
                    var BilesenDegeri = lst[4]?.ToString();
                    var Aciklama = lst[5]?.ToString();

                    modelItems.Add(new ZamanEtuduModelItem()
                    {
                        DropItem = new DropItem() { Tanim1 = PotaKartAdi, Tanim2 = string.Format("{0} - {1}", BilesenHeaderAdi, BilesenDegeri), IdInt = BilesenItemId },
                        ZamanEtuduItem = new ZamanEtuduItem() { BilesenItemId = BilesenItemId, Id = zeItemId, ZamanEtuduHeaderId = id, Aciklama = Aciklama }
                    });
                }

                squery = string.Format(
                    "SELECT        TOP (100) PERCENT dbo.ZamanEtuduItemDeger.Id, dbo.ZamanEtuduItemDeger.ZamanEtuduItemId, dbo.ZamanEtuduItemDeger.Sira, dbo.ZamanEtuduItemDeger.ItemDegeri FROM            dbo.ZamanEtuduItem INNER JOIN                         dbo.ZamanEtuduItemDeger ON dbo.ZamanEtuduItem.Id = dbo.ZamanEtuduItemDeger.ZamanEtuduItemId WHERE        (dbo.ZamanEtuduItem.ZamanEtuduHeaderId = {0})",
                    id);
                result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();
                var zamanEtuduItemDegers = new List<ZamanEtuduItemDeger>();

                foreach (var aa in result)
                {
                    var lst = aa.Values.ToList();

                    var Id = (int)lst[0];
                    var ZamanEtuduItemId = (int)lst[1];
                    var Sira = (int)lst[2];
                    var ItemDegeri = (int)lst[3];
                    zamanEtuduItemDegers.Add(new ZamanEtuduItemDeger()
                    {
                        Id = Id,
                        ItemDegeri = ItemDegeri,
                        Sira = Sira,
                        ZamanEtuduItemId = ZamanEtuduItemId
                    });
                }


                foreach (var modelItem in modelItems)
                {

                    var itemDegers = zamanEtuduItemDegers.Where(a => a.ZamanEtuduItemId == modelItem.ZamanEtuduItem.Id)
                        .OrderBy(a => a.Sira).ToList();

                    foreach (var itemValue in model.DropItem.ItemValues)
                    {
                        var sira = itemValue.IdInt;

                        var zamanItemDeger = new ZamanEtuduItemDeger()
                        {
                            ZamanEtuduItemId = modelItem.ZamanEtuduItem.Id,
                            Sira = sira,
                            Id = 0,
                            ItemDegeri = 0
                        };

                        if (itemDegers.Any(a => a.Sira == sira))
                        {
                            zamanItemDeger = itemDegers.First(a => a.Sira == sira);
                        }

                        modelItem.ZamanEtuduItemDegers.Add(zamanItemDeger);
                    }

                    model.ZamanEtuduModelItems.Add(modelItem);
                }
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult CalismaHeaderEkleDuzenle(ZamanEtuduModel model)
        {
            var header = model.ZamanEtuduHeader;

            if (header.Id == 0)
            {
                header.KayitYapan = User.Id;
                _db.ZamanEtuduHeaders.Add(header);
                _db.SaveChanges();
            }
            else
            {

                var editHeader = _db.ZamanEtuduHeaders.Find(header.Id);
                var gozlemSayisiDegistiMi = !(editHeader.GozlemSayisi == header.GozlemSayisi);
                editHeader.EtutNo = header.EtutNo;
                editHeader.Tarih = header.Tarih;
                editHeader.GozlemSayisi = header.GozlemSayisi;
                _db.SaveChanges();
                if (gozlemSayisiDegistiMi)
                {

                }
            }
            TempDataCreate(2);
            return RedirectToAction("CalismaEkleDuzenle", "ZamanEtudu", new { id = header.Id });
        }

        public ActionResult CalismaHeaderSil(int id)
        {

            var header = _db.ZamanEtuduHeaders.Find(id);

            var zeItems = _db.ZamanEtuduItems.Where(a => a.ZamanEtuduHeaderId == id).ToList();

            var degerler = new List<ZamanEtuduItemDeger>();

            foreach (var zamanEtuduItem in zeItems)
            {
                degerler.AddRange(_db.ZamanEtuduItemDegers.Where(a => a.ZamanEtuduItemId == zamanEtuduItem.Id).ToList());
            }

            _db.ZamanEtuduItemDegers.RemoveRange(degerler);
            _db.SaveChanges();
            _db.ZamanEtuduItems.RemoveRange(zeItems);
            _db.SaveChanges();
            _db.ZamanEtuduHeaders.Remove(header);
            _db.SaveChanges();
            TempDataCreate(3);
            return RedirectToAction("CalismaListe");
        }

        #endregion




        #region ze İtem
        public PartialViewResult ZeItemEkleDuzenle(int id, int itemId)
        {
            var model = new ZamanEtuduModel();
            model.PotaKarts = PotaKartlar();
            model.ZamanEtuduModelItem.ZamanEtuduItem = new ZamanEtuduItem() { ZamanEtuduHeaderId = id };



            if (itemId != 0)
            {
                model.ZamanEtuduModelItem.ZamanEtuduItem = _db.ZamanEtuduItems.Find(itemId);
            }

            model.ZamanEtuduModelItem.DropItems = DropPotaKartBilesens(model.ZamanEtuduModelItem.ZamanEtuduItem.PotaKartId);
            return PartialView("_ZeItemEkleDuzenle", model);
        }
        public PartialViewResult ZeItemBilesenGetir(int id, int pkId)
        {
            var model = new ZamanEtuduModel();
            model.PotaKarts = PotaKartlar();


            if (id != 0)
            {
                model.ZamanEtuduModelItem.ZamanEtuduItem = _db.ZamanEtuduItems.Find(id);
            }
            if (pkId != 0)
            {
                model.ZamanEtuduModelItem.DropItems = DropPotaKartBilesens(pkId);

            }
            return PartialView("_ZeItemBilesenGetir", model);
        }

        [HttpPost]
        public ActionResult ZamanEtuduItemEkleDuzenle(ZamanEtuduModel model)
        {
            var item = model.ZamanEtuduModelItem.ZamanEtuduItem;

            if (item.Id == 0)
            {
                if (item.PotaKartId != 0)
                {
                    _db.ZamanEtuduItems.Add(item);
                    _db.SaveChanges();
                }
            }
            else
            {
                var editItem = _db.ZamanEtuduItems.Find(item.Id);
                editItem.PotaKartId = item.PotaKartId;
                editItem.BilesenItemId = item.BilesenItemId;
                _db.SaveChanges();
            }
            TempDataCreate(2);
            return RedirectToAction("CalismaEkleDuzenle", "ZamanEtudu", new { id = item.ZamanEtuduHeaderId });
        }


        public ActionResult ZamanEtuduItemSil(int id)
        {
            var editItem = _db.ZamanEtuduItems.Find(id);
            var headerId = editItem.ZamanEtuduHeaderId;

            var degerler = _db.ZamanEtuduItemDegers.Where(a => a.ZamanEtuduItemId == id).ToList();

            _db.ZamanEtuduItems.Remove(editItem);
            _db.SaveChanges();
            _db.ZamanEtuduItemDegers.RemoveRange(degerler);
            _db.SaveChanges();
            TempDataCreate(3);
            return RedirectToAction("CalismaEkleDuzenle", "ZamanEtudu", new { id = headerId });
        }

        #endregion




        [HttpPost]
        public ActionResult ZeTopluGuncelle(ZamanEtuduModel model)
        {
            var header = _db.ZamanEtuduHeaders.Find(model.ZamanEtuduHeader.Id);


            var modelItems = model.ZamanEtuduModelItems;


            var guncellenecekZeItems = new List<ZamanEtuduItem>();
            var silinecekDegerler = new List<ZamanEtuduItemDeger>();
            var eklenecekDegerler = new List<ZamanEtuduItemDeger>();
            var guncellenecekDegerler = new List<ZamanEtuduItemDeger>();


            foreach (var zamanEtuduModelItem in modelItems)
            {
                guncellenecekZeItems.Add(zamanEtuduModelItem.ZamanEtuduItem);
                foreach (var deger in zamanEtuduModelItem.ZamanEtuduItemDegers)
                {
                    if (deger.Id == 0)
                    {
                        if (deger.ItemDegeri > 0)
                        {
                            deger.ZamanEtuduItemId = zamanEtuduModelItem.ZamanEtuduItem.Id;
                            eklenecekDegerler.Add(deger);
                        }
                    }
                    else
                    {
                        if (deger.ItemDegeri == 0)
                        {
                            silinecekDegerler.Add(deger);
                        }
                        else
                        {
                            guncellenecekDegerler.Add(deger);
                        }
                    }
                }
            }


            foreach (var zeItem in guncellenecekZeItems)
            {
                var editItem = _db.ZamanEtuduItems.Find(zeItem.Id);
                editItem.Aciklama = zeItem.Aciklama;
                _db.SaveChanges();
            }

            if (eklenecekDegerler.Any())
            {
                _db.ZamanEtuduItemDegers.AddRange(eklenecekDegerler);
                _db.SaveChanges();
            }

            if (silinecekDegerler.Any())
            {
                _db.ZamanEtuduItemDegers.RemoveRange(silinecekDegerler);
                _db.SaveChanges();
            }

            if (guncellenecekDegerler.Any())
            {
                foreach (var item in guncellenecekDegerler)
                {
                    var editItem = _db.ZamanEtuduItemDegers.Find(item.Id);
                    editItem.ItemDegeri = item.ItemDegeri;
                    _db.SaveChanges();
                }
            }

            HeaderItemGuncellemeYap(header);

            TempDataCreate(2);
            return RedirectToAction("CalismaEkleDuzenle", "ZamanEtudu", new { id = header.Id });
        }

        private void HeaderItemGuncellemeYap(ZamanEtuduHeader header)
        {
            var silinecekDegerler = new List<ZamanEtuduItemDeger>();




            var zeItems = _db.ZamanEtuduItems.Where(a => a.ZamanEtuduHeaderId == header.Id).ToList();

            foreach (var item in zeItems)
            {
                silinecekDegerler.AddRange(_db.ZamanEtuduItemDegers.Where(a => a.ZamanEtuduItemId == item.Id && a.Sira > header.GozlemSayisi).ToList());
            }

            if (silinecekDegerler.Any())
            {
                _db.ZamanEtuduItemDegers.RemoveRange(silinecekDegerler);
                _db.SaveChanges();
            }

            foreach (var item in zeItems)
            {
                var items = _db.ZamanEtuduItemDegers.Where(a => a.ZamanEtuduItemId == item.Id).ToList();

                decimal itemSay = Convert.ToDecimal(items.Count);
                decimal degerSay = 0;
                int i = 0;

                foreach (var deger in items)
                {
                    i++;
                    degerSay += deger.ItemDegeri;
                    var editItem = _db.ZamanEtuduItemDegers.Find(deger.Id);
                    editItem.Sira = i;
                    _db.SaveChanges();
                }

                itemSay = itemSay == 0 ? 1 : itemSay;

                var editZeItem = _db.ZamanEtuduItems.Find(item.Id);
                editZeItem.Ortalama = degerSay / itemSay;
                _db.SaveChanges();
            }

        }

        #endregion

    }
}