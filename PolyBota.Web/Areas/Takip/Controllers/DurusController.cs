using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PolyBota.BLL;
using PolyBota.DAL;
using PolyBota.Entities;
using PolyBota.Helpers;
using PolyBota.Web.Areas.Takip.Models;
using PolyBota.Web.Controllers;

namespace PolyBota.Web.Areas.Takip.Controllers
{
    public class DurusController : BaseController
    {
        // GET: Takip/Durus

        private DurusStokKartModel durusModeliPostaGoreHazirla(DurusStokKartModel model)
        {
            var BaslangisTarihi = model.BaslangisTarihi;

            var BitisTarihi = model.BitisTarihi;
            var stokKart = _db.StokKarts.Find(model.Id);
            model.StokKart = stokKart;
            var tableHeaders = DropTakvimTableHeader(BaslangisTarihi, BitisTarihi, model.GosterimSekli);

            model.TableHeaderlar = tableHeaders;
            model.TakipTakvimModels = new List<List<TakipTakvimModel>>();

            var takvimModel = new List<List<TakipTakvimModel>>();

            var duruslar = _db.StokKartDurus
                .Where(a => a.StokKartId == stokKart.Id && a.DurusBaslangic >= BaslangisTarihi &&
                            a.DurusBaslangic <= BitisTarihi).OrderBy(a => a.DurusBaslangic).ToList();

            var araModel = new List<TakipTakvimModel>();


            araModel.Add(new TakipTakvimModel()
            {
                StokKart = stokKart
            });



            foreach (var tableHeader in tableHeaders)
            {
                var araItem = new TakipTakvimModel();
                araItem.StokKart = stokKart;


                var aralikDuruslar = duruslar.Where(a =>
                    a.DurusBaslangic >= tableHeader.DateTime && a.DurusBaslangic <= tableHeader.DateTime2).ToList();

                if (aralikDuruslar.Any())
                {
                    araItem.StokKartDurus = aralikDuruslar;
                }
                else
                {
                    araItem.StokKartOperasyons.Add(new StokKartOperasyon());
                    araItem.StokKartDurus.Add(new StokKartDuru());
                }
                araModel.Add(araItem);
            }


            takvimModel.Add(araModel);


            model.TakipTakvimModels = takvimModel;
            model.DurusTipiTanims = _db.DurusTipiTanims.ToList();
            return model;
        }
        public ActionResult MakineDurus(int id=0)
        {
            var model = new DurusStokKartModel
            {
                Id = id,

            };
            if (id!=0)
            {
                model = durusModeliPostaGoreHazirla(model);

            }

            
            return View(model);
        }
        public PartialViewResult MakineAra(string makId)
        {
            var model = new DurusStokKartModel();
            var str = string.Format(
                "SELECT TOP 20 dbo.StokKart.Id, dbo.StokKart.StokKodu, dbo.StokKart.StokTanimAdi, dbo.KomponentTanim.KomponentTanimAdi, dbo.KomponentTanimGrup.KomponentTanimGrupAdi, dbo.Bolum.BolumAdi FROM dbo.Bolum RIGHT OUTER JOIN dbo.StokKart LEFT OUTER JOIN dbo.KomponentTanimGrup INNER JOIN dbo.KomponentTanim ON dbo.KomponentTanimGrup.Id = dbo.KomponentTanim.KomponentTanimGrupId ON dbo.StokKart.KomponentTanimId = dbo.KomponentTanim.Id ON dbo.Bolum.Id = dbo.StokKart.BolumId WHERE(dbo.StokKart.StokKodu LIKE '%{0}%') OR (dbo.StokKart.StokTanimAdi LIKE '%{0}%')  ORDER BY dbo.StokKart.StokTanimAdi", makId);

            var squery = BllMssql.CustomSql(str, SuaHelper.defaultSql()).ToList();


            foreach (var itt in squery)
            {
                var lst = itt.Values.ToList();

                var stokKartId = lst[0]?.ToString();
                var stokKodu = lst[1]?.ToString();
                var stokTanimAdi = lst[2]?.ToString();
                var kompTanimAdi = lst[3]?.ToString();
                var kompTanimGrubu = lst[4]?.ToString();
                var bolumAdi = lst[5]?.ToString();

                model.DropStokKarts.Add(new DropItem()
                {
                    Tanim1 = stokTanimAdi,
                    Tanim3 = string.Format("{0} ->{1} ", kompTanimGrubu, kompTanimAdi),
                    Tanim2 = stokKodu,
                    Id = stokKartId
                });

            }

            return PartialView("_MakineAra", model);
        }
        [HttpPost]
        public PartialViewResult MakineDurusAnaliz(DurusStokKartModel model)
        {
            model.Id = model.StokKart.Id;
            model = durusModeliPostaGoreHazirla(model);
            return PartialView("_MakineDurusAnaliz", model);
        }
        public PartialViewResult DurusEkleDuzenle(int id, int ItemId)
        {
            var model = new DurusStokKartModel
            {
                Id = ItemId,
                StokKartDuru = new StokKartDuru(){StokKartId = id,DurusBaslangic = DateTime.Now.Date.AddDays(-1), DurusBitis = DateTime.Now.Date},
                DurusGrubuTanims = _db.DurusGrubuTanims.OrderBy(a=>a.DurusGrubuTanimAdi).ToList(),
                DurusTipiTanims = _db.DurusTipiTanims.OrderBy(a=>a.DurusTipi).ToList(),
                StokKart = _db.StokKarts.Find(id)
            };

            if (ItemId!=0)
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

            if (durus.DurusTipi!=0)
            {

                state = 1;

                var totalSure =Convert.ToDecimal( Math.Abs(span.TotalHours));

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
                        .Where(a => a.OperasyonDurumu==0 &&  a.StokKartId==stokKart.Id).ToList();

                    foreach (var stokKartOperasyon in lst)
                    {
                        stokKartOperasyon.PlanlananTarih = stokKartOperasyon.PlanlananTarih.AddHours(Convert.ToDouble(totalSure));
                        _db.SaveChanges();
                    }
                }
            }
            return new JsonResult { Data = new { Id = stokKart.Id, state = state }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}