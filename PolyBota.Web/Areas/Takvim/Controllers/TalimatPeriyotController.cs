using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PolyBota.DAL;
using PolyBota.Entities;
using PolyBota.Web.Areas.Takvim.Model;
using PolyBota.Web.Controllers;

namespace PolyBota.Web.Areas.Takvim.Controllers
{
    public class TalimatPeriyotController : BaseController
    {
        // GET: Takvim/TalimatPeriyot

        public TakvimTalimatModel TakvimOperasyonModeliAnalizYap(TakvimTalimatModel model)
        {

           

            var baslangic = model.BaslangisTarihi.Date;
            var bitis = model.BitisTarihi.AddDays(1).Date;

            var header = DropTakvimTableHeader(model.BaslangisTarihi, model.BitisTarihi, model.GosterimSekli);


            var idler = model.PotaKarts.Where(a => a.SeciliMi).Select(a => a.Id).ToList();
            var kartListe = _db.PotaKarts.Where(a => idler.Any(b => b == a.Id)).OrderBy(a => a.PotaKartAdi).ToList();

            var operasyonlar = new List<StokKartOperasyon>();
            var duruslar = new List<StokKartDuru>();

            foreach (var potaKart in kartListe)
            {
                var araListeOperasyon = _db.StokKartOperasyons
                      .Where(a => a.StokKartId == potaKart.Id && a.PlanlananTarih >=
                          baslangic && a.PlanlananTarih < bitis).OrderBy(a => a.PlanlananTarih)
                      .ToList();
                operasyonlar.AddRange(araListeOperasyon);

                var araListeDuruslar = _db.StokKartDurus
                    .Where(a => a.StokKartId == potaKart.Id && a.DurusBaslangic >=
                        baslangic && a.DurusBaslangic < bitis).OrderBy(a => a.DurusBaslangic)
                    .ToList();
                duruslar.AddRange(araListeDuruslar);
            }

           // model.ArrTable = new int[kartListe.Count, header.Count, 2, operasyonlar.Count];

            int i = -1;



            foreach (var potaKart in kartListe)
            {
                i++;
                int j = -1;
                var kartOperasyonlari = operasyonlar.Where(a => a.StokKartId == potaKart.Id).ToList();
                var kartDuruslari = duruslar.Where(a => a.StokKartId == potaKart.Id).ToList();
                foreach (var dropItem in header)
                {
                    j++;

                    int t = -1;

                    var lstOperasyon = kartOperasyonlari.Where(a =>
                        a.PlanlananTarih >= dropItem.DateTime &&
                        a.PlanlananTarih < dropItem.DateTime2).ToList();
                    var lstDuruslar = kartDuruslari.Where(a =>
                        a.DurusBaslangic >= dropItem.DateTime &&
                        a.DurusBaslangic < dropItem.DateTime2).ToList();

                    /*
                    foreach (var operasyon in lstOperasyon)
                    {
                        t++;
                        model.ArrTable[i, j, 0, t] = operasyon.Id;
                    }
                    foreach (var durus in lstDuruslar)
                    {
                        t++;
                        model.ArrTable[i, j, 1, t] = durus.Id;
                    }
                    */
                }
            }
            /*
            model.PotaKarts = kartListe;
            model.StokKartOperasyons = operasyonlar;
            model.KomponentTalimatGrups = _db.KomponentTalimatGrups.ToList();
            model.TableHeaderlar = header;
            model.PeriyotTanims = _db.PeriyotTanims.ToList();

            */
            return model;
        }

        public ActionResult Secim()
        {

            var dropList = new List<DropItem>();
            var itemValues = new List<ItemValue>();

            var squery =
                "SELECT DISTINCT TOP (100) PERCENT dbo.PeriyotTanim.PeriyotTipi, dbo.KomponentTalimatGrup.Id, dbo.PeriyotTipiTanim.PeriyotTipiAdi, dbo.KomponentTalimatGrup.KomponentTalimatGrupAdi FROM dbo.PeriyotTipiTanim INNER JOIN dbo.PeriyotTanim ON dbo.PeriyotTipiTanim.Id = dbo.PeriyotTanim.PeriyotTipi INNER JOIN dbo.KomponentTalimatGrup ON dbo.PeriyotTanim.Id = dbo.KomponentTalimatGrup.PeriyotTanimId INNER JOIN dbo.StokKartOperasyon ON dbo.KomponentTalimatGrup.Id = dbo.StokKartOperasyon.KomponentTalimatGrupId ORDER BY dbo.PeriyotTipiTanim.PeriyotTipiAdi, dbo.KomponentTalimatGrup.KomponentTalimatGrupAdi";
            return View();
        }
    }
}