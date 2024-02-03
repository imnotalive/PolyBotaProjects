using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using PolyBota.BLL;
using PolyBota.DAL;
using PolyBota.Entities;
using PolyBota.Helpers;
using PolyBota.Web.Areas.Ariza.Models;
using PolyBota.Web.Controllers;

namespace PolyBota.Web.Areas.Ariza.Controllers
{
    public class ArizaKayitController : BaseController
    {
        // GET: Ariza/ArizaKayit

        #region Aktif Kullanılan
        public ActionResult ArizaTechizatListe()
        {

            var model = new ArizaKayitModel()
            {
               // ArizaTechizatGrubuTanims = _db.ArizaTechizatGrubuTanims.OrderBy(a => a.ArizaTechizatGrubuAdi).ToList()
            };

            var squery = string.Format(
                "SELECT DISTINCT TOP (100) PERCENT dbo.ArizaTechizatGrubuTanim.Id, dbo.ArizaTechizatGrubuTanim.ArizaTechizatGrubuAdi FROM            dbo.ArizaTanimArizaTechizatGrubu INNER JOIN                         dbo.ArizaTechizatGrubuTanim ON dbo.ArizaTanimArizaTechizatGrubu.ArizaTechizatGrubuId = dbo.ArizaTechizatGrubuTanim.Id INNER JOIN                         dbo.ArizaAcilabilecekDepartman ON dbo.ArizaTanimArizaTechizatGrubu.ArizaId = dbo.ArizaAcilabilecekDepartman.ArizaTanimId INNER JOIN                         dbo.UserDepartman ON dbo.ArizaAcilabilecekDepartman.DepartmanId = dbo.UserDepartman.DepartmanId WHERE        (dbo.UserDepartman.UserId = {0}) ORDER BY dbo.ArizaTechizatGrubuTanim.ArizaTechizatGrubuAdi",
                User.Id);

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();
                var ArizaTechizatGrubuId = (int) lst[0];
                var ArizaTechizatGrubuAdi = lst[1]?.ToString();
                model.ArizaTechizatGrubuTanims.Add(new ArizaTechizatGrubuTanim()
                {
                    ArizaTechizatGrubuAdi = ArizaTechizatGrubuAdi,
                    Id = ArizaTechizatGrubuId
                });


            }
            return View(model);
        }

        #region Arıza Kayıt Sıra
        // Adım-1
        public ActionResult ArizaTechizataGoreArizaSecimi(int id)
        {
            var model = new ArizaKayitModel()
            {
                ArizaTechizatGrubuTanim = _db.ArizaTechizatGrubuTanims.Find(id),

            };

            // arıza tanımı  olan pota kartlar

            var squery = string.Format(
                "SELECT DISTINCT TOP (100) PERCENT dbo.PotaKart.Id, dbo.PotaKart.PotaKartAdi, dbo.PotaKart.PotaKodu FROM            dbo.ArizaTanim INNER JOIN                         dbo.ArizaTanimArizaTechizatGrubu ON dbo.ArizaTanim.Id = dbo.ArizaTanimArizaTechizatGrubu.ArizaId INNER JOIN                         dbo.ArizaTechizatGrubuTanim ON dbo.ArizaTanimArizaTechizatGrubu.ArizaTechizatGrubuId = dbo.ArizaTechizatGrubuTanim.Id INNER JOIN                         dbo.ArizaTanimGerekliTechizatTuru ON dbo.ArizaTanimArizaTechizatGrubu.Id = dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimArizaTechizatGrupId AND                          dbo.ArizaTanim.Id = dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimId INNER JOIN                         dbo.PotaKart ON dbo.ArizaTanimGerekliTechizatTuru.TechizatTuruId = dbo.PotaKart.TechizatTuruId WHERE        (dbo.ArizaTanimArizaTechizatGrubu.ArizaTechizatGrubuId = {0}) ORDER BY dbo.PotaKart.PotaKartAdi",
                id);

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql());

            foreach (var aa in result)
            {

                var lst = aa.Values.ToList();
                var potaKartId = (int)lst[0];
                var potaKartAdi = lst[1].ToString();
                var potaKodu = lst[2]?.ToString();


                model.PotaKarts.Add(new PotaKart()
                {
                    Id = potaKartId,
                    PotaKartAdi = string.IsNullOrEmpty(potaKodu) ? potaKartAdi : string.Format("{0} ({1})", potaKartAdi, potaKodu),
                    PotaKodu = potaKodu
                });

            }

            model.PotaKarts.Add(new PotaKart()
            {
                Id = 0,
                PotaKartAdi = "Seçim Yapınız"
            });

            return View(model);
        }


        // Adım-2 
        public PartialViewResult PkSecimineGoreArizaGetir(int id, int arizaTechizatGrubuId)
        {
            var model = new ArizaKayitModel()
            {
                PotaKartId = id,
                ArizaTechizatGrubuId = arizaTechizatGrubuId
            };
            /*
             arizaTechizatGrubuId ve potakartId si olan arıza tanımlar
             */

            var squery = string.Format(
                "SELECT DISTINCT TOP (100) PERCENT dbo.PotaKart.Id, dbo.ArizaTanimArizaTechizatGrubu.ArizaId, dbo.ArizaTanim.ArizaTanimAdi, dbo.ArizaTechizatGrubuTanim.ArizaTechizatGrubuAdi FROM            dbo.ArizaTanim INNER JOIN                         dbo.ArizaTanimArizaTechizatGrubu ON dbo.ArizaTanim.Id = dbo.ArizaTanimArizaTechizatGrubu.ArizaId INNER JOIN                         dbo.ArizaTechizatGrubuTanim ON dbo.ArizaTanimArizaTechizatGrubu.ArizaTechizatGrubuId = dbo.ArizaTechizatGrubuTanim.Id INNER JOIN                         dbo.ArizaTanimGerekliTechizatTuru ON dbo.ArizaTanimArizaTechizatGrubu.Id = dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimArizaTechizatGrupId AND                          dbo.ArizaTanim.Id = dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimId INNER JOIN                         dbo.PotaKart ON dbo.ArizaTanimGerekliTechizatTuru.TechizatTuruId = dbo.PotaKart.TechizatTuruId WHERE        (dbo.ArizaTanimArizaTechizatGrubu.ArizaTechizatGrubuId = {0}) AND (dbo.PotaKart.Id = {1})",
                arizaTechizatGrubuId, id);

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql());

            foreach (var aa in result)
            {

                var lst = aa.Values.ToList();

                var potaKartId = lst[0].ToString();
                var ArizaTanimId = (int)lst[1];
                var ArizaTanimAdi = lst[2]?.ToString();
                var ArizaTechizatGrubuAdi = lst[3]?.ToString();



                model.ArizaTanims.Add(new ArizaTanim()
                {
                    Id = ArizaTanimId,
                    ArizaTanimAdi = ArizaTechizatGrubuAdi + " >> " + ArizaTanimAdi
                });

            }
            model.ArizaTanims.Add(new ArizaTanim()
            {
                Id = 0,
                ArizaTanimAdi = "Arıza Seçiniz"
            });
            //  model = ArizaTanimaGoreArizaKayitModeliGetir(model);
            return PartialView("_PkSecimineGoreArizaGetir", model);
        }

        //Adım-3
        public PartialViewResult PkveArizayaGoreKayitHazirlik(int id, int arizaTechizatGrubuId, int arizaId)
        {
            var model = new ArizaKayitModel()
            {
                PotaKartId = id,
                ArizaTechizatGrubuId = arizaTechizatGrubuId,
                ArizaTanimId = arizaId,
                ArizaTanim = _db.ArizaTanims.Find(arizaId)
            };

            var referansPotaKart = _db.PotaKarts.Find(id);
            var olasiEntegreKartlar = new List<PotaKart>();
            #region Entegre Kartlar
            var entegreKartIdler = new List<int>();
            var entegreOlabilecekKartlar = _db.PotaKartEntegreKarts
                .Where(a => a.PotaKartId == referansPotaKart.Id || a.EntegrePotaKartId == referansPotaKart.Id).ToList();

            foreach (var item in entegreOlabilecekKartlar)
            {

                var olasiKartId = 0;
                if (item.PotaKartId == referansPotaKart.Id)
                {
                    olasiKartId = item.EntegrePotaKartId;
                }
                if (item.EntegrePotaKartId == referansPotaKart.Id)
                {
                    olasiKartId = item.PotaKartId;
                }

                if (olasiKartId != 0)
                {
                    if (!entegreKartIdler.Any(a => a == olasiKartId))
                    {
                        entegreKartIdler.Add(olasiKartId);
                    }
                }

            }

            foreach (var i in entegreKartIdler)
            {
                if (_db.PotaKarts.Any(a=>a.Id==i))
                {
                    olasiEntegreKartlar.Add(_db.PotaKarts.Find(i));
                }
            
            }
            #endregion

            var dropArizaTanimArizaTechizatGruplar = new List<DropItem>();
            var ItemValuesTechizatTurleri = new List<ItemValue>();
            var dropSecilenArizaTanimArizaTechizatGrup = new DropItem();
            #region arızayi açabilmek için gerekli ArizaTanimArizaTechizatGrupId ve TechizatTuruId
            var squery = string.Format(
                "SELECT  dbo.ArizaTanimGerekliTechizatTuru.TechizatTuruId,                          dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimArizaTechizatGrupId FROM            dbo.ArizaTanim INNER JOIN                         dbo.ArizaTanimArizaTechizatGrubu ON dbo.ArizaTanim.Id = dbo.ArizaTanimArizaTechizatGrubu.ArizaId INNER JOIN                         dbo.ArizaTechizatGrubuTanim ON dbo.ArizaTanimArizaTechizatGrubu.ArizaTechizatGrubuId = dbo.ArizaTechizatGrubuTanim.Id INNER JOIN                         dbo.ArizaTanimGerekliTechizatTuru ON dbo.ArizaTanimArizaTechizatGrubu.Id = dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimArizaTechizatGrupId AND                          dbo.ArizaTanim.Id = dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimId INNER JOIN                         dbo.TechizatTuruTanim ON dbo.ArizaTanimGerekliTechizatTuru.TechizatTuruId = dbo.TechizatTuruTanim.Id WHERE        (dbo.ArizaTanimArizaTechizatGrubu.ArizaId = {0}) AND (dbo.ArizaTanimArizaTechizatGrubu.ArizaTechizatGrubuId = {1})",
                arizaId, arizaTechizatGrubuId);
            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql());


            foreach (var aa in result)
            {

                var lst = aa.Values.ToList();


                var TechizatTuruId = (int)lst[0];
                var ArizaTanimArizaTechizatGrupId = (int)lst[1];
                ItemValuesTechizatTurleri.Add(new ItemValue()
                {
                    IdInt = TechizatTuruId,
                    IdInt2 = ArizaTanimArizaTechizatGrupId
                });

                if (dropArizaTanimArizaTechizatGruplar.Count(a => a.IdInt == ArizaTanimArizaTechizatGrupId) == 0)
                {
                    dropArizaTanimArizaTechizatGruplar.Add(new DropItem()
                    {
                        IdInt = ArizaTanimArizaTechizatGrupId
                    });
                }
            }

            foreach (var dropItem in dropArizaTanimArizaTechizatGruplar)
            {
                dropItem.ItemValues = ItemValuesTechizatTurleri.Where(a => a.IdInt2 == dropItem.IdInt).ToList();
            }

            foreach (var dropItem in dropArizaTanimArizaTechizatGruplar)
            {
                if (dropSecilenArizaTanimArizaTechizatGrup.IdInt != 0)
                {
                    break;
                }
                foreach (var itemValue in dropItem.ItemValues)
                {
                    if (itemValue.IdInt == referansPotaKart.TechizatTuruId)
                    {
                        dropSecilenArizaTanimArizaTechizatGrup = dropItem;
                        break;
                    }
                }
            }



            #endregion


            var ArizaTanimArizaTechizatGrubuId = dropSecilenArizaTanimArizaTechizatGrup.IdInt;
            var dropGerekliTechizatlarGruplar = DropArizaTanimArizaTechizatGruplariGetir(ArizaTanimArizaTechizatGrubuId);

            var bilesenHeaders = _db.BilesenHeaders.AsNoTracking().ToList();
            var bilesenItems = _db.BilesenItems.AsNoTracking().ToList();
            int tt = -1;
            foreach (var dropTechizat in dropGerekliTechizatlarGruplar)
            {

                tt++;
                var araModel = new ArizaPotaKartSecimModel()
                {
                    TechizatTuruTanim = new TechizatTuruTanim() { TechizatTuruTanimAdi = dropTechizat.Tanim, Id = dropTechizat.IdInt },
                    BilesenVarMi = dropTechizat.ItemValues.Any(),
                    i = tt,
                    ArizaTanimArizaTechizatGrubuId = ArizaTanimArizaTechizatGrubuId
                };

                if (referansPotaKart.TechizatTuruId == dropTechizat.IdInt)
                {
                    araModel.PotaKarts.Add(referansPotaKart);
                    araModel.PotaKartId = referansPotaKart.Id;
                    if (araModel.BilesenVarMi)
                    {
                        var potaKartBilesenModels = new List<PotaKartBilesenModel>();
                        var potaKartBilesenItems = _db.PotaKartBilesenItems.Where(a => a.PotaKartId == referansPotaKart.Id)
                            .ToList();

                        foreach (var bilesenHeader in dropTechizat.ItemValues)
                        {
                            if (bilesenHeader.IdInt != 0)
                            {

                                var bilesenItemsByHeader = bilesenItems
                                    .Where(a => a.BilesenHeaderId == bilesenHeader.IdInt)
                                    .OrderBy(a => a.BilesenDegeri).ToList();
                                var dropBilesenItems = new List<DropItem>();
                                dropBilesenItems.Add(new DropItem() { Id = "0", Tanim = "Seçiniz" });
                                foreach (var ii in bilesenItemsByHeader)
                                {
                                    if (potaKartBilesenItems.Any(a => a.BilesenItemId == ii.Id))
                                    {
                                        dropBilesenItems.Add(new DropItem()
                                        {
                                            Id = ii.Id.ToString(),
                                            Tanim = ii.BilesenDegeri
                                        });
                                    }
                                }

                                var ss = new PotaKartBilesenModel()
                                {
                                    BilesenHeader = bilesenHeaders.First(a => a.Id == bilesenHeader.IdInt),
                                    DropPotaKartBilesenItems = dropBilesenItems

                                };

                                potaKartBilesenModels.Add(ss);
                            }

                        }


                        araModel.PotaKartBilesenModels = potaKartBilesenModels;
                    }
                }
                else
                {
                    var araListe = new List<PotaKart>();
                    araListe.Add(new PotaKart() { Id = 0, PotaKartAdi = "Seçiniz" });
                    araListe.AddRange(olasiEntegreKartlar.Where(a => a.TechizatTuruId == dropTechizat.IdInt).ToList());
                    araModel.PotaKarts.AddRange(araListe);

                }
                model.ArizaPotaKartSecimModels.Add(araModel);
            }

            return PartialView("_PkveArizayaGoreKayit", model);
        }

        // Adım-3.1 yardımcı metot
        public List<DropItem> DropArizaTanimArizaTechizatGruplariGetir(int ArizaTanimArizaTechizatGrubuId)
        {
            var dropGerekliTechizatlarGruplar = new List<DropItem>();
            var itemValuesGerekliBilesenHeaderlar = new List<ItemValue>();

            #region ArizaTanimArizaTechizatGrup için gerekli techizatTuru ve Bilesen Headerlar

            // ArizaTanimArizaTechizatGrup için gerekli techizatTuru ve Bilesen Headerlar
            var squery = string.Format(
                  "SELECT        TOP (100) PERCENT dbo.ArizaTanimGerekliTechizatTuru.TechizatTuruId, ISNULL(dbo.ArizaTanimGerekliBilesenHeader.BilesenHeaderId, 0) AS BilesenHeaderId, dbo.BilesenHeader.BilesenHeaderAdi,                          dbo.TechizatTuruTanim.TechizatTuruTanimAdi FROM            dbo.TechizatTuruTanim INNER JOIN                         dbo.ArizaTanimArizaTechizatGrubu INNER JOIN                         dbo.ArizaTanimGerekliTechizatTuru ON dbo.ArizaTanimArizaTechizatGrubu.Id = dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimArizaTechizatGrupId AND                          dbo.ArizaTanimArizaTechizatGrubu.ArizaId = dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimId ON dbo.TechizatTuruTanim.Id = dbo.ArizaTanimGerekliTechizatTuru.TechizatTuruId LEFT OUTER JOIN                         dbo.BilesenHeader INNER JOIN                         dbo.ArizaTanimGerekliBilesenHeader ON dbo.BilesenHeader.Id = dbo.ArizaTanimGerekliBilesenHeader.BilesenHeaderId ON                          dbo.ArizaTanimGerekliTechizatTuru.TechizatTuruId = dbo.ArizaTanimGerekliBilesenHeader.TechizatTuruId AND                          dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimArizaTechizatGrupId = dbo.ArizaTanimGerekliBilesenHeader.ArizaTanimArizaTechizatGrupId AND                          dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimId = dbo.ArizaTanimGerekliBilesenHeader.ArizaTanimId WHERE        (dbo.ArizaTanimArizaTechizatGrubu.Id = {0}) ORDER BY dbo.BilesenHeader.BilesenHeaderAdi",
                  ArizaTanimArizaTechizatGrubuId);

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();

                var TechizatTuruId = (int)lst[0];
                var BilesenHeaderId = (int)lst[1];
                var BilesenHeaderAdi = lst[2]?.ToString();
                var TechizatTuruTanimAdi = lst[3]?.ToString();
                if (dropGerekliTechizatlarGruplar.Count(a => a.IdInt == TechizatTuruId) == 0)
                {
                    dropGerekliTechizatlarGruplar.Add(new DropItem()
                    {
                        IdInt = TechizatTuruId,
                        Tanim = TechizatTuruTanimAdi
                    });
                }

                if (BilesenHeaderId != 0)
                {
                    itemValuesGerekliBilesenHeaderlar.Add(new ItemValue()
                    {
                        IdInt = BilesenHeaderId,
                        IdInt2 = TechizatTuruId,
                        Text = BilesenHeaderAdi
                    });
                }
            }


            foreach (var item in dropGerekliTechizatlarGruplar)
            {
                item.ItemValues = itemValuesGerekliBilesenHeaderlar.Where(a => a.IdInt2 == item.IdInt)
                    .ToList();
            }

            #endregion

            return dropGerekliTechizatlarGruplar;
        }

        #endregion






        public PartialViewResult PotaKartaGoreBilesenItemGetir(int id, int arzId, int indexId)
        {
            //arzId =ArizaTanimArizaTechizatGrubuId 

            var model = new ArizaKayitModel()
            {
                indexId = indexId
            };

            for (int i = 0; i <= indexId; i++)
            {
                model.ArizaPotaKartSecimModels.Add(new ArizaPotaKartSecimModel());
            }


            var bilesenHeaders = _db.BilesenHeaders.AsNoTracking().ToList();
            var bilesenItems = _db.BilesenItems.AsNoTracking().ToList();

            var dropGerekliTechizatlarGruplar = DropArizaTanimArizaTechizatGruplariGetir(arzId);

            var potaKart = _db.PotaKarts.Find(id);

            var araModel = new ArizaPotaKartSecimModel()
            {
                i = indexId,
                PotaKartId = id,
                ArizaTanimArizaTechizatGrubuId = arzId
            };

            foreach (var dropTechizat in dropGerekliTechizatlarGruplar)
            {

                if (potaKart.TechizatTuruId == dropTechizat.IdInt)
                {
                    araModel.BilesenVarMi = dropTechizat.ItemValues.Any();
                    if (araModel.BilesenVarMi)
                    {
                        var potaKartBilesenModels = new List<PotaKartBilesenModel>();
                        var potaKartBilesenItems = _db.PotaKartBilesenItems.Where(a => a.PotaKartId == potaKart.Id)
                            .ToList();

                        foreach (var bilesenHeader in dropTechizat.ItemValues)
                        {
                            if (bilesenHeader.IdInt != 0)
                            {

                                var bilesenItemsByHeader = bilesenItems
                                    .Where(a => a.BilesenHeaderId == bilesenHeader.IdInt)
                                    .OrderBy(a => a.BilesenDegeri).ToList();
                                var dropBilesenItems = new List<DropItem>();
                                dropBilesenItems.Add(new DropItem() { Id = "0", Tanim = "Seçiniz" });
                                foreach (var ii in bilesenItemsByHeader)
                                {
                                    if (potaKartBilesenItems.Any(a => a.BilesenItemId == ii.Id))
                                    {
                                        dropBilesenItems.Add(new DropItem()
                                        {
                                            Id = ii.Id.ToString(),
                                            Tanim = ii.BilesenDegeri
                                        });
                                    }
                                }

                                var ss = new PotaKartBilesenModel()
                                {
                                    BilesenHeader = bilesenHeaders.First(a => a.Id == bilesenHeader.IdInt),
                                    DropPotaKartBilesenItems = dropBilesenItems

                                };

                                potaKartBilesenModels.Add(ss);
                            }

                        }


                        araModel.PotaKartBilesenModels = potaKartBilesenModels;
                    }
                }


            }

            model.ArizaPotaKartSecimModels[indexId] = araModel;
            return PartialView("_PkveArizayaGoreKayitItem", model);
            /*
            var model = new ArizaKayitModel()
            {
                indexId = indexId
            };

            for (int i = 0; i <= indexId; i++)
            {
                model.ArizaPotaKartSecimModels.Add(new ArizaPotaKartSecimModel());
            }



            if (id != 0)
            {

                bool BilesenHeaderGerekiyor = _db.ArizaTanimGerekliBilesenHeaders.Any(a => a.ArizaTanimId == arizaId);

                var bilesenHeaders = new List<BilesenHeader>();
                var dropPotaKartBilesenItems = new List<DropItem>();
                if (BilesenHeaderGerekiyor)
                {
                    // bileşen header gerekiyor
                    var squery = string.Format(
                 "SELECT DISTINCT dbo.ArizaTechizatItemTanim.TechizatTuruId, dbo.ArizaTanim.ArizaTechizatGrubuId, dbo.ArizaTanimGerekliBilesenHeader.BilesenHeaderId, dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimId, dbo.PotaKartBilesenItems.PotaKartId, dbo.TechizatTuruTanim.TechizatTuruTanimAdi, dbo.ArizaTechizatGrubuTanim.ArizaTechizatGrubuAdi, dbo.ArizaTanim.ArizaTanimAdi, dbo.BilesenHeader.BilesenHeaderAdi, dbo.PotaKart.PotaKartAdi,dbo.PotaKartBilesenItems.BilesenItemId, dbo.BilesenItem.BilesenDegeri  FROM            dbo.ArizaTanim INNER JOIN dbo.ArizaTanimGerekliBilesenHeader ON dbo.ArizaTanim.Id = dbo.ArizaTanimGerekliBilesenHeader.ArizaTanimId INNER JOIN dbo.ArizaTanimGerekliTechizatTuru ON dbo.ArizaTanimGerekliBilesenHeader.TechizatTuruId = dbo.ArizaTanimGerekliTechizatTuru.TechizatTuruId AND dbo.ArizaTanim.Id = dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimId INNER JOIN dbo.ArizaTechizatGrubuTanim ON dbo.ArizaTanim.ArizaTechizatGrubuId = dbo.ArizaTechizatGrubuTanim.Id INNER JOIN dbo.ArizaTechizatItemTanim ON dbo.ArizaTechizatGrubuTanim.Id = dbo.ArizaTechizatItemTanim.ArizaTechizatGrubuId INNER JOIN dbo.TechizatTuruTanim ON dbo.ArizaTechizatItemTanim.TechizatTuruId = dbo.TechizatTuruTanim.Id INNER JOIN dbo.BilesenHeader ON dbo.ArizaTanimGerekliBilesenHeader.BilesenHeaderId = dbo.BilesenHeader.Id INNER JOIN dbo.PotaKart ON dbo.ArizaTechizatItemTanim.TechizatTuruId = dbo.PotaKart.TechizatTuruId INNER JOIN dbo.BilesenItem ON dbo.ArizaTanimGerekliBilesenHeader.BilesenHeaderId = dbo.BilesenItem.BilesenHeaderId INNER JOIN dbo.PotaKartBilesenItems ON dbo.PotaKart.Id = dbo.PotaKartBilesenItems.PotaKartId AND dbo.BilesenItem.Id = dbo.PotaKartBilesenItems.BilesenItemId WHERE(dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimId = {0}) AND (dbo.PotaKartBilesenItems.PotaKartId = {1})", arizaId, id);

                    var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

                    foreach (var aa in result)
                    {
                        var lst = aa.Values.ToList();
                        var TechizatTuruId = (int)lst[0];
                        var ArizaTechizatGrubuId = (int)lst[1];
                        var BilesenHeaderId = (int)lst[2];
                        var ArizaTanimId = (int)lst[3];
                        var PotaKartId = (int)lst[4];
                        var TechizatTuruTanimAdi = lst[5].ToString();
                        var ArizaTechizatGrubuAdi = lst[6].ToString();
                        var ArizaTanimAdi = lst[7].ToString();
                        var BilesenHeaderAdi = lst[8].ToString();
                        var PotaKartAdi = lst[9].ToString();

                        var potaKartBilesenItemId = (int)lst[10];
                        var bilesenDegeri = lst[11].ToString();

                        dropPotaKartBilesenItems.Add(new DropItem()
                        {
                            Id = potaKartBilesenItemId.ToString(),
                            IdInt = potaKartBilesenItemId,
                            Tanim = bilesenDegeri,
                            IdInt2 = BilesenHeaderId
                        });

                        if (bilesenHeaders.Count(a => a.Id == BilesenHeaderId) == 0)
                        {
                            bilesenHeaders.Add(new BilesenHeader()
                            {
                                Id = BilesenHeaderId,
                                BilesenHeaderAdi = BilesenHeaderAdi
                            });
                        }
                    }
                }
                else
                {
                    // sadece makine seçilecek
                }

                foreach (var bilesenHeader in bilesenHeaders)
                {
                    var araModel = new PotaKartBilesenModel()
                    {
                        BilesenHeader = bilesenHeader,
                        DropPotaKartBilesenItems =
                            dropPotaKartBilesenItems.Where(a => a.IdInt2 == bilesenHeader.Id).ToList()
                    };
                    araModel.DropPotaKartBilesenItems.Add(new DropItem()
                    {
                        Id = "0",
                        IdInt = 0,
                        Tanim = "Seçiniz"
                    });
                    model.ArizaPotaKartSecimModels[indexId].PotaKartBilesenModels.Add(araModel);
                }

            }
            */



        }


        public PartialViewResult PkveArizayaGoreKayit(int id, int arizaTechizatGrubuId, int arizaId)
        {
            var model = new ArizaKayitModel()
            {
                PotaKartId = id,
                ArizaTechizatGrubuId = arizaTechizatGrubuId,
                ArizaTanimId = arizaId
            };

            var arizaTanim = _db.ArizaTanims.Find(arizaId);
            var arizaTechizatGrubu = _db.ArizaTechizatGrubuTanims.Find(arizaTechizatGrubuId);
            var potaKart = _db.PotaKarts.Find(id);

            // ariza için gereken TechizatTuruIdler
            var squery = string.Format(
                "SELECT        dbo.ArizaTanimGerekliTechizatTuru.TechizatTuruId FROM dbo.ArizaTanim INNER JOIN dbo.ArizaTanimGerekliTechizatTuru ON dbo.ArizaTanim.Id = dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimId INNER JOIN dbo.TechizatTuruTanim ON dbo.ArizaTanimGerekliTechizatTuru.TechizatTuruId = dbo.TechizatTuruTanim.Id WHERE        (dbo.ArizaTanim.Id ={0})",
                arizaId);
            var gereklitechizatTuruTanims = new List<TechizatTuruTanim>();
            var gerekliArizaBilesenHeaders = new List<ArizaTanimGerekliBilesenHeader>();
            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql());

            foreach (var aa in result)
            {

                var lst = aa.Values.ToList();
                var TechizatTuruId = (int)lst[0];
                gereklitechizatTuruTanims.Add(_db.TechizatTuruTanims.Find(TechizatTuruId));
            }

            squery = string.Format(
                "SELECT        dbo.ArizaTanimGerekliTechizatTuru.TechizatTuruId, dbo.ArizaTanimGerekliBilesenHeader.BilesenHeaderId FROM            dbo.ArizaTanim INNER JOIN                         dbo.ArizaTanimGerekliTechizatTuru ON dbo.ArizaTanim.Id = dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimId INNER JOIN                         dbo.TechizatTuruTanim ON dbo.ArizaTanimGerekliTechizatTuru.TechizatTuruId = dbo.TechizatTuruTanim.Id INNER JOIN                         dbo.ArizaTanimGerekliBilesenHeader ON dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimId = dbo.ArizaTanimGerekliBilesenHeader.ArizaTanimId AND                          dbo.ArizaTanimGerekliTechizatTuru.TechizatTuruId = dbo.ArizaTanimGerekliBilesenHeader.TechizatTuruId WHERE        (dbo.ArizaTanim.Id = {0})", arizaId);


            result = BllMssql.CustomSql(squery, SuaHelper.defaultSql());

            foreach (var aa in result)
            {

                var lst = aa.Values.ToList();
                var TechizatTuruId = (int)lst[0];
                var BilesenHeaderId = (int)lst[1];
                gerekliArizaBilesenHeaders.Add(new ArizaTanimGerekliBilesenHeader()
                {
                    BilesenHeaderId = BilesenHeaderId,
                    TechizatTuruId = TechizatTuruId
                });
            }


            var entegreOlabilecekKartlar = _db.PotaKartEntegreKarts
                .Where(a => a.PotaKartId == potaKart.Id || a.EntegrePotaKartId == potaKart.Id).ToList();


            var entegreKartIdler = new List<int>();

            foreach (var item in entegreOlabilecekKartlar)
            {

                var olasiKartId = 0;
                if (item.PotaKartId == potaKart.Id)
                {
                    olasiKartId = item.EntegrePotaKartId;
                }
                if (item.EntegrePotaKartId == potaKart.Id)
                {
                    olasiKartId = item.PotaKartId;
                }

                if (olasiKartId != 0)
                {
                    if (!entegreKartIdler.Any(a => a == olasiKartId))
                    {
                        entegreKartIdler.Add(olasiKartId);
                    }
                }

            }

            var bilesenHeaders = _db.BilesenHeaders.AsNoTracking().ToList();
            var bilesenItems = _db.BilesenItems.AsNoTracking().ToList();

            var olasiEntegreKartlar = _db.PotaKarts.Where(a => entegreKartIdler.Any(b => b == a.Id)).ToList();

            foreach (var item in gereklitechizatTuruTanims.OrderBy(a => a.TechizatTuruTanimAdi).ToList())
            {
                var araModel = new ArizaPotaKartSecimModel()
                {
                    TechizatTuruTanim = item,
                    BilesenVarMi = gerekliArizaBilesenHeaders.Any(a => a.TechizatTuruId == item.Id)
                };

                if (potaKart.TechizatTuruId == item.Id)
                {
                    araModel.PotaKarts.Add(potaKart);
                    araModel.PotaKartId = potaKart.Id;
                    if (araModel.BilesenVarMi)
                    {
                        var araListe = gerekliArizaBilesenHeaders.Where(a => a.TechizatTuruId == item.Id).ToList();
                        var potaKartBilesenModels = new List<PotaKartBilesenModel>();

                        var potaKartBilesenItems = _db.PotaKartBilesenItems.Where(a => a.PotaKartId == potaKart.Id)
                            .ToList();

                        foreach (var bilesenHeader in araListe)
                        {
                            if (bilesenHeaders.Any(a => a.Id == bilesenHeader.BilesenHeaderId))
                            {

                                var bilesenItemsByHeader = bilesenItems
                                    .Where(a => a.BilesenHeaderId == bilesenHeader.BilesenHeaderId)
                                    .OrderBy(a => a.BilesenDegeri).ToList();
                                var dropBilesenItems = new List<DropItem>();
                                dropBilesenItems.Add(new DropItem() { Id = "0", Tanim = "Seçiniz" });
                                foreach (var ii in bilesenItemsByHeader)
                                {
                                    if (potaKartBilesenItems.Any(a => a.BilesenItemId == ii.Id))
                                    {
                                        dropBilesenItems.Add(new DropItem()
                                        {
                                            Id = ii.Id.ToString(),
                                            Tanim = ii.BilesenDegeri
                                        });
                                    }
                                }

                                var ss = new PotaKartBilesenModel()
                                {
                                    BilesenHeader = bilesenHeaders.First(a => a.Id == bilesenHeader.BilesenHeaderId),
                                    DropPotaKartBilesenItems = dropBilesenItems

                                };

                                potaKartBilesenModels.Add(ss);
                            }

                        }


                        araModel.PotaKartBilesenModels = potaKartBilesenModels;
                    }
                }
                else
                {
                    var araListe = new List<PotaKart>();
                    araListe.Add(new PotaKart() { Id = 0, PotaKartAdi = "Seçiniz" });
                    araListe.AddRange(olasiEntegreKartlar.Where(a => a.TechizatTuruId == item.Id).ToList());
                    araModel.PotaKarts.AddRange(araListe);

                }
                model.ArizaPotaKartSecimModels.Add(araModel);
            }

            model.ArizaTanim = arizaTanim;

            return PartialView("_PkveArizayaGoreKayit", model);
        }
        #endregion







        public ArizaKayitModel ArizaTanimaGoreArizaKayitModeliGetir(ArizaKayitModel model)
        {
            var gerekliTechizatTanimlar = new List<TechizatTuruTanim>();
            var sadeceTechizatGerekenListe = new List<TechizatTuruTanim>();
            var techizatBilesenGerekenListe = new List<TechizatTuruTanim>();
            var gerekliArizaBilesenHeaderlar = new List<ArizaTanimGerekliBilesenHeader>();
            var uygunPotakartLar = new List<PotaKart>();
            var dropPotaKartlar = new List<DropItem>();
            var gerekliBilesenHeaderTanimlar = new List<BilesenHeader>();


            #region gerekli Techizat Tanim Liste
            var squery = string.Format(
                "SELECT DISTINCT dbo.ArizaTanimGerekliTechizatTuru.TechizatTuruId, dbo.TechizatTuruTanim.TechizatTuruTanimAdi FROM dbo.ArizaTanim INNER JOIN dbo.ArizaTanimGerekliTechizatTuru ON dbo.ArizaTanim.Id = dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimId INNER JOIN  dbo.ArizaTechizatGrubuTanim ON dbo.ArizaTanim.ArizaTechizatGrubuId = dbo.ArizaTechizatGrubuTanim.Id INNER JOIN  dbo.TechizatTuruTanim ON dbo.ArizaTanimGerekliTechizatTuru.TechizatTuruId = dbo.TechizatTuruTanim.Id WHERE(dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimId = {0}) ORDER BY dbo.TechizatTuruTanim.TechizatTuruTanimAdi",
                model.ArizaTanimId);


            var resultTechizatTanim = BllMssql.CustomSql(squery, SuaHelper.defaultSql());


            foreach (var aa in resultTechizatTanim)
            {
                var lst = aa.Values.ToList();
                gerekliTechizatTanimlar.Add(new TechizatTuruTanim()
                {
                    Id = (int)lst[0],
                    TechizatTuruTanimAdi = lst[1].ToString()
                });
            }


            #endregion





            foreach (var item in gerekliTechizatTanimlar)
            {
                if (_db.ArizaTanimGerekliBilesenHeaders.Any(a => a.ArizaTanimId == model.ArizaTanimId && a.TechizatTuruId == item.Id))
                {
                    techizatBilesenGerekenListe.Add(item);
                    gerekliArizaBilesenHeaderlar.AddRange(_db.ArizaTanimGerekliBilesenHeaders.Where(a => a.ArizaTanimId == model.ArizaTanimId && a.TechizatTuruId == item.Id));
                }
                else
                {
                    sadeceTechizatGerekenListe.Add(item);
                    uygunPotakartLar.AddRange(_db.PotaKarts.Where(a => a.TechizatTuruId == item.Id));
                }
            }

            var bilesenHeaderItemIdler = gerekliArizaBilesenHeaderlar.Select(a => a.BilesenHeaderId).Distinct().ToList();

            gerekliBilesenHeaderTanimlar = _db.BilesenHeaders.Where(a => bilesenHeaderItemIdler.Any(b => b == a.Id)).ToList();



            foreach (var item in gerekliArizaBilesenHeaderlar)
            {

                // gereken bileşen header için uygun pota kartları getiriyor, (Foreignk Key ArizaTanimGerekliBilesenHeader.Id)
                var ss = string.Format(
                    "SELECT DISTINCT dbo.PotaKart.Id, dbo.PotaKart.PotaKartAdi FROM dbo.BilesenHeader INNER JOIN dbo.ArizaTanimGerekliBilesenHeader ON dbo.BilesenHeader.Id = dbo.ArizaTanimGerekliBilesenHeader.BilesenHeaderId INNER JOIN dbo.PotaKart ON dbo.ArizaTanimGerekliBilesenHeader.TechizatTuruId = dbo.PotaKart.TechizatTuruId INNER JOIN dbo.PotaKartBilesenItems ON dbo.PotaKart.Id = dbo.PotaKartBilesenItems.PotaKartId INNER JOIN dbo.BilesenItem ON dbo.ArizaTanimGerekliBilesenHeader.BilesenHeaderId = dbo.BilesenItem.BilesenHeaderId AND dbo.PotaKartBilesenItems.BilesenItemId = dbo.BilesenItem.Id WHERE(dbo.ArizaTanimGerekliBilesenHeader.Id = {0})",
                    item.Id);

                var resultItem = BllMssql.CustomSql(ss, SuaHelper.defaultSql());
                foreach (var aa in resultItem)
                {
                    var lst = aa.Values.ToList();

                    dropPotaKartlar.Add(new DropItem()
                    {
                        IdInt = (int)lst[0],
                        Tanim = lst[1].ToString(),
                        IdInt2 = item.Id
                    });
                }

            }







            foreach (var techizatTuruTanim in gerekliTechizatTanimlar)
            {
                var potaKarts = new List<PotaKart>();
                potaKarts.Add(new PotaKart() { Id = 0, PotaKartAdi = "Seçiniz" });
                var BilesenVarMi = false;
                if (sadeceTechizatGerekenListe.Any(a => a.Id == techizatTuruTanim.Id))
                {
                    potaKarts.AddRange(uygunPotakartLar.Where(a => a.TechizatTuruId == techizatTuruTanim.Id).ToList());
                }
                else
                {
                    if (gerekliArizaBilesenHeaderlar.Any(a => a.TechizatTuruId == techizatTuruTanim.Id))
                    {
                        BilesenVarMi = true;
                        foreach (var ii in gerekliArizaBilesenHeaderlar.Where(a => a.TechizatTuruId == techizatTuruTanim.Id).ToList())
                        {
                            var drops = dropPotaKartlar.Where(a => a.IdInt2 == ii.Id).ToList();

                            foreach (var dropItem in drops)
                            {
                                if (potaKarts.Count(a => a.Id == dropItem.IdInt) == 0)
                                {
                                    potaKarts.Add(new PotaKart()
                                    {
                                        Id = dropItem.IdInt,
                                        PotaKartAdi = dropItem.Tanim,
                                        TechizatTuruId = techizatTuruTanim.Id
                                    });
                                }
                            }
                        }
                    }
                }
                model.ArizaPotaKartSecimModels.Add(new ArizaPotaKartSecimModel()
                {
                    PotaKarts = potaKarts,
                    TechizatTuruTanim = techizatTuruTanim,
                    BilesenVarMi = BilesenVarMi
                });

            }

            return model;
        }
        public PartialViewResult ArizaSecimineGorePotaKartlariGetir(int id)
        {
            var model = new ArizaKayitModel()
            {
                ArizaTanimId = id
            };
            model = ArizaTanimaGoreArizaKayitModeliGetir(model);
            return PartialView("_ArizaSecimineGorePotaKartlariGetir", model);
        }




        public ActionResult ArizaTechizataGoreSecim(int id)
        {
            var model = new ArizaKayitModel()
            {
                ArizaTechizatGrubuTanim = _db.ArizaTechizatGrubuTanims.Find(id)
            };

            var gerekliArizaTechizatItems = _db.ArizaTechizatItemTanims.Where(a => a.ArizaTechizatGrubuId == id).ToList();


            foreach (var item in gerekliArizaTechizatItems)
            {
                if (model.TechizatTuruTanims.Count(a => a.Id == item.TechizatTuruId) == 0)
                {
                    model.TechizatTuruTanims.Add(_db.TechizatTuruTanims.Find(item.TechizatTuruId));
                }
            }

            model.TechizatTuruTanims = model.TechizatTuruTanims.OrderBy(a => a.TechizatTuruTanimAdi).ToList();


            foreach (var item in model.TechizatTuruTanims)
            {
                var araModel = new ArizaPotaKartSecimModel()
                {
                    TechizatTuruTanim = item,
                    PotaKarts = _db.PotaKarts.Where(a => a.TechizatTuruId == item.Id).ToList()
                };
                araModel.PotaKarts.Add(new PotaKart() { Id = 0, PotaKartAdi = "Seçim Yapınız" });
                model.ArizaPotaKartSecimModels.Add(araModel);
            }
            return View(model);
        }

      

        [HttpPost]
        public PartialViewResult ArizaSecimiGetir(ArizaKayitModel model)
        {
            model.indexId = 1; // sorun yok

            var arizaPotaKartSecimModels = model.ArizaPotaKartSecimModels.ToList();

            if (arizaPotaKartSecimModels.Count(a => a.PotaKartId == 0) > 0)
            {
                model.indexId = 0;
            }
            else
            {
                foreach (var item in arizaPotaKartSecimModels)
                {
                    if (item.PotaKartBilesenModels.Any(a => a.SecilenBilesenItemId == 0))
                    {
                        model.indexId = 0;
                        break;
                    }
                }
            }


            return PartialView("_ArizaSecimiGetir", model);
        }


        [HttpPost]

        public JsonResult ArizaKaydet(int id, ArizaKayitModel model)
        {
            int state = 0; // 0- bilgiler eksik, 1- kayıt okey

            string icon = "";
            string title = "";
            int arizaDurumu = 0;

            var liste = model.ArizaPotaKartSecimModels.ToList();


            bool SorunVarmi = liste.Any(a => a.PotaKartId == 0);

            if (!SorunVarmi)
            {
                foreach (var item in liste)
                {

                    if (item.BilesenVarMi)
                    {
                        SorunVarmi = item.PotaKartBilesenModels.Any(a => a.SecilenBilesenItemId == 0);
                    }

                    if (SorunVarmi)
                    {
                        break;
                    }
                }
            }

            if (SorunVarmi)
            {
                icon = "warning";
                title = "bilgileri kontrol ediniz";
            }
            else
            {

                state = 1;

                var arizaHeader = new ArizaBildirimHeader()
                {
                    KayitYapan = User.Id,
                    KayitTarihi = DateTime.Now,
                    AcilanArizaTanimId = model.ArizaTanimId,
                    ArizaDurumu = 1,
                    KapananArizaTanimId = model.ArizaTanimId, // default açılan arıza doğru açılmıştır
                    AcilanArizaNotu = model.ArizaNotu
                };
               

                var items = new List<ArizaBildirimItem>();

                foreach (var item in liste)
                {

                    if (item.BilesenVarMi)
                    {
                        foreach (var itemPotaKartBilesenModel in item.PotaKartBilesenModels)
                        {

                            var secilenBilesenItem =
                                _db.BilesenItems.Find(itemPotaKartBilesenModel.SecilenBilesenItemId);

                            var yeniArizaItemAcilan = new ArizaBildirimItem()
                            {
                                TechizatTuruId = item.TechizatTuruTanim.Id,
                                PotaKartId = item.PotaKartId,
                                ArizaBildirimHeaderId = arizaHeader.Id,
                                BilesenHeaderId = secilenBilesenItem.BilesenHeaderId,
                                BilesenItemId = secilenBilesenItem.Id,
                                AcilanArizaTanimId = arizaHeader.AcilanArizaTanimId,
                                KapananArizaTanimId = 0
                            };
                            var yeniArizaItemAcilanKapanan = new ArizaBildirimItem()
                            {
                                TechizatTuruId = item.TechizatTuruTanim.Id,
                                PotaKartId = item.PotaKartId,
                                ArizaBildirimHeaderId = arizaHeader.Id,
                                BilesenHeaderId = secilenBilesenItem.BilesenHeaderId,
                                BilesenItemId = secilenBilesenItem.Id,
                                AcilanArizaTanimId = 0,
                                KapananArizaTanimId = arizaHeader.AcilanArizaTanimId
                            };
                            items.Add(yeniArizaItemAcilan);
                            items.Add(yeniArizaItemAcilanKapanan);

                        }
                    }
                    else
                    {
                        var yeniArizaItemAcilan = new ArizaBildirimItem()
                        {
                            TechizatTuruId = item.TechizatTuruTanim.Id,
                            PotaKartId = item.PotaKartId,
                            ArizaBildirimHeaderId = arizaHeader.Id,
                            AcilanArizaTanimId = arizaHeader.AcilanArizaTanimId
                        };
                        var yeniArizaItemAcilanKapanan = new ArizaBildirimItem()
                        {
                            TechizatTuruId = item.TechizatTuruTanim.Id,
                            PotaKartId = item.PotaKartId,
                            ArizaBildirimHeaderId = arizaHeader.Id,
                            KapananArizaTanimId = arizaHeader.AcilanArizaTanimId
                        };
                        items.Add(yeniArizaItemAcilan);
                        items.Add(yeniArizaItemAcilanKapanan);
                        
                    }
                }

                bool mukerrerMi = false;

                var benzerHeaderlar = _db.ArizaBildirimHeaders.Where(a =>
                    a.ArizaDurumu != 5 && a.AcilanArizaTanimId == arizaHeader.AcilanArizaTanimId);



                if (benzerHeaderlar.Any())
                {
                    foreach (var arizaBildirimHeader in benzerHeaderlar)
                    {
                        var bildirimItems = _db.ArizaBildirimItems.Where(a => a.ArizaBildirimHeaderId == arizaBildirimHeader.Id)
                            .ToList();

                        foreach (var arizaBildirimItem in bildirimItems)
                        {
                            foreach (var guncelItem in items)
                            {
                                if (guncelItem.PotaKartId==arizaBildirimItem.PotaKartId && guncelItem.BilesenHeaderId==arizaBildirimItem.BilesenHeaderId && guncelItem.BilesenItemId==arizaBildirimItem.BilesenItemId)
                                {
                                    mukerrerMi = true;

                                }
                                if (mukerrerMi) break;
                            }
                            if (mukerrerMi) break;
                        }
                        if (mukerrerMi) break;
                    }
                }

                if (mukerrerMi)
                {
                    icon = "warning";
                    title = "Bu Arıza daha önce açıldı ve henüz kapanmadı";
                    state = 0;
                }
                else
                {
                    arizaHeader.ArizaTechizatGrubuId = id;

                    state = model.YonlendirmeDurumu + 1;
                    _db.ArizaBildirimHeaders.Add(arizaHeader);
                    _db.SaveChanges();

                    foreach (var arizaBildirimItem in items)
                    {
                        arizaBildirimItem.ArizaBildirimHeaderId = arizaHeader.Id;

                    }
                    _db.ArizaBildirimItems.AddRange(items);
                    _db.SaveChanges();
                }
              

                TempDataCreate(1);
            }

            return new JsonResult { Data = new { Id = arizaDurumu, state = state, icon = icon, title = title }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

      
    }
}