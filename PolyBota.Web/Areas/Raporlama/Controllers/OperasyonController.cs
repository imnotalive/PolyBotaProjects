using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PolyBota.BLL;
using PolyBota.DAL;
using PolyBota.Entities;
using PolyBota.Helpers;
using PolyBota.Web.Areas.Raporlama.Models;
using PolyBota.Web.Controllers;

namespace PolyBota.Web.Areas.Raporlama.Controllers
{
    public class OperasyonController : BaseController
    {
        // GET: Raporlama/Operasyon

        public RaporlamaOperasyonModel RaporlamaOperasyonModeliAnalizYap(RaporlamaOperasyonModel model)
        {
            var baslangic = model.BaslangisTarihi.Date;
            var bitis = model.BitisTarihi.AddDays(1).Date;

            var seciliKompTalimatIdler = new List<int>();
            var potaKartIdler = new List<int>();
            var seciliKompTalimatlar = new List<KomponentTalimatGrup>();

            var OperasyonTamamlandi = false;
            var OperasyonGecikti = false;
            var OperasyonBekleniyor = false;

            foreach (var dropItem in model.DropOperasyonDurumlari.Where(a=>a.SeciliMi).ToList())
            {

                /*
                    new DropItem(){Id = "1", SeciliMi = true, Tanim = "Operasyon Tamamlandı", Tanim2 = "alert-success"},
                new DropItem(){Id = "0", SeciliMi = true, Tanim = "Operasyon Tarihi Henüz Gelmedi", Tanim2 = "alert-secondary"},
                new DropItem(){Id = "2", SeciliMi = true, Tanim = "Operasyon Gecikti", Tanim2 = "alert-danger"},
                 */
                if (dropItem.Id=="1")
                {
                    OperasyonTamamlandi = true;
                }
                if (dropItem.Id == "0")
                {
                    OperasyonBekleniyor = true;
                }
                if (dropItem.Id == "2")
                {
                    OperasyonGecikti = true;
                }
            }

            var bugn = DateTime.Now.Date;
            foreach (var item in model.PeriyotTanimKomponentTalimatGrupModels)
            {
                foreach (var itt in item.DropPeriyotTanimKompGrups)
                {
                    var lst = itt.ItemValues.Where(a => a.SeciliMi).Select(a => a.Id).ToList();

                    foreach (var i in lst)
                    {
                        seciliKompTalimatIdler.Add(Convert.ToInt32(i));
                    }
                }
            }

            var operasyonlar = new List<StokKartOperasyon>();

            foreach (var i in seciliKompTalimatIdler)
            {
                seciliKompTalimatlar.Add(_db.KomponentTalimatGrups.Find(i));

                var lst = _db.StokKartOperasyons.Where(a => a.KomponentTalimatGrupId == i && a.PlanlananTarih >=
                    baslangic && a.PlanlananTarih < bitis).ToList();


                var filtreliListe = new List<StokKartOperasyon>();

                foreach (var ii in lst)
                {
                    if (ii.OperasyonDurumu==1)
                    {
                        if (OperasyonTamamlandi)
                        {
                            filtreliListe.Add(ii);
                        }
                    }
                    else
                    {
                        if (ii.PlanlananTarih.Date > bugn)
                        {
                            // tarih gelmedi
                            if (OperasyonBekleniyor)
                            {
                                filtreliListe.Add(ii);
                            }
                        }
                        else
                        {
                            // gecikti
                            if (OperasyonGecikti)
                            {
                                filtreliListe.Add(ii);
                            }
                        }
                    }
                }

                operasyonlar.AddRange(filtreliListe);
            }

            potaKartIdler = operasyonlar.Select(a => a.StokKartId).Distinct().ToList();


           
            var header = DropTakvimTableHeader(model.BaslangisTarihi, model.BitisTarihi, model.GosterimSekli);

            model.ArrTable = new int[header.Count, operasyonlar.Count, 1];

            int x = -1;
          
           
            foreach (var dropItem in header)
            {
                x++;
                int y = -1;
                var lstOperasyon = operasyonlar.Where(a =>
                    a.PlanlananTarih >= dropItem.DateTime &&
                    a.PlanlananTarih < dropItem.DateTime2).OrderBy(a=>a.PlanlananTarih).ToList();

                foreach (var operasyon in lstOperasyon)
                {
                    y++;
                    model.ArrTable[x, y, 0] = operasyon.Id;
                }
            }
           
            model.TableHeaderlar = header;
            model.KartOperasyons = operasyonlar;
            model.KomponentTalimatGrups = seciliKompTalimatlar;
            foreach (var i in potaKartIdler)
            {
                if (_db.PotaKarts.Any(a=>a.Id==i))
                {
                    model.PotaKarts.Add(_db.PotaKarts.Find(i));
                }
             
            }
            return model;
        }
        public ActionResult PlanlananListe()
        {
            var squery =
                "SELECT DISTINCT dbo.PeriyotTanim.PeriyotTipi, dbo.KomponentTalimatGrup.PeriyotTanimId, dbo.StokKartOperasyon.KomponentTalimatGrupId, dbo.PeriyotTipiTanim.PeriyotTipiAdi, dbo.PeriyotTanim.PeriyotTanimAdi,  dbo.KomponentTalimatGrup.KomponentTalimatGrupAdi FROM            dbo.PeriyotTipiTanim INNER JOIN dbo.PeriyotTanim ON dbo.PeriyotTipiTanim.Id = dbo.PeriyotTanim.PeriyotTipi INNER JOIN dbo.KomponentTalimatGrup ON dbo.PeriyotTanim.Id = dbo.KomponentTalimatGrup.PeriyotTanimId INNER JOIN dbo.StokKartOperasyon ON dbo.KomponentTalimatGrup.Id = dbo.StokKartOperasyon.KomponentTalimatGrupId INNER JOIN dbo.PotaKart ON dbo.StokKartOperasyon.StokKartId = dbo.PotaKart.Id ORDER BY dbo.PeriyotTipiTanim.PeriyotTipiAdi, dbo.PeriyotTanim.PeriyotTanimAdi, dbo.KomponentTalimatGrup.KomponentTalimatGrupAdi";

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

            var PeriyotTipiTanims = new List<PeriyotTipiTanim>();

            var dropList = new List<DropItem>();
            var itemValues = new List<ItemValue>();
            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();

                var PeriyotTipi = (int)lst[0];
                var PeriyotTanimId = (int)lst[1];
                var KomponentTalimatGrupId = (int)lst[2];
                var PeriyotTipiAdi = lst[3].ToString();
                var PeriyotTanimAdi = lst[4].ToString();
                var KomponentTalimatGrupAdi = lst[5].ToString();

                if (PeriyotTipiTanims.Count(a => a.Id == PeriyotTipi) == 0)
                {
                    PeriyotTipiTanims.Add(new PeriyotTipiTanim()
                    {
                        PeriyotTipiAdi = PeriyotTipiAdi,
                        Id = PeriyotTipi
                    });

                }

                if (dropList.Count(a => a.IdInt == PeriyotTanimId) == 0)
                {
                    dropList.Add(new DropItem()
                    {
                        IdInt = PeriyotTanimId,
                        Tanim = PeriyotTanimAdi,
                        IdInt2 = PeriyotTipi
                    });
                }
                itemValues.Add(new ItemValue()
                {
                    Id = KomponentTalimatGrupId.ToString(),
                    Text = KomponentTalimatGrupAdi,
                    IdInt = PeriyotTanimId,
                    SeciliMi = true
                });

            }

            var araModel = new List<PeriyotTanimKomponentTalimatGrupModel>();

            foreach (var item in PeriyotTipiTanims)
            {
                var araItem = new PeriyotTanimKomponentTalimatGrupModel()
                {
                    PeriyotTipiTanim = item,
                    DropPeriyotTanimKompGrups = dropList.Where(a => a.IdInt2 == item.Id).ToList()
                };
                foreach (var itt in araItem.DropPeriyotTanimKompGrups)
                {
                    itt.ItemValues = itemValues.Where(a => a.IdInt == itt.IdInt).ToList();
                }
                araModel.Add(araItem);
            }
            var model = new RaporlamaOperasyonModel()
            {
                PeriyotTanimKomponentTalimatGrupModels = araModel
            };
            model = RaporlamaOperasyonModeliAnalizYap(model);
            return View(model);
        }

        [HttpPost]
        public PartialViewResult PlanlananListeAnalizSonuc(RaporlamaOperasyonModel model)
        {
            return PartialView("_PlanlananAnalizSonuc", RaporlamaOperasyonModeliAnalizYap(model));
        }
    }
}