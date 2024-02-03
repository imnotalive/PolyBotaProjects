using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PolyBota.Entities;
using PolyBota.Web.Areas.Takip.Models;
using PolyBota.Web.Controllers;

namespace PolyBota.Web.Areas.Takip.Controllers
{
    public class PeriyodikTakvimController : BaseController
    {
        // GET: Takip/PeriyodikTakvim
        public ActionResult BakimListe()
        {
            var model = new TakvimBakimModel()
            {
                SecilenYil = DateTime.Now.Year
            };
            var baslangicTarihi = new DateTime(model.SecilenYil, 1, 1);
            var bitisTarihi = baslangicTarihi.AddYears(1).AddDays(-6);

            var basHafta = SecilenHaftaNoGetir(baslangicTarihi);
            var bitHafta = SecilenHaftaNoGetir(bitisTarihi);
            model.BaslangicHafta = basHafta;
            model.BitisHafta = bitHafta;

            for (int i = model.SecilenYil - 50; i < model.SecilenYil + 5; i++)
            {
                model.Yillar.Add(i);
            }

            var bolumKirilimlar = DropBolumKirilimlariGetir();

            foreach (var dropItem in bolumKirilimlar)
            {
                var makineler = _db.StokKarts.Where(a => a.BolumId == dropItem.IdInt).OrderBy(a => a.StokTanimAdi)
                    .ToList();

                if (makineler.Any())
                {
                    dropItem.ItemValues = makineler.Select(a => new ItemValue()
                    {
                        Id = a.Id.ToString(),
                        IdInt = a.Id,
                        Text = a.StokTanimAdi
                    }).ToList();
                }
                model.BolumStoklar.Add(dropItem);
            }

            var periyotTipiTanims = _db.PeriyotTipiTanims.OrderBy(a => a.PeriyotTipiAdi).ToList();

            var periyotTanims = _db.PeriyotTanims.OrderBy(a => a.PeriyotTanimAdi).ToList();

            foreach (var periyotTipi in periyotTipiTanims)
            {
                var drop = new DropItem()
                {
                    Id = periyotTipi.Id.ToString(),
                    Tanim = periyotTipi.PeriyotTipiAdi
                };

                var lst = new List<ItemValue>();

                foreach (var periyotTanim in periyotTanims.Where(a => a.PeriyotTipi == periyotTipi.Id).ToList())
                {
                    lst.Add(new ItemValue()
                    {
                        Id = periyotTanim.Id.ToString(),
                        IdInt = periyotTanim.Id,
                        SeciliMi = true,
                        Text = string.Format("{0} - ({1} Hafta)", periyotTanim.PeriyotTanimAdi, periyotTanim.PeriyotDonemi)
                    });
                }

                drop.ItemValues = lst.OrderBy(a => a.Text).ToList();

                model.DropPeriyotlar.Add(drop);
            }

            return View(model);
        }

        [HttpPost]
        public PartialViewResult BakimAnaliz(TakvimBakimModel model)
        {
            model.StokKartOperasyonDurumTanims = _db.StokKartOperasyonDurumTanims.ToList();
            var yil = model.SecilenYil;
            var basHafta = model.BaslangicHafta;
            var bitHafta = model.BitisHafta;

            var basTarih = HaftadanGunGetir(yil, basHafta, true);
            var bitTarih = HaftadanGunGetir(yil, bitHafta, false);

            var operasyonlar = _db.StokKartOperasyons
                .Where(a => a.PlanlananTarih >= basTarih && a.PlanlananTarih <= bitTarih).OrderBy(a => a.PlanlananTarih)
                .ToList();

            model.StokKartOperasyons = operasyonlar;

            var kompGrupIdler = operasyonlar.Select(a => a.KomponentTalimatGrupId).Distinct().ToList();

            var stokIdler = operasyonlar.Select(a => a.StokKartId).Distinct().ToList();

            foreach (var i in kompGrupIdler)
            {
                if (_db.KomponentTalimatGrups.Any(a=>a.Id==i))
                {
                    model.KomponentTalimatGrups.Add(_db.KomponentTalimatGrups.Find(i));
                }
             
            }
            foreach (var i in stokIdler)
            {
                if (_db.StokKarts.Any(a => a.Id == i))
                {
                    model.StokKarts.Add(_db.StokKarts.Find(i));
                }

            }

            var sutunSayisi = 0;
            var satirSayisi = 0;
            var z = 1;

            var haftaListe = new List<int>();

            for (var i = model.BaslangicHafta; i <= model.BitisHafta; i++)
            {
                sutunSayisi++;
                haftaListe.Add(i);
                var lst = operasyonlar.Count(a => a.PlanlananHafta == i);
                if (lst >= satirSayisi)
                {
                    satirSayisi = lst;
                }

            }


            model.TabloArray = new int[satirSayisi, sutunSayisi, z];

            for (int i = 0; i < sutunSayisi; i++)
            {
                var hafta = haftaListe[i];

                var opLst = operasyonlar.Where(a => a.PlanlananHafta == hafta).ToList();

                int p = -1;

                foreach (var oper in opLst)
                {
                    p++;
                    model.TabloArray[p, i, 0] = oper.Id;
                }

            }

           


            return PartialView("_BakimAnaliz", model);
        }
    }
}