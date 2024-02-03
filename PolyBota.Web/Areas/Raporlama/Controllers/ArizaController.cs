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
    public class ArizaController : BaseController
    {
        // GET: Raporlama/Ariza


        public List<DropItem> DropSorumluDepartmanlariGetir()
        {
            var squery = "SELECT DISTINCT TOP (100) PERCENT dbo.ArizaSorumluDepartman.DepartmanId, dbo.Departman.DepartmanAdi FROM            dbo.ArizaSorumluDepartman INNER JOIN                         dbo.Departman ON dbo.ArizaSorumluDepartman.DepartmanId = dbo.Departman.Id ORDER BY dbo.Departman.DepartmanAdi";


            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

            var drops = new List<DropItem>();

            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();

                drops.Add(new DropItem()
                {
                    IdInt = (int)lst[0],
                    Tanim = lst[1]?.ToString(),
                    SeciliMi = true
                });
            }

            return drops;
        }

        #region MakineArizaSonuclari

        public ActionResult MakineArizaSonuclari()
        {
            var dbCon = SuaHelper.defaultSql();

            var model = new RaporlamaArizaModel()
            {
                BaslangicTarihi = new DateTime(DateTime.Now.Year, 1, 1),
                DropArizaDurumus = _db.ArizaDurumuTanims.ToList()
                    .Select(a => new DropItem() { IdInt = a.Id, Tanim = a.ArizaDurumuAdi, SeciliMi = true }).ToList(),
                DropSorumluDepartmans = DropSorumluDepartmanlariGetir()
            };

            var squery =
                           "SELECT DISTINCT TOP (100) PERCENT dbo.PotaKart.PotaKartAdi, dbo.ArizaBildirimItem.PotaKartId FROM            dbo.ArizaBildirimItem INNER JOIN                          dbo.PotaKart ON dbo.ArizaBildirimItem.PotaKartId = dbo.PotaKart.Id ORDER BY dbo.PotaKart.PotaKartAdi";

            var result = BllMssql.CustomSql(squery, dbCon).ToList();

            model.DropItems.Add(new DropItem()
            {
                IdInt = 0,
                Tanim = "Seçiniz"

            });
            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();
                var PotaKartAdi = lst[0]?.ToString();
                var PotaKartId = lst[1]?.ToString();
                model.DropItems.Add(new DropItem()
                {
                    Id = PotaKartId,
                    Tanim = PotaKartAdi
                });
            }

            squery =
                "SELECT DISTINCT TOP (100) PERCENT dbo.ArizaBildirimHeader.ArizaTechizatGrubuId, dbo.ArizaTechizatGrubuTanim.ArizaTechizatGrubuAdi FROM            dbo.ArizaBildirimHeader INNER JOIN                         dbo.ArizaTechizatGrubuTanim ON dbo.ArizaBildirimHeader.ArizaTechizatGrubuId = dbo.ArizaTechizatGrubuTanim.Id ORDER BY dbo.ArizaTechizatGrubuTanim.ArizaTechizatGrubuAdi";

            result = BllMssql.CustomSql(squery, dbCon).ToList();

            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();

                var ArizaTechizatGrubuId = (int)lst[0];
                var ArizaTechizatGrubuAdi = lst[1]?.ToString();

                model.DropArizaTechizatGruplar.Add(new DropItem()
                {
                    IdInt = ArizaTechizatGrubuId,
                    Tanim = ArizaTechizatGrubuAdi,
                    SeciliMi = true
                });
            }



            return View(model);
        }

        public PartialViewResult MakineArizaPkyaGoreAnalizHazirla(int id)
        {

            var model = new RaporlamaArizaModel();

            var squery = string.Format(
                "SELECT DISTINCT TOP (100) PERCENT dbo.ArizaBildirimItem.BilesenHeaderId, dbo.BilesenHeader.BilesenHeaderAdi, dbo.ArizaBildirimItem.BilesenItemId, dbo.BilesenItem.BilesenDegeri FROM            dbo.ArizaBildirimItem INNER JOIN                         dbo.PotaKart ON dbo.ArizaBildirimItem.PotaKartId = dbo.PotaKart.Id LEFT OUTER JOIN                         dbo.BilesenItem ON dbo.ArizaBildirimItem.BilesenItemId = dbo.BilesenItem.Id LEFT OUTER JOIN                         dbo.BilesenHeader ON dbo.ArizaBildirimItem.BilesenHeaderId = dbo.BilesenHeader.Id WHERE        (dbo.ArizaBildirimItem.PotaKartId = {0}) ORDER BY dbo.BilesenHeader.BilesenHeaderAdi, dbo.BilesenItem.BilesenDegeri", id);

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

            var distHeaderlar = new List<BilesenHeader>();
            var distBilesenItems = new List<BilesenItem>();

            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();
                var BilesenHeaderId = (int)lst[0];
                var BilesenHeaderAdi = lst[1]?.ToString();
                var BilesenItemId = (int)lst[2];
                var BilesenDegeri = lst[3]?.ToString();
                distBilesenItems.Add(new BilesenItem()
                {
                    BilesenDegeri = BilesenDegeri ?? "Seçimsiz",
                    BilesenHeaderId = BilesenHeaderId,
                    Id = BilesenItemId
                });

                if (distHeaderlar.Count(a => a.Id == BilesenHeaderId) == 0)
                {

                    string bilesenHeaderAdi = BilesenHeaderId == 0 ? "Seçimsiz" : BilesenHeaderAdi;
                    distHeaderlar.Add(new BilesenHeader()
                    {
                        BilesenHeaderAdi = bilesenHeaderAdi,
                        Id = BilesenHeaderId
                    });
                }
            }

            var drops = new List<DropItem>();
            foreach (var bilesenHeader in distHeaderlar.OrderBy(a => a.BilesenHeaderAdi).ToList())
            {
                drops.Add(new DropItem()
                {
                    IdInt = bilesenHeader.Id,
                    Tanim = bilesenHeader.BilesenHeaderAdi,
                    ItemValues = distBilesenItems.Where(a => a.BilesenHeaderId == bilesenHeader.Id).Select(b => new ItemValue() { IdInt = b.Id, Text = b.BilesenDegeri }).OrderBy(a => a.Text).ToList()
                });
            }

            model.RaporlamaMakineBazliAnalizPk = new RaporlamaMakineBazliAnalizPK()
            {
                PotaKart = new PotaKart() { Id = id },
                DropBilesenHeaderItems = drops
            };
            return PartialView("_MakineArizaPkyaGoreAnalizHazirla", model);
        }

        [HttpPost]
        public PartialViewResult MakineArizaPkyaGoreAnalizYap(RaporlamaArizaModel model)
        {

            var analizItem = model.RaporlamaMakineBazliAnalizPk;
            var basTarihi = tarihiOlustur(model.BaslangicTarihi);
            var bitTarihi = tarihiOlustur(model.BitisTarihi.AddDays(1));
            var potakart = analizItem.PotaKart;

            var secilibilesenHeaders = analizItem.DropBilesenHeaderItems.Where(a => a.SeciliMi).ToList();


            var seciliArizaDurumIds = model.DropArizaDurumus.Where(a => a.SeciliMi).ToList().Select(a => a.IdInt).ToList();
            var seciliSorumluDeptIds = model.DropSorumluDepartmans.Where(a => a.SeciliMi).ToList().Select(a => a.IdInt).ToList();

            // Uygun arizaBildirimHeaderId Bulunur
            var uygunHeaderIds = new List<int>();
            #region Uygun arizaBildirimHeaderIds
            var squery = string.Format(
                         "SELECT        KayitTarihi, ArizaBildirimHeaderId, PotaKartId, AcilanArizaTanimId, BilesenHeaderId, BilesenItemId, DepartmanId, ArizaDurumu FROM            (SELECT        TOP (100) PERCENT dbo.ArizaBildirimHeader.KayitTarihi, dbo.ArizaBildirimItem.ArizaBildirimHeaderId, dbo.ArizaBildirimItem.PotaKartId, dbo.ArizaBildirimHeader.AcilanArizaTanimId,                                                     dbo.ArizaBildirimItem.BilesenHeaderId, dbo.ArizaBildirimItem.BilesenItemId, dbo.ArizaSorumluDepartman.DepartmanId, dbo.ArizaBildirimHeader.ArizaDurumu                          FROM            dbo.ArizaBildirimHeader INNER JOIN                                                    dbo.ArizaBildirimItem ON dbo.ArizaBildirimHeader.Id = dbo.ArizaBildirimItem.ArizaBildirimHeaderId INNER JOIN                                                    dbo.ArizaSorumluDepartman ON dbo.ArizaBildirimHeader.AcilanArizaTanimId = dbo.ArizaSorumluDepartman.ArizaTanimId                          WHERE        (dbo.ArizaBildirimHeader.KayitTarihi >= CONVERT(DATETIME, '{0}', 102)) AND (dbo.ArizaBildirimHeader.KayitTarihi < CONVERT(DATETIME, '{1}', 102)) AND                                                     (dbo.ArizaBildirimItem.PotaKartId = {2}) ORDER BY dbo.ArizaBildirimHeader.KayitTarihi DESC) AS tbl",
                         basTarihi, bitTarihi, potakart.Id);


            squery += " ORDER BY KayitTarihi DESC";
            var dbCon = SuaHelper.defaultSql();
            var result = BllMssql.CustomSql(squery, dbCon).ToList();


            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();

                var kayitTarihi = lst[0]?.ToString();
                var ArizaBildirimHeaderId = (int)lst[1];
                var PotaKartId = lst[2]?.ToString();
                var AcilanArizaTanimId = lst[3]?.ToString();
                var BilesenHeaderId = (int)lst[4];
                var BilesenItemId = (int)lst[5];
                var DepartmanId = (int)lst[6];
                var ArizaDurumu = (int)lst[7];
                bool kayitYapilabilirmi = false;


                if (!secilibilesenHeaders.Any())
                {
                    kayitYapilabilirmi = true;
                }
                else
                {
                    if (secilibilesenHeaders.Any(a=>a.IdInt== BilesenHeaderId))
                    {
                        var dropItem = secilibilesenHeaders.First(a => a.IdInt == BilesenHeaderId);

                        kayitYapilabilirmi = dropItem.IntList.Any(b => b == BilesenItemId);
                    }
                }

                if (kayitYapilabilirmi)
                {
                    kayitYapilabilirmi = seciliArizaDurumIds.Any(b => b == ArizaDurumu);
                }
                if (kayitYapilabilirmi)
                {
                    kayitYapilabilirmi = seciliSorumluDeptIds.Any(b => b == DepartmanId);
                }

                if (kayitYapilabilirmi)
                {
                    if (uygunHeaderIds.Count(a => a == ArizaBildirimHeaderId) == 0)
                    {
                        uygunHeaderIds.Add(ArizaBildirimHeaderId);
                    }
                }
                


                
            }
            #endregion

            var items = new List<RaporlamaArizaModelItem>();

            foreach (var uygunHeaderId in uygunHeaderIds)
            {
                squery = string.Format(
                    "SELECT DISTINCT  dbo.ArizaBildirimHeader.ArizaTechizatGrubuId, dbo.ArizaBildirimHeader.Id, dbo.ArizaSorumluDepartman.DepartmanId, dbo.ArizaTechizatGrubuTanim.ArizaTechizatGrubuAdi,                          dbo.ArizaBildirimHeader.KayitTarihi, dbo.ArizaTanim.ArizaTanimAdi, ArizaTanim_1.ArizaTanimAdi AS KapananAriza, dbo.Departman.DepartmanAdi, dbo.[User].Name, dbo.ArizaBildirimHeader.ArizaDurumu,                          dbo.ArizaDurumuTanim.ArizaDurumuAdi FROM            dbo.ArizaBildirimHeader INNER JOIN                         dbo.ArizaTechizatGrubuTanim ON dbo.ArizaBildirimHeader.ArizaTechizatGrubuId = dbo.ArizaTechizatGrubuTanim.Id INNER JOIN                         dbo.ArizaTanim ON dbo.ArizaBildirimHeader.AcilanArizaTanimId = dbo.ArizaTanim.Id INNER JOIN                         dbo.ArizaTanim AS ArizaTanim_1 ON dbo.ArizaBildirimHeader.KapananArizaTanimId = ArizaTanim_1.Id INNER JOIN                         dbo.ArizaSorumluDepartman ON ArizaTanim_1.Id = dbo.ArizaSorumluDepartman.ArizaTanimId INNER JOIN                         dbo.Departman ON dbo.ArizaSorumluDepartman.DepartmanId = dbo.Departman.Id INNER JOIN                         dbo.[User] ON dbo.ArizaBildirimHeader.KayitYapan = dbo.[User].Id INNER JOIN                         dbo.ArizaDurumuTanim ON dbo.ArizaBildirimHeader.ArizaDurumu = dbo.ArizaDurumuTanim.Id WHERE        (dbo.ArizaBildirimHeader.Id = {0})",
                    uygunHeaderId);

                result = BllMssql.CustomSql(squery, dbCon).ToList();

                foreach (var aa in result)
                {
                    var lst = aa.Values.ToList();

                    var ArizaTechizatGrubuId = (int)lst[0];
                    var Id = (int)lst[1];
                    var DepartmanId = (int)lst[2];
                    var ArizaTechizatGrubuAdi = lst[3]?.ToString();
                    var KayitTarihi = (DateTime)lst[4];
                    var ArizaTanimAdi = lst[5]?.ToString();
                    var KapananAriza = lst[6]?.ToString();
                    var DepartmanAdi = lst[7]?.ToString();
                    var Name = lst[8]?.ToString();
                    var ArizaDurumu = (int)lst[9];
                    var ArizaDurumuAdi = lst[10]?.ToString();
                    if (items.Any(a => a.Id == Id))
                    {
                        var item = items.First(a => a.Id == Id);
                        item.DepartmanAdi += " - " + DepartmanAdi;
                    }
                    else
                    {
                        items.Add(new RaporlamaArizaModelItem()
                        {
                            Id = Id,
                            ArizaTechizatGrubuId = ArizaTechizatGrubuId,
                            AcilanArizaTanimAdi = ArizaTanimAdi,
                            ArizaTechizatGrubuAdi = ArizaTechizatGrubuAdi,
                            DepartmanAdi = DepartmanAdi,
                            DepartmanId = DepartmanId,
                            KapananArizaTanimAdi = KapananAriza,
                            KayitTarihi = KayitTarihi,
                            KayitYapanKulAdi = Name,
                            ArizaDurumu = ArizaDurumu,
                            ArizaDurumuAdi = ArizaDurumuAdi
                        });
                    }


                }
            }

            foreach (var item in items)
            {
                squery = string.Format(
                    "SELECT        dbo.ArizaBildirimItem.BilesenHeaderId, dbo.ArizaBildirimItem.BilesenItemId, dbo.ArizaBildirimItem.PotaKartId, dbo.PotaKart.PotaKartAdi, dbo.BilesenHeader.BilesenHeaderAdi, dbo.BilesenItem.BilesenDegeri FROM            dbo.ArizaBildirimHeader INNER JOIN                         dbo.ArizaBildirimItem ON dbo.ArizaBildirimHeader.Id = dbo.ArizaBildirimItem.ArizaBildirimHeaderId INNER JOIN                         dbo.PotaKart ON dbo.ArizaBildirimItem.PotaKartId = dbo.PotaKart.Id LEFT OUTER JOIN                         dbo.BilesenItem ON dbo.ArizaBildirimItem.BilesenItemId = dbo.BilesenItem.Id LEFT OUTER JOIN                         dbo.BilesenHeader ON dbo.ArizaBildirimItem.BilesenHeaderId = dbo.BilesenHeader.Id WHERE        (dbo.ArizaBildirimItem.ArizaBildirimHeaderId = {0}) AND (dbo.ArizaBildirimItem.KapananArizaTanimId = 0)",
                    item.Id);


                result = BllMssql.CustomSql(squery, dbCon).ToList();
                var dropIcs = new List<DropItem>();

                var potakarts = new List<PotaKart>();

                var bilesenHeaders = new List<BilesenHeader>();
                var bilesenItems = new List<BilesenItem>();
                foreach (var aa in result)
                {
                    var lst = aa.Values.ToList();

                    var BilesenHeaderId = (int)lst[0];
                    var BilesenItemId = (int)lst[1];
                    var PotaKartId = (int)lst[2];
                    var PotaKartAdi = lst[3]?.ToString();
                    var BilesenHeaderAdi = lst[4]?.ToString();
                    var BilesenDegeri = lst[5]?.ToString();

                    if (potakarts.Count(a => a.Id == PotaKartId) == 0)
                    {
                        potakarts.Add(new PotaKart()
                        {
                            Id = PotaKartId,
                            PotaKartAdi = PotaKartAdi
                        });
                    }

                    if (bilesenHeaders.Count(a => a.Id == BilesenHeaderId) == 0)
                    {
                        bilesenHeaders.Add(new BilesenHeader()
                        {
                            Id = BilesenHeaderId,
                            BilesenHeaderAdi = BilesenHeaderAdi
                        });
                    }
                    if (bilesenItems.Count(a => a.Id == BilesenItemId) == 0)
                    {
                        bilesenItems.Add(new BilesenItem()
                        {
                            Id = BilesenItemId,
                            BilesenDegeri = BilesenDegeri
                        });
                    }

                    dropIcs.Add(new DropItem()
                    {
                        IdInt = PotaKartId,
                        IdInt2 = BilesenHeaderId,
                        IdInt3 = BilesenItemId,
                        Tanim1 = PotaKartAdi,
                        Tanim2 = BilesenHeaderAdi,
                        Tanim3 = BilesenDegeri
                    });

                }


                foreach (var potaKart in potakarts.OrderBy(a => a.PotaKartAdi).ToList())
                {

                    var raporlamaArizaModelItemPotaKart = new RaporlamaArizaModelItemPotaKart()
                    {
                        PotaKartAdi = potaKart.PotaKartAdi,
                        PotaKartId = potaKart.Id
                    };

                    var araDrops = dropIcs.Where(a => a.IdInt == potaKart.Id).OrderBy(a => a.Tanim2).ToList();

                    var distBilesenHeaders = araDrops.Select(a => a.IdInt2).ToList();
                    foreach (var i in distBilesenHeaders)
                    {
                        var drop = araDrops.First(a => a.IdInt2 == i);
                        var itemValues = araDrops.Where(a => a.IdInt2 == i).OrderBy(a => a.Tanim3).ToList();

                        raporlamaArizaModelItemPotaKart.DropBilesenHeaderItems.Add(new DropItem()
                        {
                            IdInt = i,
                            Tanim = drop.Tanim2,
                            ItemValues = itemValues.Select(a => new ItemValue() { IdInt = a.IdInt3, Text = a.Tanim3 }).ToList()
                        });
                    }
                    item.RaporlamaArizaModelItemPotaKarts.Add(raporlamaArizaModelItemPotaKart);
                }

                model.RaporlamaArizaModelItems.Add(item);
            }
            return PartialView("_ArizaBildirimAnalizYap", model);

        }
        #endregion


        #region ArizaBildirimRapor
        public ActionResult ArizaBildirimRapor()
        {
            var arizaTechizatGrubuTanims = _db.ArizaTechizatGrubuTanims.ToList();


            var model = new RaporlamaArizaModel()
            {
                DropArizaDurumus = _db.ArizaDurumuTanims.ToList()
                    .Select(a => new DropItem() { IdInt = a.Id, Tanim = a.ArizaDurumuAdi, SeciliMi = true }).ToList(),
                DropSorumluDepartmans = DropSorumluDepartmanlariGetir()
            };


            var userDepts = _db.UserDepartmen.Where(a => a.UserId == User.Id).ToList();



            if (User.ArizaMudahaleYetki == 1)
            {
                // müdahale edebilir

                foreach (var item in model.DropSorumluDepartmans)
                {
                    item.SeciliMi = userDepts.Any(a => a.DepartmanId == item.IdInt);
                }
            }




            foreach (var item in arizaTechizatGrubuTanims)
            {

                model.DropArizaTechizatGruplar.Add(new DropItem()
                {
                    IdInt = item.Id,
                    Tanim = item.ArizaTechizatGrubuAdi,
                    SeciliMi = true
                });
            }
            return View(model);
        }

        [HttpPost]
        public PartialViewResult ArizaBildirimAnalizYap(RaporlamaArizaModel model)
        {
            var basTarihi = tarihiOlustur(model.BaslangicTarihi);
            var bitTarihi = tarihiOlustur(model.BitisTarihi.AddDays(1));

            var strDbCon = SuaHelper.defaultSql();
            var seciliTechizatGrupIdler = model.DropArizaTechizatGruplar.Where(a => a.SeciliMi).Select(a => a.IdInt).ToList();

            var seciliSorumluDeptIdler = model.DropSorumluDepartmans.Where(a => a.SeciliMi).Select(a => a.IdInt).ToList();
            var seciliArizaDurumus = model.DropArizaDurumus.Where(a => a.SeciliMi).Select(a => a.IdInt).ToList();

            var squery = string.Format(
                "SELECT DISTINCT  dbo.ArizaBildirimHeader.ArizaTechizatGrubuId, dbo.ArizaBildirimHeader.Id, dbo.ArizaSorumluDepartman.DepartmanId, dbo.ArizaTechizatGrubuTanim.ArizaTechizatGrubuAdi,                          dbo.ArizaBildirimHeader.KayitTarihi, dbo.ArizaTanim.ArizaTanimAdi, ArizaTanim_1.ArizaTanimAdi AS KapananAriza, dbo.Departman.DepartmanAdi, dbo.[User].Name, dbo.ArizaBildirimHeader.ArizaDurumu,                          dbo.ArizaDurumuTanim.ArizaDurumuAdi FROM            dbo.ArizaBildirimHeader INNER JOIN                         dbo.ArizaTechizatGrubuTanim ON dbo.ArizaBildirimHeader.ArizaTechizatGrubuId = dbo.ArizaTechizatGrubuTanim.Id INNER JOIN                         dbo.ArizaTanim ON dbo.ArizaBildirimHeader.AcilanArizaTanimId = dbo.ArizaTanim.Id INNER JOIN                         dbo.ArizaTanim AS ArizaTanim_1 ON dbo.ArizaBildirimHeader.KapananArizaTanimId = ArizaTanim_1.Id INNER JOIN                         dbo.ArizaSorumluDepartman ON ArizaTanim_1.Id = dbo.ArizaSorumluDepartman.ArizaTanimId INNER JOIN                         dbo.Departman ON dbo.ArizaSorumluDepartman.DepartmanId = dbo.Departman.Id INNER JOIN                         dbo.[User] ON dbo.ArizaBildirimHeader.KayitYapan = dbo.[User].Id INNER JOIN                         dbo.ArizaDurumuTanim ON dbo.ArizaBildirimHeader.ArizaDurumu = dbo.ArizaDurumuTanim.Id WHERE        (dbo.ArizaBildirimHeader.KayitTarihi >= CONVERT(DATETIME, '{0}', 102)) AND (dbo.ArizaBildirimHeader.KayitTarihi <= CONVERT(DATETIME, '{1}', 102)) ORDER BY dbo.ArizaBildirimHeader.KayitTarihi DESC",
                basTarihi, bitTarihi);

            var result = BllMssql.CustomSql(squery, strDbCon).ToList();
            var items = new List<RaporlamaArizaModelItem>();
            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();

                var ArizaTechizatGrubuId = (int)lst[0];
                var Id = (int)lst[1];
                var DepartmanId = (int)lst[2];
                var ArizaTechizatGrubuAdi = lst[3]?.ToString();
                var KayitTarihi = (DateTime)lst[4];
                var ArizaTanimAdi = lst[5]?.ToString();
                var KapananAriza = lst[6]?.ToString();
                var DepartmanAdi = lst[7]?.ToString();
                var Name = lst[8]?.ToString();
                var ArizaDurumu = (int)lst[9];
                var ArizaDurumuAdi = lst[10]?.ToString();


                if (seciliTechizatGrupIdler.Any(a => a == ArizaTechizatGrubuId) && seciliSorumluDeptIdler.Any(a => a == DepartmanId) && seciliArizaDurumus.Any(a => a == ArizaDurumu))
                {
                    if (items.Any(a => a.Id == Id))
                    {
                        var item = items.First(a => a.Id == Id);
                        item.DepartmanAdi += " - " + DepartmanAdi;
                    }
                    else
                    {
                        items.Add(new RaporlamaArizaModelItem()
                        {
                            Id = Id,
                            ArizaTechizatGrubuId = ArizaTechizatGrubuId,
                            AcilanArizaTanimAdi = ArizaTanimAdi,
                            ArizaTechizatGrubuAdi = ArizaTechizatGrubuAdi,
                            DepartmanAdi = DepartmanAdi,
                            DepartmanId = DepartmanId,
                            KapananArizaTanimAdi = KapananAriza,
                            KayitTarihi = KayitTarihi,
                            KayitYapanKulAdi = Name,
                            ArizaDurumu = ArizaDurumu,
                            ArizaDurumuAdi = ArizaDurumuAdi
                        });
                    }
                }

            }

            foreach (var item in items)
            {
                squery = string.Format(
                    "SELECT        dbo.ArizaBildirimItem.BilesenHeaderId, dbo.ArizaBildirimItem.BilesenItemId, dbo.ArizaBildirimItem.PotaKartId, dbo.PotaKart.PotaKartAdi, dbo.BilesenHeader.BilesenHeaderAdi, dbo.BilesenItem.BilesenDegeri FROM            dbo.ArizaBildirimHeader INNER JOIN                         dbo.ArizaBildirimItem ON dbo.ArizaBildirimHeader.Id = dbo.ArizaBildirimItem.ArizaBildirimHeaderId INNER JOIN                         dbo.PotaKart ON dbo.ArizaBildirimItem.PotaKartId = dbo.PotaKart.Id LEFT OUTER JOIN                         dbo.BilesenItem ON dbo.ArizaBildirimItem.BilesenItemId = dbo.BilesenItem.Id LEFT OUTER JOIN                         dbo.BilesenHeader ON dbo.ArizaBildirimItem.BilesenHeaderId = dbo.BilesenHeader.Id WHERE        (dbo.ArizaBildirimItem.ArizaBildirimHeaderId = {0}) AND (dbo.ArizaBildirimItem.KapananArizaTanimId = 0)",
                    item.Id);


                result = BllMssql.CustomSql(squery, strDbCon).ToList();
                var dropIcs = new List<DropItem>();

                var potakarts = new List<PotaKart>();

                var bilesenHeaders = new List<BilesenHeader>();
                var bilesenItems = new List<BilesenItem>();
                foreach (var aa in result)
                {
                    var lst = aa.Values.ToList();

                    var BilesenHeaderId = (int)lst[0];
                    var BilesenItemId = (int)lst[1];
                    var PotaKartId = (int)lst[2];
                    var PotaKartAdi = lst[3]?.ToString();
                    var BilesenHeaderAdi = lst[4]?.ToString();
                    var BilesenDegeri = lst[5]?.ToString();

                    if (potakarts.Count(a => a.Id == PotaKartId) == 0)
                    {
                        potakarts.Add(new PotaKart()
                        {
                            Id = PotaKartId,
                            PotaKartAdi = PotaKartAdi
                        });
                    }

                    if (bilesenHeaders.Count(a => a.Id == BilesenHeaderId) == 0)
                    {
                        bilesenHeaders.Add(new BilesenHeader()
                        {
                            Id = BilesenHeaderId,
                            BilesenHeaderAdi = BilesenHeaderAdi
                        });
                    }
                    if (bilesenItems.Count(a => a.Id == BilesenItemId) == 0)
                    {
                        bilesenItems.Add(new BilesenItem()
                        {
                            Id = BilesenItemId,
                            BilesenDegeri = BilesenDegeri
                        });
                    }

                    dropIcs.Add(new DropItem()
                    {
                        IdInt = PotaKartId,
                        IdInt2 = BilesenHeaderId,
                        IdInt3 = BilesenItemId,
                        Tanim1 = PotaKartAdi,
                        Tanim2 = BilesenHeaderAdi,
                        Tanim3 = BilesenDegeri
                    });

                }


                foreach (var potaKart in potakarts.OrderBy(a => a.PotaKartAdi).ToList())
                {

                    var raporlamaArizaModelItemPotaKart = new RaporlamaArizaModelItemPotaKart()
                    {
                        PotaKartAdi = potaKart.PotaKartAdi,
                        PotaKartId = potaKart.Id
                    };

                    var araDrops = dropIcs.Where(a => a.IdInt == potaKart.Id).OrderBy(a => a.Tanim2).ToList();

                    var distBilesenHeaders = araDrops.Select(a => a.IdInt2).ToList();
                    foreach (var i in distBilesenHeaders)
                    {
                        var drop = araDrops.First(a => a.IdInt2 == i);
                        var itemValues = araDrops.Where(a => a.IdInt2 == i).OrderBy(a => a.Tanim3).ToList();

                        raporlamaArizaModelItemPotaKart.DropBilesenHeaderItems.Add(new DropItem()
                        {
                            IdInt = i,
                            Tanim = drop.Tanim2,
                            ItemValues = itemValues.Select(a => new ItemValue() { IdInt = a.IdInt3, Text = a.Tanim3 }).ToList()
                        });
                    }
                    item.RaporlamaArizaModelItemPotaKarts.Add(raporlamaArizaModelItemPotaKart);
                }

                model.RaporlamaArizaModelItems.Add(item);
            }


            return PartialView("_ArizaBildirimAnalizYap", model);
        }


        #endregion




        #region MyRegion
        /*
          if (seciliArizaDurumIds.Any() || seciliSorumluDeptIds.Any())
            {
                squery += string.Format(" WHERE (PotaKartId = {0})",potakart.Id);

                if (seciliArizaDurumIds.Any())
                {
                    squery += " AND ";
                    foreach (var i in seciliArizaDurumIds)
                    {
                        squery += string.Format(" (ArizaDurumu = {0})", i);

                        if (i != seciliArizaDurumIds.Last())
                        {
                            squery += " OR ";
                        }
                    }

                }

                if (seciliSorumluDeptIds.Any())
                {
                    squery += " AND ";
                    foreach (var i in seciliSorumluDeptIds)
                    {
                        squery += string.Format(" (DepartmanId  = {0})", i);

                        if (i != seciliSorumluDeptIds.Last())
                        {
                            squery += " OR ";
                        }
                    }

                }
            }
         */

        /*
     public ActionResult ArizaBildirimRaporMakineBazli()
     {
         var model = new RaporlamaArizaModel();
         var squery = string.Format(
             "SELECT DISTINCT                          TOP (100) PERCENT dbo.PotaKart.TechizatTuruId, dbo.TechizatTuruTanim.TechizatTuruTanimAdi, dbo.PotaKart.PotaKartAdi, dbo.ArizaBildirimItem.PotaKartId, dbo.ArizaBildirimItem.BilesenHeaderId,                          dbo.BilesenHeader.BilesenHeaderAdi, dbo.ArizaBildirimItem.BilesenItemId, dbo.BilesenItem.BilesenDegeri FROM            dbo.ArizaBildirimItem INNER JOIN                         dbo.PotaKart ON dbo.ArizaBildirimItem.PotaKartId = dbo.PotaKart.Id INNER JOIN                         dbo.TechizatTuruTanim ON dbo.PotaKart.TechizatTuruId = dbo.TechizatTuruTanim.Id LEFT OUTER JOIN                         dbo.BilesenItem ON dbo.ArizaBildirimItem.BilesenItemId = dbo.BilesenItem.Id LEFT OUTER JOIN                         dbo.BilesenHeader ON dbo.ArizaBildirimItem.BilesenHeaderId = dbo.BilesenHeader.Id ORDER BY dbo.TechizatTuruTanim.TechizatTuruTanimAdi, dbo.PotaKart.PotaKartAdi");

         var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();
         var hamliste = new List<RaporlamaAcilanArizaGenelItem>();

         var dropTechizats = DropTechizatKirilimlar().ToList();

         var distTechizats = new List<TechizatTuruTanim>();

         var distPotaKarts = new List<PotaKart>();

         var distBilesenHeaders = new List<BilesenHeader>();
         var distBilesenItems = new List<BilesenItem>();

         foreach (var aa in result)
         {
             var lst = aa.Values.ToList();

             var TechizatTuruId = (int)lst[0];
             var TechizatTuruTanimAdi = lst[1]?.ToString();
             var PotaKartAdi = lst[2]?.ToString();
             var PotaKartId = (int)lst[3];
             var BilesenHeaderId = (int)lst[4];
             var BilesenHeaderAdi = lst[5]?.ToString();
             var BilesenItemId = (int)lst[6];
             var BilesenDegeri = lst[7]?.ToString();
             hamliste.Add(new RaporlamaAcilanArizaGenelItem()
             {
                 PotaKartAdi = PotaKartAdi,
                 PotaKartId = PotaKartId,
                 BilesenDegeri = BilesenDegeri,
                 BilesenHeaderAdi = BilesenHeaderAdi,
                 BilesenHeaderId = BilesenHeaderId,
                 BilesenItemId = BilesenItemId,
                 TechizatTuruId = TechizatTuruId,
                 TechizatTuruTanimAdi = TechizatTuruTanimAdi,
             });

             if (distTechizats.Count(a => a.Id == TechizatTuruId) == 0)
             {
                 if (dropTechizats.Any(a => a.IdInt == TechizatTuruId))
                 {
                     var dropItem = dropTechizats.First(a => a.IdInt == TechizatTuruId);

                     distTechizats.Add(new TechizatTuruTanim()
                     {
                         Id = TechizatTuruId,
                         TechizatTuruTanimAdi = dropItem.Tanim
                     });
                 }
             }

             if (distPotaKarts.Count(a => a.Id == PotaKartId) == 0)
             {
                 distPotaKarts.Add(new PotaKart()
                 {
                     TechizatTuruId = TechizatTuruId,
                     Id = PotaKartId,
                     PotaKartAdi = PotaKartAdi
                 });
             }


             if (distBilesenHeaders.Count(a => a.Id == BilesenHeaderId) == 0)
             {
                 if (BilesenHeaderId == 0)
                 {
                     distBilesenHeaders.Add(new BilesenHeader()
                     {
                         Id = 0,
                         BilesenHeaderAdi = "Seçimsiz"
                     });
                 }
                 else
                 {
                     distBilesenHeaders.Add(new BilesenHeader()
                     {
                         Id = BilesenHeaderId,
                         BilesenHeaderAdi = BilesenHeaderAdi
                     });
                 }
             }
             if (distBilesenItems.Count(a => a.Id == BilesenItemId) == 0)
             {
                 if (BilesenItemId == 0)
                 {
                     distBilesenItems.Add(new BilesenItem()
                     {
                         Id = 0,
                         BilesenDegeri = "Seçimsiz"
                     });
                 }
                 else
                 {
                     distBilesenItems.Add(new BilesenItem()
                     {
                         Id = BilesenItemId,
                         BilesenDegeri = BilesenDegeri
                     });
                 }
             }
         }

         distTechizats = distTechizats.OrderBy(a => a.TechizatTuruTanimAdi).ToList();



         foreach (var techizatTuruTanim in distTechizats)
         {
             var araItem = new RaporlamaMakineBazliAnalizItem
             {
                 TechizatTuruTanim = techizatTuruTanim
             };

             var pots = distPotaKarts.Where(a => a.TechizatTuruId == techizatTuruTanim.Id)
                 .OrderBy(a => a.PotaKartAdi).ToList();

             var araItem1ler = new List<RaporlamaMakineBazliAnalizPK>();
             foreach (var potaKart in pots)
             {

                 var potaKartBheaderItems = hamliste.Where(a => a.PotaKartId == potaKart.Id)
                     .OrderBy(a => a.BilesenHeaderAdi).ThenBy(a=>a.BilesenDegeri).ToList();
                 var dropsAra = new List<DropItem>();
                 var distBheaders = potaKartBheaderItems.Select(a => a.BilesenHeaderId).Distinct().ToList();



                 foreach (var i in distBheaders)
                 {

                     var drop = new DropItem() {IdInt = i, Tanim = distBilesenHeaders.First(a=>a.Id==i).BilesenHeaderAdi};

                     var araDistBilesenItems = potaKartBheaderItems.Where(a => a.BilesenHeaderId == i).ToList().Select(a=>a.BilesenItemId).Distinct().ToList();

                     foreach (var j in araDistBilesenItems)
                     {
                         if (distBilesenItems.Any(a=>a.Id==j))
                         {
                             drop.ItemValues.Add(new ItemValue()
                             {
                                 IdInt = j,
                                 Text = distBilesenItems.First(a => a.Id == j).BilesenDegeri
                             });
                         }

                     }

                     dropsAra.Add(drop);

                 }


                 araItem1ler.Add(new RaporlamaMakineBazliAnalizPK()
                 {
                     PotaKart = potaKart,
                     DropBilesenHeaderItems = dropsAra
                 });

             }


             araItem.PkList = araItem1ler.ToList();

             model.RaporlamaMakineBazliAnalizItems.Add(araItem);

         }




         return View(model);
     }
     */
        #endregion

    }
}
