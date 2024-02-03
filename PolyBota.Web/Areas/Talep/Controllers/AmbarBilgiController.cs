using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using PolyBota.BLL;
using PolyBota.DAL;
using PolyBota.Entities;
using PolyBota.Helpers;
using PolyBota.Web.Areas.Talep.Models;
using PolyBota.Web.Controllers;

namespace PolyBota.Web.Areas.Talep.Controllers
{
    public class AmbarBilgiController : BaseController
    {
        // GET: Talep/AmbarBilgi

        #region Ambar Stok Arama

        public ActionResult AmbarStokAra()
        {
            return View(new TalepAmbarModel());
        }

        public PartialViewResult StokAra(string str)
        {

            var model = new TalepAmbarModel
            {

            };


            var sorgu = string.Format(
                " SELECT  TOP (20)  Id, LotNo, StokKodu, StokTanimAdi FROM     dbo.StokKart WHERE  (StokKodu LIKE '%{0}%') OR (StokTanimAdi LIKE '%{0}%') ORDER BY StokTanimAdi",
                str);

            var result = BllMssql.CustomSql(sorgu, SuaHelper.defaultSql()).ToList();

            foreach (var squery in result.ToList())
            {
                var lst = squery.Values.ToList();

                try
                {
                    int id = Convert.ToInt32(lst[0]);
                    var LotNo = lst[1]?.ToString();
                    var StokKodu = lst[2]?.ToString();
                    var StokTanimAdi = lst[3]?.ToString();
                    model.StokKarts.Add(new StokKart()
                    {
                        Id = id,
                        LotNo = LotNo,
                        StokKodu = StokKodu,
                        StokTanimAdi = StokTanimAdi
                    });
                }
                catch (Exception e)
                {
                    var aa = e.Message;
                }

            }

            return PartialView("_StokAra", model);

        }

        [HttpPost]
        public PartialViewResult AmbarStokAramaSonuc(TalepAmbarModel model)
        {


            model.AmbarStokKarts = new List<AmbarStokKartModel>();

            var secilenStokIdler = model.StokKarts.Where(a => a.SeciliMi).ToList().Select(a => a.Id).ToList();

            var secilenStoklar = new List<StokKart>();

            foreach (var i in secilenStokIdler)
            {
                secilenStoklar.Add(_db.StokKarts.Find(i));
            }


            secilenStoklar = secilenStoklar.OrderBy(a => a.StokTanimAdi).ToList();

            var ptksAmbarlar = _db.Ambars.Where(a => a.AmbarTipi == 0).AsNoTracking().ToList();
            var Ambarlar = _db.Ambars.Where(a => a.AmbarTipi != 0).AsNoTracking().ToList();
            model.Ambars.AddRange(ptksAmbarlar);
            model.Ambars.AddRange(Ambarlar);
            foreach (var stokKart in secilenStoklar)
            {

                var araItem = new AmbarStokKartModel()
                {
                    StokKart = stokKart,

                };
                var spResult = _db.SpAmbarStokGetir(stokKart.StokKodu, stokKart.LotNo).ToList();

                if (spResult.Any())
                {

                    foreach (var item in spResult.ToList())
                    {
                        foreach (var amb in ptksAmbarlar)
                        {
                            if (item.AmbarNo.Replace(" ", "") == amb.PotaAmbarNo.Replace(" ", ""))
                            {

                                var ambarStok = new AmbarStokKart();

                                if (_db.AmbarStokKarts.Any(a => a.StokKartId == stokKart.Id && a.AmbarId == amb.Id))
                                {
                                    ambarStok = _db.AmbarStokKarts.First(a =>
                                        a.StokKartId == stokKart.Id && a.AmbarId == amb.Id);
                                }
                                else
                                {
                                    ambarStok = new AmbarStokKart()
                                    {
                                        StokKartId = stokKart.Id,
                                        AmbarId = amb.Id,
                                        ToplamMiktar = 0
                                    };
                                    _db.AmbarStokKarts.Add(ambarStok);
                                    _db.SaveChanges();
                                }
                                ambarStok.ToplamMiktar = item.Miktar1 ?? 0;

                                araItem.PTKSAmbarStokKarts.Add(ambarStok);

                            }
                        }
                    }


                }


                var koltukAmbars = _db.AmbarStokKarts.Where(a => a.StokKartId == stokKart.Id && a.ToplamMiktar > 0)
                    .ToList();
                araItem.AmbarStokKarts = koltukAmbars;

                model.AmbarStokKarts.Add(araItem);
            }

            return PartialView("_AmbarStokAramaSonuc", model);
        }

        #endregion

        #region Ambar Stokları

        public ActionResult AmbarStokListe()
        {
            var model = new TalepAmbarModel()
            {
                DropItems = new List<DropItem>()
            };

            var ambars = _db.Ambars.OrderBy(a => a.AmbarTipi).ThenBy(a => a.AmbarAdi).ToList();
            var ambarTipis = _db.AmbarTipiTanims.ToList();


            foreach (var ambar in ambars)
            {
                var tanim = ambarTipis.First(a=>a.Id==ambar.AmbarTipi).AmbarTipAdi;

                
               
                if (ambar.AmbarTipi == 0)
                {
                    tanim = string.Format("({0}) {1} - ({2})", tanim, ambar.AmbarAdi, ambar.PotaAmbarNo);
                }
                else
                {
                    tanim = string.Format("({0})  {1}",tanim, ambar.AmbarAdi);
                }

                model.DropItems.Add(new DropItem()
                {
                    Tanim = tanim,
                    Id = ambar.Id.ToString()
                });
            }
            return View(model);
        }

        public PartialViewResult AmbarinStokListesi(int id)
        {
            var model = new TalepAmbarModel();
            var ambar = _db.Ambars.Find(id);


            if (ambar.AmbarTipi == 0)
            {
                var lst = _db.SpAmbarStoklar(ambar.PotaAmbarNo).Where(a => a.Miktar1 > 0).ToList();


                foreach (var item in lst.OrderBy(a => a.StokAdi).ToList())
                {
                    var araModel = new AmbarStokKartModel
                    {
                        StokKart = new StokKart()
                        {
                            Id = item.Id ?? 0,
                            StokTanimAdi = item.StokAdi,
                            LotNo = item.LotNo,
                            StokKodu = item.StokKodu
                        }
                    };
                    araModel.PTKSAmbarStokKarts.Add(new AmbarStokKart()
                    {
                        StokKartId = item.Id ?? 0,
                        ToplamMiktar = item.Miktar1 ?? 0,
                        AmbarId = id
                    });
                    model.AmbarStokKarts.Add(araModel);
                }
            }

            if (ambar.AmbarTipi==1)
            {
                // bota ambar

                var squery = string.Format(
                    " SELECT dbo.AmbarStokKart.Id, dbo.AmbarStokKart.AmbarId, dbo.AmbarStokKart.StokKartId, dbo.AmbarStokKart.ToplamMiktar, dbo.StokKart.StokTanimAdi, dbo.StokKart.LotNo, dbo.StokKart.StokKodu FROM     dbo.AmbarStokKart INNER JOIN dbo.StokKart ON dbo.AmbarStokKart.StokKartId = dbo.StokKart.Id WHERE  (dbo.AmbarStokKart.AmbarId = {0}) ORDER BY dbo.StokKart.StokTanimAdi",
                    id);



                var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();



                foreach (var sItem in result)
                {

                    var lst = sItem.Values.ToList();

                    var ambarStokKartId = (int)(lst[0] ?? 0);
                    var ambarId = (int)(lst[1] ?? 0);
                    var stokKartId = (int)(lst[2] ?? 0);
                    var toplamMiktar = (decimal)(lst[3] ?? 0);
                    var stokTanimAdi = lst[4]?.ToString();
                    var lotNo = lst[5]?.ToString();
                    var stokKodu = lst[6]?.ToString();
                   
                    var araModel = new AmbarStokKartModel
                    {
                        StokKart = new StokKart()
                        {
                            Id = stokKartId,
                            StokTanimAdi = stokTanimAdi,
                            LotNo = lotNo,
                            StokKodu = stokKodu
                        }
                    };
                    araModel.PTKSAmbarStokKarts.Add(new AmbarStokKart()
                    {
                        StokKartId = stokKartId,
                        ToplamMiktar = toplamMiktar,
                        AmbarId = id
                    });
                    model.AmbarStokKarts.Add(araModel);
                }
            }

            return PartialView("_AmbarinStokListesi", model);
        }

        #endregion

        #region Ambar Stok Talebi düzenleme ve ptks hareket

        public PartialViewResult AmbarStokTalebi(int id, int ambarId)
        {
            var model = new TalepAmbarModel
            {
                AmbarStokTalep = new AmbarStokTalep()
                {
                    StokKartId = id,
                    TalepEdilenAmbarId = ambarId
                },
                DropItems = new List<DropItem>()
            };
            var stokKart = _db.StokKarts.Find(id);

            var ambar = _db.Ambars.Find(ambarId);

            model.StokKart = stokKart;
            model.Ambar = ambar;
            model.Ambars = _db.Ambars.Where(a => a.AmbarTipi == 1).OrderBy(a => a.AmbarAdi).ToList();
            decimal miktar = 0;

            if (ambar.AmbarTipi == 0)
            {
                // pota ambar

                var spResult = _db.SpAmbarStokGetir(stokKart.StokKodu, stokKart.LotNo).ToList();

                foreach (var item in spResult)
                {
                    if (item.AmbarNo.Replace(" ", "") == ambar.PotaAmbarNo.Replace(" ", ""))
                    {
                        miktar = item.Miktar1 ?? 0;
                    }
                }
            }


            for (int i = 1; i <= miktar; i++)
            {
                model.DropItems.Add(new DropItem()
                {
                    Tanim = i.ToString(),
                    Id = i.ToString()
                });
            }

            return PartialView("_AmbarStokTalebi", model);
        }

        [HttpPost]
        public JsonResult AmbarStokTalebi(TalepAmbarModel model)
        {
            int state = 1;
            var title = "Talep Oluşturulmuştur";
            var icon = "success";
            var talep = model.AmbarStokTalep;

            var yeniKayit = new AmbarStokTalep()
            {
                StokKartId = talep.StokKartId,
                AktarilacakAmbarId = talep.AktarilacakAmbarId,
                TalepEdilenAmbarId = talep.TalepEdilenAmbarId,
                Miktar = talep.Miktar,
                KayitYapan = User.Id,
                KayitTarihi = DateTime.Now,
                OnayTarihi = DateTime.Now,
                OnayVeren = 0,
                TalepDurumu = 1
            };
            _db.AmbarStokTaleps.Add(yeniKayit);
            _db.SaveChanges();

            return new JsonResult { Data = new { state = state, title = title, icon = icon }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }



        public PartialViewResult StokTalebiDuzenle(int id, int tip)
        {
            // tip:1 => kendi kayıtlarını düzenleyecek, tip:2 => onay verilecek 
            var model = new TalepAmbarModel()
            {
                tip = tip,
                Ambars = new List<Ambar>(),
                DropItems = new List<DropItem>()
            };
            var talep = _db.AmbarStokTaleps.Find(id);
            var talepAmbar = _db.Ambars.Find(talep.TalepEdilenAmbarId);
            var stokKart = _db.StokKarts.Find(talep.StokKartId);
            model.MiktarInt = Convert.ToInt32(talep.Miktar);

            model.User = _db.Users.Find(talep.KayitYapan);
            decimal miktar = 0;
            if (tip == 1)
            {
                if (talep.KayitYapan == User.Id)
                {
                    model.Ambars = _db.Ambars.Where(a => a.AmbarTipi == 1).OrderBy(a => a.AmbarAdi).ToList();

                    if (talepAmbar.AmbarTipi == 0)
                    {
                        // pota ambar
                        var spResult = _db.SpAmbarStokGetir(stokKart.StokKodu, stokKart.LotNo).ToList();
                        foreach (var item in spResult)
                        {
                            if (item.AmbarNo.Replace(" ", "") == talepAmbar.PotaAmbarNo.Replace(" ", ""))
                            {
                                miktar = item.Miktar1 ?? 0;
                            }
                        }
                    }
                    for (int i = 1; i <= miktar; i++)
                    {
                        model.DropItems.Add(new DropItem()
                        {
                            Tanim = i.ToString(),
                            Id = i.ToString()
                        });
                    }
                }
                else
                {
                    model.durum = 1;
                }
            }
            else
            {
                // onay verilecek 

                model.TigerAmbars = _db.Ambars.Where(a => a.AmbarTipi == 3).OrderBy(a => a.AmbarAdi).ToList();


            }
            model.Ambar = talepAmbar;
            model.AmbarStokTalep = talep;
            model.StokKart = stokKart;
            return PartialView("_StokTalebiDuzenle", model);
        }


        private long PtksStokBotaHareket(AmbarStokTalep talep)
        {
            var potaAmbar = _db.Ambars.Find(talep.TalepEdilenAmbarId);
            var koltukAmbar = _db.Ambars.Find(talep.AktarilacakAmbarId);
            var miktar = talep.Miktar;
            var talepUser = _db.Users.Find(talep.KayitYapan);

            var stokKart = _db.StokKarts.Find(talep.StokKartId);

            /*
             pota aktarımı yapılacak
             */

            var ambarStokKart = new AmbarStokKart();

            if (_db.AmbarStokKarts.Any(a => a.StokKartId == stokKart.Id && a.AmbarId == koltukAmbar.Id))
            {
                ambarStokKart =
                    _db.AmbarStokKarts.First(a => a.StokKartId == stokKart.Id && a.AmbarId == koltukAmbar.Id);

            }
            else
            {
                ambarStokKart = new AmbarStokKart()
                {
                    StokKartId = stokKart.Id,
                    AmbarId = koltukAmbar.Id,
                    ToplamMiktar = 0
                };
                _db.AmbarStokKarts.Add(ambarStokKart);
                _db.SaveChanges();
            }

            var aciklama = string.Format("Koltuk Ambara Stok Aktarımı - Talep Eden: {0} Onay Veren {1}",talepUser.Name.ToString(), User.Name.ToString());

            int PtksFisNo = 0; //değişecek
            int PtksSiraNo = 0; //değişecek
            
            return BotaAmbarStokHareketOlustur(ambarStokKart.Id, 0, miktar, aciklama, PtksFisNo, PtksSiraNo);
        }

        [HttpPost]
        public JsonResult StokTalebiDuzenle(TalepAmbarModel model)
        {
            int state = 1;
            int id = 0;
            try
            {
                var tip = model.tip;
                var miktar = model.MiktarInt;
                var talep = model.AmbarStokTalep;
                if (tip == 1)
                {
                    // kendi kayıdını düzenleyecek

                    var editItem = _db.AmbarStokTaleps.Find(talep.Id);
                    editItem.Miktar = miktar;
                    editItem.AktarilacakAmbarId = talep.AktarilacakAmbarId;
                    editItem.TalepDurumu = talep.TalepDurumu;
                    _db.SaveChanges();
                }
                if (tip == 2)
                {
                    // onay verilecek
                    if (model.durum == 1)
                    {
                        // sorumlu ambar için onay verdi
                        var editItem = _db.AmbarStokTaleps.Find(talep.Id);
                        editItem.TalepDurumu = 2; //Onay Verildi Stok Aktarıldı
                        editItem.OnayVeren = User.Id;
                        editItem.OnayTarihi = DateTime.Now;
                        editItem.TigerAmbarId = talep.TigerAmbarId;
                        _db.SaveChanges();
                        // stok aktarımı
                        editItem.AmbarStokKartHareketId = PtksStokBotaHareket(editItem);
                        _db.SaveChanges();
                    }


                }

                TempDataCreate(2);
            }
            catch (Exception e)
            {
                state = 0;
            }
            return new JsonResult { Data = new { Id = id, state = state }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion


        #region ambar stok talep Liste

        private TalepAmbarModel TalepAmbarModeliGetir(int CurrentPage, int PageShowCount, int id)
        {
            var model = new TalepAmbarModel()
            {
                Users = new List<User>(),
                AmbarStokTaleps = new List<AmbarStokTalep>(),
                Ambars = new List<Ambar>()
            };


            var itemIdler = _db.AmbarStokTaleps.Where(a => a.TalepDurumu == id).OrderByDescending(a => a.KayitTarihi)
                .Select(a => a.Id).ToList();

            var pliste = new PagedListBase<int>() { CurrentPage = CurrentPage, PageShowCount = PageShowCount, DataLists = itemIdler };
            var PageListBase = PageListBaseOlustur(pliste);

            model.PagedListSrcn = new PagedListSrcn()
            {
                PageShowCount = PageListBase.PageShowCount,
                PageSizeSelectList = PageListBase.PageSizeSelectList,
                PageNumberList = PageListBase.PageNumberList,
                CurrentPage = PageListBase.CurrentPage
            };

            var userIdList = new List<int>();
            var ambarIdList = new List<int>();
            var stokKartIdList = new List<int>();
            foreach (var i in PageListBase.DataLists)
            {
                var talep = _db.AmbarStokTaleps.Find(i);

                if (userIdList.Count(b => b == talep.KayitYapan) == 0)
                {
                    userIdList.Add(talep.KayitYapan);
                }
                if (userIdList.Count(b => b == talep.OnayVeren) == 0)
                {
                    userIdList.Add(talep.OnayVeren);
                }
                if (ambarIdList.Count(b => b == talep.TalepEdilenAmbarId) == 0)
                {
                    ambarIdList.Add(talep.TalepEdilenAmbarId);
                }
                if (ambarIdList.Count(b => b == talep.AktarilacakAmbarId) == 0)
                {
                    ambarIdList.Add(talep.AktarilacakAmbarId);
                }
                if (stokKartIdList.Count(b => b == talep.StokKartId) == 0)
                {
                    stokKartIdList.Add(talep.StokKartId);
                }

                model.AmbarStokTaleps.Add(talep);
            }

            userIdList = userIdList.Where(a => a != 0).ToList();

            foreach (var i in userIdList)
            {
                model.Users.Add(_db.Users.Find(i));
            }
            foreach (var i in ambarIdList)
            {
                model.Ambars.Add(_db.Ambars.Find(i));
            }
            foreach (var i in stokKartIdList)
            {
                model.StokKarts.Add(_db.StokKarts.Find(i));
            }
            return model;
        }
        public ActionResult StokTalepEdilenListe(int CurrentPage = 1, int PageShowCount = 50, int katId = 0)
        {
            var model = TalepAmbarModeliGetir(CurrentPage, PageShowCount, 1);
            return View(model);
        }
        public ActionResult StokOnaylananListe(int CurrentPage = 1, int PageShowCount = 50, int katId = 1)
        {
            var model = TalepAmbarModeliGetir(CurrentPage, PageShowCount, katId);
            model.Id = katId;
            model.DropItems = new List<DropItem>();
            model.DropItems.Add(new DropItem() { Id = "1", Tanim = "Onay Bekleyen Liste" });
            model.DropItems.Add(new DropItem() { Id = "2", Tanim = "Onay Verilen Liste" });
            return View(model);
        }
        #endregion


    }
}