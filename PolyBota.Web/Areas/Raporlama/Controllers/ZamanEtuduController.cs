using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Wordprocessing;
using PolyBota.BLL;
using PolyBota.DAL;
using PolyBota.Entities;
using PolyBota.Helpers;
using PolyBota.Web.Areas.Raporlama.Models;
using PolyBota.Web.Controllers;

namespace PolyBota.Web.Areas.Raporlama.Controllers
{
    public class ZamanEtuduController : BaseController
    {
        // GET: Raporlama/ZamanEtudu
        public ActionResult AnalizHazirla()
        {
            var model = new RaporlamaZeModel();

            #region Drop Usres

            var squery =
                "SELECT DISTINCT  dbo.ZamanEtuduItemDetail.UserId, dbo.[User].Name FROM dbo.ZamanEtuduItemDetail INNER JOIN dbo.[User] ON dbo.ZamanEtuduItemDetail.UserId = dbo.[User].Id ORDER BY dbo.[User].Name";

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();

                var UserId = (int)lst[0];
                var Name = lst[1]?.ToString();
                model.DropUsers.Add(new DropItem()
                {
                    IdInt = UserId,
                    Tanim = Name,
                    SeciliMi = true
                });

            }
            #endregion
            var RaporlamaPkVeBilesenItems = new List<RaporlamaPkVeBilesenItem>();
            #region RaporlamaPkVeBilesenItems
            squery =
                "SELECT DISTINCT                          TOP (100) PERCENT dbo.ZamanEtuduItemDetail.PotaKartId, dbo.ZamanEtuduItemDetail.BilesenItemId, dbo.BilesenItem.BilesenHeaderId, dbo.PotaKart.PotaKartAdi, dbo.BilesenHeader.BilesenHeaderAdi,                          dbo.BilesenItem.BilesenDegeri FROM            dbo.BilesenHeader RIGHT OUTER JOIN                         dbo.BilesenItem ON dbo.BilesenHeader.Id = dbo.BilesenItem.BilesenHeaderId RIGHT OUTER JOIN                         dbo.ZamanEtuduItemDetail INNER JOIN                         dbo.PotaKart ON dbo.ZamanEtuduItemDetail.PotaKartId = dbo.PotaKart.Id ON dbo.BilesenItem.Id = dbo.ZamanEtuduItemDetail.BilesenItemId ORDER BY dbo.PotaKart.PotaKartAdi, dbo.BilesenHeader.BilesenHeaderAdi, dbo.BilesenItem.BilesenDegeri";

            result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();
            var potaKarts = new List<PotaKart>();




            var bilesenHeaders = new List<BilesenHeader>();
            bilesenHeaders.Add(new BilesenHeader() { Id = 0, BilesenHeaderAdi = "Seçimsiz" });

            var bilesenItems = new List<BilesenItem>();
            bilesenItems.Add(new BilesenItem() { Id = 0, BilesenDegeri = "Seçimsiz" });
            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();

                var PotaKartId = (int)lst[0];
                var BilesenItemId = (int)lst[1];
                var BilesenHeaderId = 0;
                if (BilesenItemId != 0)
                {
                    BilesenHeaderId = (int)lst[2];
                }

                var PotaKartAdi = lst[3]?.ToString();
                var BilesenHeaderAdi = lst[4]?.ToString();
                var BilesenDegeri = lst[5]?.ToString();

                RaporlamaPkVeBilesenItems.Add(new RaporlamaPkVeBilesenItem()
                {
                    BilesenHeaderId = BilesenHeaderId,
                    BilesenItemId = BilesenItemId,
                    BilesenDegeri = BilesenDegeri,
                    BilesenHeaderAdi = BilesenHeaderAdi,
                    PotaKartAdi = PotaKartAdi,
                    PotaKartId = PotaKartId
                });

                if (bilesenItems.Count(a => a.Id == BilesenItemId) == 0)
                {
                    bilesenItems.Add(new BilesenItem()
                    {
                        Id = BilesenItemId,
                        BilesenDegeri = BilesenDegeri
                    });
                }



                if (bilesenHeaders.Count(a => a.Id == BilesenHeaderId) == 0)
                {
                    bilesenHeaders.Add(new BilesenHeader()
                    {
                        Id = BilesenItemId,
                        BilesenHeaderAdi = BilesenHeaderAdi
                    });
                }


                if (potaKarts.Count(a => a.Id == PotaKartId) == 0)
                {
                    potaKarts.Add(new PotaKart()
                    {
                        Id = PotaKartId,
                        PotaKartAdi = PotaKartAdi
                    });
                }
            }
            #endregion



            foreach (var i in potaKarts)
            {
                var liste = RaporlamaPkVeBilesenItems.Where(a => a.PotaKartId == i.Id).ToList();

                var distHeaders = liste.Select(a => a.BilesenHeaderId).Distinct().ToList();

                var drops = new List<DropItem>();
                foreach (var headerId in distHeaders)
                {

                    if (bilesenHeaders.Any(a => a.Id == headerId))
                    {
                        var bilesenHeader = bilesenHeaders.First(a => a.Id == headerId);

                        var dropItem = new DropItem()
                        {
                            IdInt = bilesenHeader.Id,
                            Tanim = bilesenHeader.BilesenHeaderAdi
                        };

                        var bilesenItemIds = liste.Where(a => a.BilesenHeaderId == headerId)
                            .Select(a => a.BilesenItemId).Distinct().ToList();

                        foreach (var bilesenItemId in bilesenItemIds)
                        {
                            if (dropItem.ItemValues.Count(a => a.IdInt == bilesenItemId) == 0)
                            {
                                if (bilesenItems.Any(a => a.Id == bilesenItemId))
                                {
                                    var bilesenItem = bilesenItems.First(a => a.Id == bilesenItemId);

                                    dropItem.ItemValues.Add(new ItemValue()
                                    {
                                        IdInt = bilesenItem.Id,
                                        Text = bilesenItem.BilesenDegeri
                                    });
                                }
                            }
                        }

                        drops.Add(dropItem);
                    }
                }
                model.RaporlamaZePotakartItems.Add(new RaporlamaZePotakartItem()
                {
                    DropBilesenHeaderItems = drops,
                    DropItemPotaKart = new DropItem()
                    {
                        IdInt = i.Id,
                        Tanim = i.PotaKartAdi
                    }
                });
            }


            model.DropZeKategoriler = _db.ZamanEtuduKategoris.OrderBy(a => a.KategoriAdi).ToList()
                .Select(a => new DropItem() { IdInt = a.Id, Tanim = a.KategoriAdi, SeciliMi = true }).ToList();


            return View(model);
        }

        [HttpPost]
        public PartialViewResult AnalizYap(RaporlamaZeModel model)
        {


            var basTarihi = tarihiOlustur(model.BaslangicTarihi);
            var bitTarihi = tarihiOlustur(model.BitisTarihi.AddDays(1));

            var secilenKategoriler = model.DropZeKategoriler.Where(a => a.SeciliMi).Select(a => a.IdInt).ToList();

            var secilenPotakartIds = model.RaporlamaZePotakartItems.Where(a => a.DropItemPotaKart.SeciliMi)
                .Select(a => a.DropItemPotaKart.IdInt).ToList();
            var secilenUsers = model.DropUsers.Where(a => a.SeciliMi).Select(a => a.IdInt).ToList();

            var hamRaporlamaZeSonucItemDetails = new List<RaporlamaZeSonucItemDetail>();

            var squery = string.Format(
                "SELECT        TOP (100) PERCENT dbo.ZamanEtuduItemDetail.Id, dbo.ZamanEtuduItemDetail.ZamanEtuduKategoriId, dbo.ZamanEtuduItemDetail.KayitYapanId, dbo.ZamanEtuduItemDetail.PotaKartId,                          dbo.ZamanEtuduItemDetail.BilesenItemId, dbo.ZamanEtuduItemDetail.UserId, dbo.BilesenItem.BilesenHeaderId, dbo.ZamanEtuduItemDetail.OlcumDegeri, dbo.PotaKart.PotaKartAdi, dbo.BilesenHeader.BilesenHeaderAdi,                          dbo.BilesenItem.BilesenDegeri, dbo.ZamanEtuduItemDetail.Tarih, dbo.ZamanEtuduItemDetail.Aciklama, dbo.ZamanEtuduKategori.KategoriAdi, dbo.[User].Name FROM            dbo.ZamanEtuduKategori INNER JOIN                         dbo.ZamanEtuduItemDetail INNER JOIN                         dbo.PotaKart ON dbo.ZamanEtuduItemDetail.PotaKartId = dbo.PotaKart.Id ON dbo.ZamanEtuduKategori.Id = dbo.ZamanEtuduItemDetail.ZamanEtuduKategoriId INNER JOIN                         dbo.[User] ON dbo.ZamanEtuduItemDetail.UserId = dbo.[User].Id LEFT OUTER JOIN                         dbo.BilesenHeader RIGHT OUTER JOIN                         dbo.BilesenItem ON dbo.BilesenHeader.Id = dbo.BilesenItem.BilesenHeaderId ON dbo.ZamanEtuduItemDetail.BilesenItemId = dbo.BilesenItem.Id WHERE        (dbo.ZamanEtuduItemDetail.Tarih >= CONVERT(DATETIME, '{0}', 102)) AND (dbo.ZamanEtuduItemDetail.Tarih <= CONVERT(DATETIME, '{1}', 102)) ORDER BY dbo.ZamanEtuduItemDetail.Tarih, dbo.PotaKart.PotaKartAdi, dbo.BilesenHeader.BilesenHeaderAdi, dbo.BilesenItem.BilesenDegeri",
                basTarihi, bitTarihi);

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

            var zeKategoris = new List<ZamanEtuduKategori>();

            var tarihler = new List<DateTime>();

            var potaKarts = new List<PotaKart>();
            var bilesenHeaders = new List<BilesenHeader>();
            bilesenHeaders.Add(new BilesenHeader() { Id = 0, BilesenHeaderAdi = "Seçimsiz" });
            var bilesenItems = new List<BilesenItem>();
            bilesenItems.Add(new BilesenItem() { Id = 0, BilesenDegeri = "Seçimsiz" });



            foreach (var aa in result)
            {
                bool kayitYapilabilirMi = false;

                var lst = aa.Values.ToList();

                var Id = (int)lst[0];
                var ZamanEtuduKategoriId = (int)lst[1];
                var KayitYapanId = (int)lst[2];
                var PotaKartId = (int)lst[3];
                var BilesenItemId = (int)lst[4];
                var UserId = (int)lst[5];
                var BilesenHeaderId = 0;

                if (BilesenItemId != 0)
                {
                    BilesenHeaderId = (int)lst[6];
                }
                var OlcumDegeri = (decimal)lst[7];

                var PotaKartAdi = lst[8]?.ToString();
                var BilesenHeaderAdi = lst[9]?.ToString();
                var BilesenDegeri = lst[10]?.ToString();
                var Tarih = (DateTime)lst[11];
                var Aciklama = lst[12]?.ToString();
                var KategoriAdi = lst[13]?.ToString();
                var Name = lst[14]?.ToString();


                /*
                   secilenKategoriler
                secilenPotakartIds
            secilenUsers
                 */

                kayitYapilabilirMi = secilenKategoriler.Any(a => a == ZamanEtuduKategoriId);

                if (kayitYapilabilirMi)
                {
                    kayitYapilabilirMi = secilenPotakartIds.Any(a => a == PotaKartId);
                }

                if (kayitYapilabilirMi)
                {
                    kayitYapilabilirMi = secilenUsers.Any(a => a == UserId);
                }


                if (kayitYapilabilirMi)
                {
                    hamRaporlamaZeSonucItemDetails.Add(new RaporlamaZeSonucItemDetail()
                    {
                        UserGozleyen = new User()
                        {
                            Id = UserId,
                            Name = Name
                        },
                        ZamanEtuduItemDetail = new ZamanEtuduItemDetail()
                        {
                            BilesenItemId = BilesenItemId,
                            PotaKartId = PotaKartId,
                            Id = Id,
                            UserId = UserId,
                            Aciklama = Aciklama,
                            KayitYapanId = KayitYapanId,
                            OlcumDegeri = OlcumDegeri,
                            Tarih = Tarih,
                            ZamanEtuduKategoriId = ZamanEtuduKategoriId
                        }
                    });


                    if (bilesenItems.Count(a => a.Id == BilesenItemId) == 0)
                    {
                        bilesenItems.Add(new BilesenItem()
                        {
                            Id = BilesenItemId,
                            BilesenDegeri = BilesenDegeri
                        });
                    }



                    if (bilesenHeaders.Count(a => a.Id == BilesenHeaderId) == 0)
                    {
                        bilesenHeaders.Add(new BilesenHeader()
                        {
                            Id = BilesenItemId,
                            BilesenHeaderAdi = BilesenHeaderAdi
                        });
                    }


                    if (potaKarts.Count(a => a.Id == PotaKartId) == 0)
                    {
                        potaKarts.Add(new PotaKart()
                        {
                            Id = PotaKartId,
                            PotaKartAdi = PotaKartAdi
                        });
                    }

                    if (zeKategoris.Count(a => a.Id == ZamanEtuduKategoriId) == 0)
                    {
                        zeKategoris.Add(new ZamanEtuduKategori()
                        {
                            Id = ZamanEtuduKategoriId,
                            KategoriAdi = KategoriAdi
                        });
                    }

                    if (tarihler.Count(a => a == Tarih) == 0)
                    {
                        tarihler.Add(Tarih);
                    }
                }


            }

            zeKategoris = zeKategoris.OrderBy(a => a.KategoriAdi).ToList();
            tarihler = tarihler.OrderBy(a => a).ToList();

            var raporlamaZeSonucItems = new List<RaporlamaZeSonucItem>();
            int maxGozlemSay = 0;
            foreach (var zamanEtuduKategori in zeKategoris)
            {

                foreach (var dateTime in tarihler)
                {
                    var filtreliListe = hamRaporlamaZeSonucItemDetails.Where(a =>
                        a.ZamanEtuduItemDetail.Tarih == dateTime &&
                        a.ZamanEtuduItemDetail.ZamanEtuduKategoriId == zamanEtuduKategori.Id).ToList();

                    var pkIds = filtreliListe.Select(a => a.ZamanEtuduItemDetail.PotaKartId).Distinct().ToList();

                    foreach (var pkId in pkIds)
                    {
                        var pkListe = filtreliListe.Where(a => a.ZamanEtuduItemDetail.PotaKartId == pkId).ToList();


                        var pkBilesenItemIds = pkListe.Select(a => a.ZamanEtuduItemDetail.BilesenItemId).Distinct()
                            .ToList();

                        foreach (var bilesenItemId in pkBilesenItemIds)
                        {
                            
                            var bilesenItem = bilesenItems.First(a => a.Id == bilesenItemId);
                            var bilesenHeader = new BilesenHeader();
                            var potaKart = potaKarts.First(a => a.Id == pkId);

                            if (bilesenItem.Id!=0)
                            {
                                bilesenHeader = bilesenHeaders.First(a => a.Id == bilesenItem.BilesenHeaderId);

                            }
                            var resultItems = pkListe
                                .Where(a => a.ZamanEtuduItemDetail.BilesenItemId == bilesenItemId).ToList();

                          

                            var raporlamaZeSonucItem = new RaporlamaZeSonucItem()
                            {
                                BilesenHeader = bilesenHeader,
                                BilesenItem = bilesenItem,
                                PotaKart = potaKart,
                                Tarih = dateTime,
                                ZamanEtuduKategori = zamanEtuduKategori,
                                RaporlamaZeSonucItemDetails = resultItems,
                                DropItems = new List<DropItem>()
                            };
                            int tt = 0;
                            foreach (var ii in resultItems)
                            {
                                tt++;
                                raporlamaZeSonucItem.DropItems.Add(new DropItem()
                                {
                                    IdInt = tt,
                                    Tanim = ii.ZamanEtuduItemDetail.Aciklama
                                });
                            }

                            if (tt> maxGozlemSay)
                            {
                                maxGozlemSay = tt;
                            }
                            model.RaporlamaZeSonucItems.Add(raporlamaZeSonucItem);
                        }

                    }
                }

            }

            model.maxGozlemSayisi = maxGozlemSay;
            return PartialView("_AnalizYap", model);
        }

        public ActionResult MakinaBazliZeAnalizHazirla()
        {
            var model = new RaporlamaZeModel();
            var squery =
                "SELECT DISTINCT TOP (100) PERCENT dbo.ZamanEtuduItemDetail.PotaKartId, dbo.PotaKart.PotaKartAdi FROM            dbo.ZamanEtuduItemDetail INNER JOIN                         dbo.PotaKart ON dbo.ZamanEtuduItemDetail.PotaKartId = dbo.PotaKart.Id ORDER BY dbo.PotaKart.PotaKartAdi";

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();

                model.DropFiltrePotaKart.ItemValues.Add(new ItemValue()
                {
                    IdInt = (int)lst[0],
                    Text = lst[1]?.ToString()
                });
            }
            return View(model);
        }


        [HttpPost]
        public PartialViewResult MakinaBazliZeAnalizYap(RaporlamaZeModel model)
        {


            var basTarihi = tarihiOlustur(model.BaslangicTarihi);
            var bitTarihi = tarihiOlustur(model.BitisTarihi.AddDays(1));

            var seciliPkIds = model.DropFiltrePotaKart.IntList;


            var zeKategoris = new List<ZamanEtuduKategori>();

            var tarihler = new List<DateTime>();

            var potaKarts = new List<PotaKart>();
            var bilesenHeaders = new List<BilesenHeader>();
            bilesenHeaders.Add(new BilesenHeader() { Id = 0, BilesenHeaderAdi = "Seçimsiz" });
            var bilesenItems = new List<BilesenItem>();
            bilesenItems.Add(new BilesenItem() { Id = 0, BilesenDegeri = "Seçimsiz" });


            var hamRaporlamaZeSonucItemDetails = new List<RaporlamaZeSonucItemDetail>();
            #region hamRaporlamaZeSonucItemDetails


            var squery = string.Format(
                "SELECT        TOP (100) PERCENT dbo.ZamanEtuduItemDetail.Id, dbo.ZamanEtuduItemDetail.ZamanEtuduKategoriId, dbo.ZamanEtuduItemDetail.KayitYapanId, dbo.ZamanEtuduItemDetail.PotaKartId,                          dbo.ZamanEtuduItemDetail.BilesenItemId, dbo.ZamanEtuduItemDetail.UserId, dbo.BilesenItem.BilesenHeaderId, dbo.ZamanEtuduItemDetail.OlcumDegeri, dbo.PotaKart.PotaKartAdi, dbo.BilesenHeader.BilesenHeaderAdi,                          dbo.BilesenItem.BilesenDegeri, dbo.ZamanEtuduItemDetail.Tarih, dbo.ZamanEtuduItemDetail.Aciklama, dbo.ZamanEtuduKategori.KategoriAdi, dbo.[User].Name FROM            dbo.ZamanEtuduKategori INNER JOIN                         dbo.ZamanEtuduItemDetail INNER JOIN                         dbo.PotaKart ON dbo.ZamanEtuduItemDetail.PotaKartId = dbo.PotaKart.Id ON dbo.ZamanEtuduKategori.Id = dbo.ZamanEtuduItemDetail.ZamanEtuduKategoriId INNER JOIN                         dbo.[User] ON dbo.ZamanEtuduItemDetail.UserId = dbo.[User].Id LEFT OUTER JOIN                         dbo.BilesenHeader RIGHT OUTER JOIN                         dbo.BilesenItem ON dbo.BilesenHeader.Id = dbo.BilesenItem.BilesenHeaderId ON dbo.ZamanEtuduItemDetail.BilesenItemId = dbo.BilesenItem.Id WHERE        (dbo.ZamanEtuduItemDetail.Tarih >= CONVERT(DATETIME, '{0}', 102)) AND (dbo.ZamanEtuduItemDetail.Tarih <= CONVERT(DATETIME, '{1}', 102)) ORDER BY dbo.ZamanEtuduItemDetail.Tarih, dbo.PotaKart.PotaKartAdi, dbo.BilesenHeader.BilesenHeaderAdi, dbo.BilesenItem.BilesenDegeri",
                basTarihi, bitTarihi);

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();


            #endregion
            foreach (var aa in result)
            {
                bool kayitYapilabilirMi = false;

                var lst = aa.Values.ToList();

                var Id = (int)lst[0];
                var ZamanEtuduKategoriId = (int)lst[1];
                var KayitYapanId = (int)lst[2];
                var PotaKartId = (int)lst[3];
                var BilesenItemId = (int)lst[4];
                var UserId = (int)lst[5];
                var BilesenHeaderId = 0;

                if (BilesenItemId != 0)
                {
                    BilesenHeaderId = (int)lst[6];
                }
                var OlcumDegeri = (decimal)lst[7];

                var PotaKartAdi = lst[8]?.ToString();
                var BilesenHeaderAdi = lst[9]?.ToString();
                var BilesenDegeri = lst[10]?.ToString();
                var Tarih = (DateTime)lst[11];
                var Aciklama = lst[12]?.ToString();
                var KategoriAdi = lst[13]?.ToString();
                var Name = lst[14]?.ToString();


                /*
                   secilenKategoriler
                secilenPotakartIds
            secilenUsers
                 */

                kayitYapilabilirMi = seciliPkIds.Any(a => a == PotaKartId);




                if (kayitYapilabilirMi)
                {
                    hamRaporlamaZeSonucItemDetails.Add(new RaporlamaZeSonucItemDetail()
                    {
                        UserGozleyen = new User()
                        {
                            Id = UserId,
                            Name = Name
                        },
                        ZamanEtuduItemDetail = new ZamanEtuduItemDetail()
                        {
                            BilesenItemId = BilesenItemId,
                            PotaKartId = PotaKartId,
                            Id = Id,
                            UserId = UserId,
                            Aciklama = Aciklama,
                            KayitYapanId = KayitYapanId,
                            OlcumDegeri = OlcumDegeri,
                            Tarih = Tarih,
                            ZamanEtuduKategoriId = ZamanEtuduKategoriId
                        }
                    });


                    if (bilesenItems.Count(a => a.Id == BilesenItemId) == 0)
                    {
                        bilesenItems.Add(new BilesenItem()
                        {
                            Id = BilesenItemId,
                            BilesenDegeri = BilesenDegeri
                        });
                    }



                    if (bilesenHeaders.Count(a => a.Id == BilesenHeaderId) == 0)
                    {
                        bilesenHeaders.Add(new BilesenHeader()
                        {
                            Id = BilesenItemId,
                            BilesenHeaderAdi = BilesenHeaderAdi
                        });
                    }


                    if (potaKarts.Count(a => a.Id == PotaKartId) == 0)
                    {
                        potaKarts.Add(new PotaKart()
                        {
                            Id = PotaKartId,
                            PotaKartAdi = PotaKartAdi
                        });
                    }

                    if (zeKategoris.Count(a => a.Id == ZamanEtuduKategoriId) == 0)
                    {
                        zeKategoris.Add(new ZamanEtuduKategori()
                        {
                            Id = ZamanEtuduKategoriId,
                            KategoriAdi = KategoriAdi
                        });
                    }

                    if (tarihler.Count(a => a == Tarih) == 0)
                    {
                        tarihler.Add(Tarih);
                    }
                }


            }

            var raporlamaZeSonucItems = new List<RaporlamaZeSonucItem>();
            int maxGozlemSay = 0;

            zeKategoris = zeKategoris.OrderBy(a => a.KategoriAdi).ToList();
            tarihler = tarihler.OrderByDescending(a => a).ToList();
            foreach (var zamanEtuduKategori in zeKategoris)
            {

                foreach (var dateTime in tarihler)
                {
                    var filtreliListe = hamRaporlamaZeSonucItemDetails.Where(a =>
                        a.ZamanEtuduItemDetail.Tarih == dateTime &&
                        a.ZamanEtuduItemDetail.ZamanEtuduKategoriId == zamanEtuduKategori.Id).ToList();

                    var pkIds = filtreliListe.Select(a => a.ZamanEtuduItemDetail.PotaKartId).Distinct().ToList();

                    foreach (var pkId in pkIds)
                    {
                        var pkListe = filtreliListe.Where(a => a.ZamanEtuduItemDetail.PotaKartId == pkId).ToList();


                        var pkBilesenItemIds = pkListe.Select(a => a.ZamanEtuduItemDetail.BilesenItemId).Distinct()
                            .ToList();

                        foreach (var bilesenItemId in pkBilesenItemIds)
                        {

                            var bilesenItem = bilesenItems.First(a => a.Id == bilesenItemId);
                            var bilesenHeader = new BilesenHeader();
                            var potaKart = potaKarts.First(a => a.Id == pkId);

                            if (bilesenItem.Id != 0)
                            {
                                bilesenHeader = bilesenHeaders.First(a => a.Id == bilesenItem.BilesenHeaderId);

                            }
                            var resultItems = pkListe
                                .Where(a => a.ZamanEtuduItemDetail.BilesenItemId == bilesenItemId).ToList();



                            var raporlamaZeSonucItem = new RaporlamaZeSonucItem()
                            {
                                BilesenHeader = bilesenHeader,
                                BilesenItem = bilesenItem,
                                PotaKart = potaKart,
                                Tarih = dateTime,
                                ZamanEtuduKategori = zamanEtuduKategori,
                                RaporlamaZeSonucItemDetails = resultItems,
                                DropItems = new List<DropItem>()
                            };
                            int tt = 0;
                            foreach (var ii in resultItems)
                            {
                                tt++;
                                raporlamaZeSonucItem.DropItems.Add(new DropItem()
                                {
                                    IdInt = tt,
                                    Tanim = ii.ZamanEtuduItemDetail.Aciklama
                                });
                            }

                            if (tt > maxGozlemSay)
                            {
                                maxGozlemSay = tt;
                            }
                            model.RaporlamaZeSonucItems.Add(raporlamaZeSonucItem);
                        }

                    }
                }

            }
            model.maxGozlemSayisi = maxGozlemSay;
            return PartialView("_AnalizYap", model);
        }
    }
}