using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Office.Interop.Excel;
using PolyBota.BLL;
using PolyBota.DAL;
using PolyBota.Entities;
using PolyBota.Helpers;
using PolyBota.Web.Areas.Tanimlamalar.Models;
using PolyBota.Web.Controllers;

namespace PolyBota.Web.Areas.Tanimlamalar.Controllers
{
    public class ArizaTemelController : BaseController
    {
        // GET: Tanimlamalar/ArizaTemel

        #region Arıza Techizat Grup CRUD
        public ActionResult ArizaTechizatGrupListe()
        {
            var model = new TanimlamaArizaModel()
            {
                //TechizatTuruTanims = _db.TechizatTuruTanims.OrderBy(a => a.TechizatTuruTanimAdi).ToList(),
                ArizaTechizatGrubuTanims = _db.ArizaTechizatGrubuTanims.OrderBy(a => a.ArizaTechizatGrubuAdi).ToList(),
                ArizaTechizatItemTanims = _db.ArizaTechizatItemTanims.ToList(),
                DropItems = DropDepartmanKirilimlariGetir()
            };
            var dropTechizatTurus = DropTechizatKirilimlar();

            foreach (var item in dropTechizatTurus)
            {
                model.TechizatTuruTanims.Add(new TechizatTuruTanim()
                {
                    Id = item.IdInt,
                    TechizatTuruTanimAdi = item.Tanim
                });
            }
            return View(model);
        }

        public PartialViewResult ArizaTechGrubuTanimEkleDuzenle(int id = 0)
        {
            var model = new TanimlamaArizaModel()
            {
                //TechizatTuruTanims = _db.TechizatTuruTanims.OrderBy(a => a.TechizatTuruTanimAdi).ToList(),
                ArizaTechizatGrubuTanim = new ArizaTechizatGrubuTanim()
            };
            var dropTechizatTurus = DropTechizatKirilimlar();

            foreach (var item in dropTechizatTurus)
            {
                model.TechizatTuruTanims.Add(new TechizatTuruTanim()
                {
                    Id = item.IdInt,
                    TechizatTuruTanimAdi = item.Tanim
                });
            }
            if (id != 0)
            {
                model.ArizaTechizatGrubuTanim = _db.ArizaTechizatGrubuTanims.Find(id);

            }
            var kayitliTechs = _db.ArizaTechizatItemTanims.Where(a => a.ArizaTechizatGrubuId == id).ToList();

            foreach (var item in model.TechizatTuruTanims)
            {
                if (kayitliTechs.Any(a => a.TechizatTuruId == item.Id))
                {
                    model.ArizaTechizatItemTanims.Add(kayitliTechs.First(a => a.TechizatTuruId == item.Id));
                }
                else
                {
                    model.ArizaTechizatItemTanims.Add(new ArizaTechizatItemTanim()
                    {
                        TechizatTuruId = item.Id
                    });
                }
            }

            return PartialView("_ArizaTechGrubuTanimEkleDuzenle", model);
        }

        [HttpPost]
        public ActionResult ArizaTechGrubuTanimEkleDuzenle(TanimlamaArizaModel model)
        {
            var grupTanim = model.ArizaTechizatGrubuTanim;
            var seciliListe = model.ArizaTechizatItemTanims.Where(a => a.SeciliMi).ToList();



            var eklenecekListe = new List<ArizaTechizatItemTanim>();
            var silinecekListe = new List<ArizaTechizatItemTanim>();

            if (string.IsNullOrEmpty(grupTanim.ArizaTechizatGrubuAdi) || seciliListe.Count == 0)
            {
                TempDataCreate(4);
            }
            else
            {
                if (grupTanim.Id == 0)
                {
                    _db.ArizaTechizatGrubuTanims.Add(grupTanim);
                    _db.SaveChanges();

                    foreach (var item in seciliListe)
                    {
                        eklenecekListe.Add(new ArizaTechizatItemTanim()
                        {
                            SeciliMi = true,
                            TechizatTuruId = item.TechizatTuruId,
                            ArizaTechizatGrubuId = grupTanim.Id
                        });
                    }


                }
                else
                {
                    var editGrup = _db.ArizaTechizatGrubuTanims.Find(grupTanim.Id);
                    editGrup.ArizaTechizatGrubuAdi = grupTanim.ArizaTechizatGrubuAdi;
                    _db.SaveChanges();

                    var kayitliListe = _db.ArizaTechizatItemTanims.Where(a => a.ArizaTechizatGrubuId == grupTanim.Id)
                        .ToList();

                    foreach (var item in seciliListe)
                    {
                        if (kayitliListe.Count(a => a.TechizatTuruId == item.TechizatTuruId) == 0)
                        {
                            eklenecekListe.Add(new ArizaTechizatItemTanim()
                            {
                                SeciliMi = true,
                                TechizatTuruId = item.TechizatTuruId,
                                ArizaTechizatGrubuId = grupTanim.Id
                            });
                        }
                    }

                    foreach (var item in kayitliListe)
                    {
                        if (seciliListe.Count(a => a.TechizatTuruId == item.TechizatTuruId) == 0)
                        {
                            silinecekListe.Add(item);
                        }
                    }
                }

                if (eklenecekListe.Any())
                {
                    _db.ArizaTechizatItemTanims.AddRange(eklenecekListe);
                    _db.SaveChanges();
                }
                if (silinecekListe.Any())
                {
                    _db.ArizaTechizatItemTanims.RemoveRange(silinecekListe);
                    _db.SaveChanges();
                }
                TempDataCreate(2);
            }


            return RedirectToAction("ArizaTechizatGrupListe");
        }


        #endregion

        #region Arıza Tanım Grup

        public ActionResult ArizaTanimGrupListe()
        {
            var model = new TanimlamaArizaModel()
            {
                ArizaTanimGrups = _db.ArizaTanimGrups.OrderBy(a => a.ArizaTanimGrupAdi).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult ArizaTanimGrupEkleDuzenle(TanimlamaArizaModel model)
        {
            var item = model.ArizaTanimGrup;
            if (string.IsNullOrEmpty(item.ArizaTanimGrupAdi))
            {
                TempDataCreate(4);
            }
            else
            {
                item.ArizaTanimGrupAdi = item.ArizaTanimGrupAdi.FirsToUpper();
                if (item.Id == 0)
                {
                    _db.ArizaTanimGrups.Add(item);
                    _db.SaveChanges();
                }
                else
                {
                    var editItem = _db.ArizaTanimGrups.Find(item.Id);
                    editItem.ArizaTanimGrupAdi = item.ArizaTanimGrupAdi;
                    _db.SaveChanges();
                }

                TempDataCreate(2);
            }
            return RedirectToAction("ArizaTanimGrupListe");
        }

        public ActionResult ArizaTanimGrupSil(int id)
        {
            if (_db.ArizaTanims.Any(a => a.ArizaTanimGrupId == id))
            {
                TempDataCustom(1, "Bu Gruba Bağlı Tanımlar Bulunmaktadır Silme Yapılamaz");
            }
            else
            {
                var editItem = _db.ArizaTanimGrups.Find(id);
                _db.ArizaTanimGrups.Remove(editItem);
                _db.SaveChanges();
                TempDataCreate(3);
            }

            return RedirectToAction("ArizaTanimGrupListe");
        }
        #endregion
        #region Arıza Tanım


        public ActionResult ArizaTanimListeArizaTechizataGore(int CurrentPage = 1, int PageShowCount = 150, int katId = 0)
        {
            var model = new TanimlamaArizaModel()
            {
                TechizatTuruTanims = _db.TechizatTuruTanims.OrderBy(a => a.TechizatTuruTanimAdi).ToList(),
               
                KatId = katId,
                BilesenHeaders = _db.BilesenHeaders.ToList(),
                ArizaTanimGrups = _db.ArizaTanimGrups.OrderBy(a => a.ArizaTanimGrupAdi).ToList(),
                ArizaTechizatGrubuTanims = _db.ArizaTechizatGrubuTanims.OrderBy(a => a.ArizaTechizatGrubuAdi).ToList(),
                ArizaTechizatItemTanims = _db.ArizaTechizatItemTanims.ToList(),
                DropItems = DropDepartmanKirilimlariGetir()
            };
      

            model.ArizaTanimGrups.Add(new ArizaTanimGrup() { Id = 0, ArizaTanimGrupAdi = "Tümü" });
            var items = new List<DropItem>();

            var squery = string.Format("SELECT DISTINCT                          TOP (100) PERCENT dbo.ArizaTechizatGrubuTanim.ArizaTechizatGrubuAdi, dbo.ArizaTanimGrup.ArizaTanimGrupAdi, dbo.ArizaTanim.ArizaTanimAdi, dbo.ArizaTanimArizaTechizatGrubu.ArizaTechizatGrubuId,                          dbo.ArizaTanimArizaTechizatGrubu.ArizaId FROM            dbo.ArizaTanimArizaTechizatGrubu INNER JOIN                         dbo.ArizaTanim ON dbo.ArizaTanimArizaTechizatGrubu.ArizaId = dbo.ArizaTanim.Id INNER JOIN                         dbo.ArizaTechizatGrubuTanim ON dbo.ArizaTanimArizaTechizatGrubu.ArizaTechizatGrubuId = dbo.ArizaTechizatGrubuTanim.Id INNER JOIN dbo.ArizaTanimGrup ON dbo.ArizaTanim.ArizaTanimGrupId = dbo.ArizaTanimGrup.Id ");

            if (katId != 0)
            {

                squery += string.Format(" WHERE (dbo.ArizaTanimArizaTechizatGrubu.ArizaTechizatGrubuId = {0}) ", katId);

            }
            squery += " ORDER BY dbo.ArizaTechizatGrubuTanim.ArizaTechizatGrubuAdi, dbo.ArizaTanimGrup.ArizaTanimGrupAdi, dbo.ArizaTanim.ArizaTanimAdi";



            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();

                var ArizaTechizatGrubuAdi = lst[0]?.ToString();
                var ArizaTanimGrupAdi = lst[1]?.ToString();
                var ArizaTanimAdi = lst[2]?.ToString();
                var ArizaTechizatGrubuId = (int)lst[3];
                var ArizaId = (int)lst[4];

                items.Add(new DropItem()
                {
                    Tanim1 = ArizaTechizatGrubuAdi,
                    Tanim2 = ArizaTanimGrupAdi,
                    Tanim3 = ArizaTanimAdi,
                    Id = ArizaId.ToString(),
                    IdInt2 = ArizaTechizatGrubuId
                });
            }


            var pliste = new PagedListBase<DropItem>() { CurrentPage = CurrentPage, PageShowCount = PageShowCount, DataLists = items };
            var PageListBase = PageListBaseOlustur(pliste);
            model.PagedListSrcn = new PagedListSrcn()
            {
                PageShowCount = PageListBase.PageShowCount,
                PageSizeSelectList = PageListBase.PageSizeSelectList,
                PageNumberList = PageListBase.PageNumberList,
                CurrentPage = PageListBase.CurrentPage
            };

            model.DropItems2 = pliste.DataLists;
            return View(model);
        }

        public ActionResult ArizaTanimListe(int CurrentPage = 1, int PageShowCount = 150, int katId = 0)
        {
            var model = new TanimlamaArizaModel()
            {
                TechizatTuruTanims = _db.TechizatTuruTanims.OrderBy(a => a.TechizatTuruTanimAdi).ToList(),
                ArizaTechizatGrubuTanims = _db.ArizaTechizatGrubuTanims.OrderBy(a => a.ArizaTechizatGrubuAdi).ToList(),
                DropItems = DropDepartmanKirilimlariGetir(),
                KatId = katId,
                BilesenHeaders = _db.BilesenHeaders.ToList(),
                ArizaTanimGrups = _db.ArizaTanimGrups.OrderBy(a => a.ArizaTanimGrupAdi).ToList()
            };
            model.ArizaTanimGrups.Add(new ArizaTanimGrup() { Id = 0, ArizaTanimGrupAdi = "Tümü" });
            var itemIdler = new List<int>();

            var squery = string.Format("SELECT Id FROM dbo.ArizaTanim");

            if (katId != 0)
            {

                squery += string.Format(" WHERE (ArizaTanimGrupId = {0})", katId);

            }
            squery += " ORDER BY ArizaTanimAdi";



            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

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
                model.ArizaTanims.Add(_db.ArizaTanims.Find(i));
                //model.ArizaSorumluDepartmans.AddRange(_db.ArizaSorumluDepartmen.Where(a => a.ArizaTanimId == i));
                //model.ArizaAcilabilecekDepartmans.AddRange(_db.ArizaAcilabilecekDepartmen.Where(a => a.ArizaTanimId == i));
                //model.ArizaTanimGerekliBilesenHeaders.AddRange(_db.ArizaTanimGerekliBilesenHeaders.Where(a => a.ArizaTanimId == i));
                //model.ArizaTanimGerekliTechizatTurus.AddRange(_db.ArizaTanimGerekliTechizatTurus.Where(a => a.ArizaTanimId == i));
            }

            return View(model);
        }


        public TanimlamaArizaModel DropBilesenHeaderOlustur(TanimlamaArizaModel model, int ArizaTechizatGrubuId, int arizaTanimArizaTechizatGrubu)
        {
            var kirilimliTechizatTurleri = DropTechizatKirilimlar();
            var squery = string.Format(
                "SELECT dbo.TechizatTuruBilesenHeader.TechizatTuruTanimId, dbo.TechizatTuruTanim.TechizatTuruTanimAdi, dbo.TechizatTuruBilesenHeader.BilesenHeaderId, dbo.BilesenHeader.BilesenHeaderAdi FROM dbo.ArizaTechizatGrubuTanim INNER JOIN dbo.ArizaTechizatItemTanim ON dbo.ArizaTechizatGrubuTanim.Id = dbo.ArizaTechizatItemTanim.ArizaTechizatGrubuId INNER JOIN dbo.TechizatTuruTanim ON dbo.ArizaTechizatItemTanim.TechizatTuruId = dbo.TechizatTuruTanim.Id INNER JOIN dbo.TechizatTuruBilesenHeader ON dbo.TechizatTuruTanim.Id = dbo.TechizatTuruBilesenHeader.TechizatTuruTanimId INNER JOIN dbo.BilesenHeader ON dbo.TechizatTuruBilesenHeader.BilesenHeaderId = dbo.BilesenHeader.Id WHERE(dbo.ArizaTechizatItemTanim.ArizaTechizatGrubuId = {0}) ORDER BY dbo.TechizatTuruTanim.TechizatTuruTanimAdi", ArizaTechizatGrubuId);



            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();


            var dropTechizatTurleri = new List<DropItem>();
            var itemValues = new List<ItemValue>();
            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();

                var TechizatTuruTanimId = (int)lst[0];
                var TechizatTuruTanimAdi = kirilimliTechizatTurleri.First(a => a.IdInt == TechizatTuruTanimId).Tanim;  //lst[1].ToString();
                var BilesenHeaderId = (int)lst[2];
                var BilesenHeaderAdi = lst[3].ToString();

                if (dropTechizatTurleri.Count(a => a.IdInt == TechizatTuruTanimId) == 0)
                {
                    dropTechizatTurleri.Add(new DropItem()
                    {
                        IdInt = TechizatTuruTanimId,
                        Tanim = TechizatTuruTanimAdi
                    });
                }

                itemValues.Add(new ItemValue()
                {
                    Id = BilesenHeaderId.ToString(),
                    IdInt = TechizatTuruTanimId,
                    Text = BilesenHeaderAdi
                });


            }

            foreach (var item in dropTechizatTurleri)
            {
                item.ItemValues = itemValues.Where(a => a.IdInt == item.IdInt).OrderBy(a => a.Text).ToList();
            }
            /*
            if (model.ArizaTanim.Id != 0)
            {

                var kayitliArizaBilesenIdler = _db.ArizaTanimGerekliBilesenHeaders.Where(a => a.ArizaTanimId == model.ArizaTanim.Id).ToList().Select(a => a.BilesenHeaderId).ToList();
                var kayitliTechizatTurleri = _db.ArizaTanimGerekliTechizatTurus
                    .Where(a => a.ArizaTanimId == model.ArizaTanim.Id).ToList().Select(a => a.TechizatTuruId).ToList();


                foreach (var dropItem in dropTechizatTurleri)
                {
                    if (kayitliTechizatTurleri.Any(a => a == dropItem.IdInt))
                    {
                        dropItem.SeciliMi = true;
                    }

                    foreach (var itemValue in dropItem.ItemValues)
                    {
                        int ii = Convert.ToInt32(itemValue.Id);
                        if (kayitliArizaBilesenIdler.Any(a => a == ii))
                        {
                            itemValue.SeciliMi = true;
                        }
                    }

                }


            }
            */
            model.DropBilesenHeader = dropTechizatTurleri;
            return model;
        }


        public List<TanimlamaArizaAlternatifGosterimItem> ArizaAlternatifListeyiOlustur(int arizaId)
        {
            var model = new List<TanimlamaArizaAlternatifGosterimItem>();
            var squery = string.Format(
                "SELECT        dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimArizaTechizatGrupId, dbo.ArizaTanimArizaTechizatGrubu.ArizaId, dbo.ArizaTanimArizaTechizatGrubu.ArizaTechizatGrubuId, dbo.ArizaTanimGerekliTechizatTuru.TechizatTuruId,                          ISNULL(dbo.ArizaTanimGerekliBilesenHeader.BilesenHeaderId, 0) AS BilesenHeaderId, dbo.ArizaTanim.ArizaTanimAdi, dbo.ArizaTechizatGrubuTanim.ArizaTechizatGrubuAdi, dbo.TechizatTuruTanim.TechizatTuruTanimAdi,                          dbo.BilesenHeader.BilesenHeaderAdi FROM            dbo.BilesenHeader INNER JOIN                         dbo.ArizaTanimGerekliBilesenHeader ON dbo.BilesenHeader.Id = dbo.ArizaTanimGerekliBilesenHeader.BilesenHeaderId RIGHT OUTER JOIN                         dbo.ArizaTechizatGrubuTanim INNER JOIN                         dbo.ArizaTanim INNER JOIN                         dbo.ArizaTanimArizaTechizatGrubu ON dbo.ArizaTanim.Id = dbo.ArizaTanimArizaTechizatGrubu.ArizaId INNER JOIN                         dbo.ArizaTanimGerekliTechizatTuru ON dbo.ArizaTanimArizaTechizatGrubu.ArizaId = dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimId AND                          dbo.ArizaTanimArizaTechizatGrubu.Id = dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimArizaTechizatGrupId INNER JOIN                         dbo.TechizatTuruTanim ON dbo.ArizaTanimGerekliTechizatTuru.TechizatTuruId = dbo.TechizatTuruTanim.Id ON dbo.ArizaTechizatGrubuTanim.Id = dbo.ArizaTanimArizaTechizatGrubu.ArizaTechizatGrubuId ON                          dbo.ArizaTanimGerekliBilesenHeader.ArizaTanimArizaTechizatGrupId = dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimArizaTechizatGrupId AND                          dbo.ArizaTanimGerekliBilesenHeader.TechizatTuruId = dbo.ArizaTanimGerekliTechizatTuru.TechizatTuruId AND dbo.ArizaTanimGerekliBilesenHeader.ArizaTanimId = dbo.ArizaTanimGerekliTechizatTuru.ArizaTanimId WHERE        (dbo.ArizaTanimArizaTechizatGrubu.ArizaId = {0})",
                arizaId);


            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

            var hamlisteAlternatifOzet = new List<TanimlamaArizaAlternatifOzetItem>();
            var itemValues = new List<ItemValue>();
            List<int> distArizaTanimArizaTechizatGrupIdler = new List<int>();
            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();

                int ArizaTanimArizaTechizatGrupId = (int)lst[0];
                int ArizaId = (int)lst[1];

                int ArizaTechizatGrubuId = (int)lst[2];
                int TechizatTuruId = (int)lst[3];
                int BilesenHeaderId = (int)lst[4];

                string ArizaTanimAdi = lst[5]?.ToString();
                string ArizaTechizatGrubuAdi = lst[6]?.ToString();
                string TechizatTuruTanimAdi = lst[7]?.ToString();
                string BilesenHeaderAdi = lst[8]?.ToString();
                if (distArizaTanimArizaTechizatGrupIdler.Count(a => a == ArizaTanimArizaTechizatGrupId) == 0)
                {
                    distArizaTanimArizaTechizatGrupIdler.Add(ArizaTanimArizaTechizatGrupId);
                }
                hamlisteAlternatifOzet.Add(new TanimlamaArizaAlternatifOzetItem()
                {
                    ArizaTanimArizaTechizatGrupId = ArizaTanimArizaTechizatGrupId,
                    ArizaId = ArizaId,
                    ArizaTechizatGrubuId = ArizaTechizatGrubuId,
                    TechizatTuruId = TechizatTuruId,
                    BilesenHeaderId = BilesenHeaderId,

                    ArizaTanimAdi = ArizaTanimAdi,
                    ArizaTechizatGrubuAdi = ArizaTechizatGrubuAdi,
                    TechizatTuruTanimAdi = TechizatTuruTanimAdi,
                    BilesenHeaderAdi = BilesenHeaderAdi
                });

            }

            var Seviye1ArizaTechizatGrubuAdinaGore = new List<TanimlamaArizaAlternatifOzetItem>(); //Üretim Makinesi Arıza
            var Seviye2TechizatTuruTanimAdinaGore = new List<TanimlamaArizaAlternatifOzetItem>(); //Üretim Makinesi

            var Seviye3BilesenHeaderaGore = new List<TanimlamaArizaAlternatifOzetItem>(); //Kanal


            foreach (var arizaTanimArizaTechizatGrupId in distArizaTanimArizaTechizatGrupIdler)
            {
                if (arizaTanimArizaTechizatGrupId == 5)
                {
                    var ff = 0;
                }
                var hamItem = hamlisteAlternatifOzet.First(a => a.ArizaTanimArizaTechizatGrupId == arizaTanimArizaTechizatGrupId);


                var arizaTechizatGrubu = new ArizaTechizatGrubuTanim()
                {
                    ArizaTechizatGrubuAdi = hamItem.ArizaTechizatGrubuAdi,
                    Id = hamItem.ArizaTechizatGrubuId
                };
                var techizataGoreListe = hamlisteAlternatifOzet.Where(a =>
                    a.ArizaTechizatGrubuId == hamItem.ArizaTechizatGrubuId &&
                    a.ArizaTanimArizaTechizatGrupId == hamItem.ArizaTanimArizaTechizatGrupId).OrderBy(a => a.TechizatTuruTanimAdi).ToList();

                var dropTechizatlar = new List<DropItem>();
                var distTechizatIdler = techizataGoreListe.Select(a => a.TechizatTuruId).Distinct().ToList();

                foreach (var techizatTuruId in distTechizatIdler)
                {
                    var drop = new DropItem()
                    {
                        IdInt = techizatTuruId,
                        Tanim = techizataGoreListe.First(a => a.TechizatTuruId == techizatTuruId).TechizatTuruTanimAdi
                    };


                    var itemvalues = techizataGoreListe.Where(a => a.TechizatTuruId == techizatTuruId).ToList();

                    foreach (var item in itemvalues.OrderBy(a => a.BilesenHeaderAdi).ToList())
                    {
                        if (item.BilesenHeaderId != 0)
                        {
                            drop.ItemValues.Add(new ItemValue()
                            {
                                IdInt = item.TechizatTuruId,
                                Text = item.BilesenHeaderAdi
                            });
                        }
                    }

                    dropTechizatlar.Add(drop);
                }




                var araItem = new TanimlamaArizaAlternatifGosterimItem
                {
                    ArizaTanimArizaTechizatGrupId = arizaTanimArizaTechizatGrupId,
                    ArizaTechizatGrubuTanim = arizaTechizatGrubu,
                    DropTechizatTuruBilesenHeader = dropTechizatlar
                };
                model.Add(araItem);

            }

            return model;
        }
        public List<DropItem> DropListArizaTechizatGrupBilesenliOlustur(int arizaTechizatGrubuId)
        {
            var kirilimliTechizatTurleri = DropTechizatKirilimlar();
            var squery = string.Format(
                "SELECT        dbo.ArizaTechizatItemTanim.ArizaTechizatGrubuId, dbo.ArizaTechizatItemTanim.TechizatTuruId, ISNULL(dbo.TechizatTuruBilesenHeader.BilesenHeaderId, 0) AS BilesenHeaderId,  dbo.ArizaTechizatGrubuTanim.ArizaTechizatGrubuAdi, dbo.TechizatTuruTanim.TechizatTuruTanimAdi, dbo.BilesenHeader.BilesenHeaderAdi FROM            dbo.BilesenHeader INNER JOIN dbo.TechizatTuruBilesenHeader ON dbo.BilesenHeader.Id = dbo.TechizatTuruBilesenHeader.BilesenHeaderId RIGHT OUTER JOIN dbo.ArizaTechizatGrubuTanim INNER JOIN dbo.ArizaTechizatItemTanim ON dbo.ArizaTechizatGrubuTanim.Id = dbo.ArizaTechizatItemTanim.ArizaTechizatGrubuId INNER JOIN dbo.TechizatTuruTanim ON dbo.ArizaTechizatItemTanim.TechizatTuruId = dbo.TechizatTuruTanim.Id ON dbo.TechizatTuruBilesenHeader.TechizatTuruTanimId = dbo.TechizatTuruTanim.Id WHERE(dbo.ArizaTechizatItemTanim.ArizaTechizatGrubuId = {0})",
                arizaTechizatGrubuId);

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

            var dropTechizatTurleri = new List<DropItem>();
            var itemValues = new List<ItemValue>();
            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();


                var ArizaTechizatGrubuId = (int)lst[0];
                var TechizatTuruId = (int)lst[1];
                var BilesenHeaderId = (int)lst[2];
                var ArizaTechizatGrubuAdi = lst[3]?.ToString();
                var TechizatTuruTanimAdi = "";

                if (kirilimliTechizatTurleri.Any(a => a.IdInt == TechizatTuruId))
                {
                    TechizatTuruTanimAdi = kirilimliTechizatTurleri.First(a => a.IdInt == TechizatTuruId).Tanim; //lst[4]?.ToString();

                    var BilesenHeaderAdi = lst[5]?.ToString();


                    if (dropTechizatTurleri.Count(a => a.IdInt == TechizatTuruId) == 0)
                    {
                        dropTechizatTurleri.Add(new DropItem()
                        {
                            IdInt = TechizatTuruId,
                            Tanim = TechizatTuruTanimAdi
                        });
                    }
                    if (BilesenHeaderId != 0)
                    {
                        itemValues.Add(new ItemValue()
                        {
                            Id = BilesenHeaderId.ToString(),
                            IdInt = TechizatTuruId,
                            Text = BilesenHeaderAdi
                        });
                    }
                    foreach (var item in dropTechizatTurleri)
                    {
                        item.ItemValues = itemValues.Where(a => a.IdInt == item.IdInt).OrderBy(a => a.Text).ToList();
                    }


                }






            }

            return dropTechizatTurleri;

        }
        public ActionResult ArizaTanimEkleDuzenle(int id = 0)
        {
            var model = new TanimlamaArizaModel()
            {
                ArizaTechizatGrubuTanims = _db.ArizaTechizatGrubuTanims.OrderBy(a => a.ArizaTechizatGrubuAdi).ToList(),
                ArizaTechizatItemTanims = _db.ArizaTechizatItemTanims.ToList(),
                DropDepartmanlar = DropDepartmanKirilimlariGetir(),
                KatId = id,
                ArizaTanimGrups = _db.ArizaTanimGrups.OrderBy(a => a.ArizaTanimGrupAdi).ToList(),
                ArizaTanimCozums = _db.ArizaTanimCozums.Where(a => a.ArizaTanimId == id).ToList(),
                ArizaTanimCozum = new ArizaTanimCozum() { ArizaTanimId = id }
            };
            model.ArizaTechizatGrubuTanims.Add(new ArizaTechizatGrubuTanim()
            {
                Id = 0,
                ArizaTechizatGrubuAdi = "Seçiniz"
            });
            var kayitliSorumluDepartmanlar = new List<ArizaSorumluDepartman>();
            var kayitliAcilabilecekDepartmanlar = new List<ArizaAcilabilecekDepartman>();
            if (id != 0)
            {
                model.ArizaTanim = _db.ArizaTanims.Find(id);
                kayitliSorumluDepartmanlar = _db.ArizaSorumluDepartmen.Where(a => a.ArizaTanimId == id).ToList();
                kayitliAcilabilecekDepartmanlar = _db.ArizaAcilabilecekDepartmen.Where(a => a.ArizaTanimId == id).ToList();
                model.TanimlamaArizaAlternatifGosterimItems = ArizaAlternatifListeyiOlustur(id);
            }

            foreach (var item in model.DropDepartmanlar)
            {
                if (kayitliSorumluDepartmanlar.Any(a => a.DepartmanId == item.IdInt))
                {
                    model.ArizaSorumluDepartmans.Add(kayitliSorumluDepartmanlar.First(a => a.DepartmanId == item.IdInt));
                }
                else
                {
                    model.ArizaSorumluDepartmans.Add(new ArizaSorumluDepartman()
                    {
                        DepartmanId = item.IdInt
                    });
                }

                if (kayitliAcilabilecekDepartmanlar.Any(a => a.DepartmanId == item.IdInt))
                {
                    model.ArizaAcilabilecekDepartmans.Add(kayitliAcilabilecekDepartmanlar.First(a => a.DepartmanId == item.IdInt));
                }
                else
                {
                    model.ArizaAcilabilecekDepartmans.Add(new ArizaAcilabilecekDepartman()
                    {
                        DepartmanId = item.IdInt
                    });
                }

            }

            //  model = DropBilesenHeaderOlustur(model, model.ArizaTanim.ArizaTechizatGrubuId);
            return View(model);
        }

        public PartialViewResult ArizaTechizatGrubaGoreGereklililkGetir(int id, int arizaId, int arzTanArzTechGrupId = 0)
        {
            // 
            //id => ArizaTechizatGrubuId
            //arzTanArzTechGrupId=> ArizaTanimArizaTechizatGrubu

            var model = new TanimlamaArizaModel()
            {
                ArizaTanim = new ArizaTanim() { Id = arizaId },
                ArizaTanimArizaTechizatGrubu = new ArizaTanimArizaTechizatGrubu() { Id = arzTanArzTechGrupId, ArizaTechizatGrubuId = id }

            };
            //  model = DropBilesenHeaderOlustur(model, id, arzTanArzTechGrupId);
            var dropBilesenler = DropListArizaTechizatGrupBilesenliOlustur(id);

            foreach (var dropItem in dropBilesenler)
            {
                dropItem.ItemValues = dropItem.ItemValues.Distinct().ToList();

                var itemValues = dropItem.ItemValues;

                var yeniItemValues = new List<ItemValue>();

                foreach (var itemValue in itemValues)
                {
                    if (yeniItemValues.Count(a => a.Id == itemValue.Id) == 0)
                    {
                        yeniItemValues.Add(itemValue);
                    }
                }

                dropItem.ItemValues = yeniItemValues;
                model.DropBilesenHeader.Add(dropItem);
            }
            return PartialView("_ArizaTechizatGrubaGoreGereklililkGetir", model);
        }

        [HttpPost]
        public JsonResult ArizaTanimEkleDuzenle(TanimlamaArizaModel model)
        {
            int state = 1;
            int id = 0;

            var arizaTanim = model.ArizaTanim;
            var seciliDepartmanSorumlular = model.ArizaSorumluDepartmans.Where(a => a.SeciliMi).ToList();
            var seciliDepartmanAcilabilecekler = model.ArizaAcilabilecekDepartmans.Where(a => a.SeciliMi).ToList();


            if (string.IsNullOrEmpty(arizaTanim.ArizaTanimAdi) || seciliDepartmanSorumlular.Count == 0 || arizaTanim.ArizaTanimGrupId == 0)
            {
                state = 0;
            }
            else
            {

                var eklenecekArizaSorumluDeps = new List<ArizaSorumluDepartman>();
                var silinecekArizaSorumluDeps = new List<ArizaSorumluDepartman>();
                var eklenecekArizaAcilabilirDeps = new List<ArizaAcilabilecekDepartman>();
                var silinecekArizaAcilabilirDeps = new List<ArizaAcilabilecekDepartman>();


                if (arizaTanim.Id == 0)
                {
                    arizaTanim.ArizaTanimAdi = arizaTanim.ArizaTanimAdi.FirsToUpper();

                    _db.ArizaTanims.Add(arizaTanim);
                    _db.SaveChanges();

                    foreach (var item in seciliDepartmanSorumlular)
                    {
                        eklenecekArizaSorumluDeps.Add(new ArizaSorumluDepartman()
                        {
                            DepartmanId = item.DepartmanId,
                            SeciliMi = true,
                            ArizaTanimId = arizaTanim.Id
                        });
                    }
                    foreach (var item in seciliDepartmanAcilabilecekler)
                    {
                        eklenecekArizaAcilabilirDeps.Add(new ArizaAcilabilecekDepartman()
                        {
                            DepartmanId = item.DepartmanId,
                            SeciliMi = true,
                            ArizaTanimId = arizaTanim.Id
                        });
                    }
                }
                else
                {
                    var editItem = _db.ArizaTanims.Find(arizaTanim.Id);
                    editItem.ArizaTechizatGrubuId = arizaTanim.ArizaTechizatGrubuId;
                    editItem.ArizaTanimAdi = arizaTanim.ArizaTanimAdi.FirsToUpper();
                    editItem.ArizaTanimGrupId = arizaTanim.ArizaTanimGrupId;
                    editItem.OnemDerecesi = arizaTanim.OnemDerecesi;
                    _db.SaveChanges();

                    var kayitliSorumluListe = _db.ArizaSorumluDepartmen.Where(a => a.ArizaTanimId == arizaTanim.Id)
                        .ToList();
                    var kayitliAcilabilirListe = _db.ArizaAcilabilecekDepartmen.Where(a => a.ArizaTanimId == arizaTanim.Id)
                        .ToList();

                    foreach (var item in seciliDepartmanSorumlular)
                    {
                        if (kayitliSorumluListe.Count(a => a.DepartmanId == item.DepartmanId) == 0)
                        {
                            eklenecekArizaSorumluDeps.Add(new ArizaSorumluDepartman()
                            {
                                DepartmanId = item.DepartmanId,
                                SeciliMi = true,
                                ArizaTanimId = arizaTanim.Id
                            });
                        }
                    }

                    foreach (var item in kayitliSorumluListe)
                    {
                        if (seciliDepartmanSorumlular.Count(a => a.DepartmanId == item.DepartmanId) == 0)
                        {
                            silinecekArizaSorumluDeps.Add(item);
                        }
                    }


                    foreach (var item in seciliDepartmanAcilabilecekler)
                    {
                        if (kayitliAcilabilirListe.Count(a => a.DepartmanId == item.DepartmanId) == 0)
                        {
                            eklenecekArizaAcilabilirDeps.Add(new ArizaAcilabilecekDepartman()
                            {
                                DepartmanId = item.DepartmanId,
                                SeciliMi = true,
                                ArizaTanimId = arizaTanim.Id
                            });
                        }
                    }

                    foreach (var item in kayitliAcilabilirListe)
                    {
                        if (seciliDepartmanAcilabilecekler.Count(a => a.DepartmanId == item.DepartmanId) == 0)
                        {
                            silinecekArizaAcilabilirDeps.Add(item);
                        }
                    }
                }

                if (eklenecekArizaSorumluDeps.Any())
                {
                    _db.ArizaSorumluDepartmen.AddRange(eklenecekArizaSorumluDeps);
                    _db.SaveChanges();
                }
                if (silinecekArizaSorumluDeps.Any())
                {
                    _db.ArizaSorumluDepartmen.RemoveRange(silinecekArizaSorumluDeps);
                    _db.SaveChanges();
                }

                if (eklenecekArizaAcilabilirDeps.Any())
                {
                    _db.ArizaAcilabilecekDepartmen.AddRange(eklenecekArizaAcilabilirDeps);
                    _db.SaveChanges();
                }
                if (silinecekArizaAcilabilirDeps.Any())
                {
                    _db.ArizaAcilabilecekDepartmen.RemoveRange(silinecekArizaAcilabilirDeps);
                    _db.SaveChanges();
                }



                TempDataCreate(2);
            }

            return new JsonResult { Data = new { Id = arizaTanim.Id, state = state }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public ActionResult ArizaTanimAlternatifEkleDuzenle(TanimlamaArizaModel model)
        {
            var arizaId = model.ArizaTanim.Id;

            var arizaTanimArizaTechizatGrubu = model.ArizaTanimArizaTechizatGrubu;
            arizaTanimArizaTechizatGrubu.ArizaId = arizaId;
            var bilesenHeaderlar = model.DropBilesenHeader.Where(a => a.SeciliMi).ToList();

            bool SorunVarmi = !bilesenHeaderlar.Any();
            if (!SorunVarmi && arizaTanimArizaTechizatGrubu.Id == 0)
            {
                // mükerrer kayıt
                if (_db.ArizaTanimArizaTechizatGrubus.Any(a =>
                    a.ArizaId == arizaId &&
                    a.ArizaTechizatGrubuId == arizaTanimArizaTechizatGrubu.ArizaTechizatGrubuId))
                {


                    var bilesenler = _db.ArizaTanimGerekliBilesenHeaders.Where(a =>

                        a.ArizaTanimId == arizaId && a.ArizaTanimArizaTechizatGrupId != 0).ToList();

                    foreach (var ii in bilesenler)
                    {

                        foreach (var bheader in bilesenHeaderlar)
                        {
                            if (SorunVarmi)
                            {
                                break;
                            }
                            // TechizatTuruId olmalı
                            if (ii.BilesenHeaderId == bheader.IdInt)
                            {
                                SorunVarmi = true;
                            }
                        }

                        if (SorunVarmi)
                        {
                            break;
                        }
                    }

                }



            }


            if (!SorunVarmi)
            {
                if (arizaTanimArizaTechizatGrubu.Id == 0)
                {
                    _db.ArizaTanimArizaTechizatGrubus.Add(arizaTanimArizaTechizatGrubu);
                    _db.SaveChanges();

                }
                var gerekliTechizatTuruIdler = new List<int>();
                var gerekliBilesenler = new List<ItemValue>();

                foreach (var item in bilesenHeaderlar)
                {

                    gerekliTechizatTuruIdler.Add(item.IdInt);

                    foreach (var itemValue in item.ItemValues.Where(a => a.SeciliMi).ToList())
                    {
                        gerekliBilesenler.Add(itemValue);
                    }
                }

                var eklenecekArizaGerekliBilesenler = new List<ArizaTanimGerekliBilesenHeader>();
                var silinecekArizaGerekliBilesenler = new List<ArizaTanimGerekliBilesenHeader>();

                var eklenecekArizaTechizatTurus = new List<ArizaTanimGerekliTechizatTuru>();
                var silinecekArizaTechizatTurus = new List<ArizaTanimGerekliTechizatTuru>();
                var kayitliGerekliBilesenIdler =
                         _db.ArizaTanimGerekliBilesenHeaders.Where(a => a.ArizaTanimId == arizaId && a.ArizaTanimArizaTechizatGrupId == arizaTanimArizaTechizatGrubu.Id).ToList();
                var kayitliGerekliTechizatTurleri = _db.ArizaTanimGerekliTechizatTurus.Where(a => a.ArizaTanimId == arizaId && a.ArizaTanimArizaTechizatGrupId == arizaTanimArizaTechizatGrubu.Id).ToList();
                foreach (var i in gerekliTechizatTuruIdler)
                {
                    if (kayitliGerekliTechizatTurleri.Count(a => a.TechizatTuruId == i) == 0)
                    {
                        eklenecekArizaTechizatTurus.Add(new ArizaTanimGerekliTechizatTuru()
                        {
                            SeciliMi = true,

                            ArizaTanimId = arizaId,
                            TechizatTuruId = i,
                            ArizaTanimArizaTechizatGrupId = arizaTanimArizaTechizatGrubu.Id
                        });
                    }
                }

                foreach (var item in kayitliGerekliTechizatTurleri)
                {
                    if (gerekliTechizatTuruIdler.Count(a => a == item.TechizatTuruId) == 0)
                    {
                        silinecekArizaTechizatTurus.Add(item);
                    }
                }


                foreach (var tValue in gerekliBilesenler)
                {
                    var bilesenHeaderId = Convert.ToInt32(tValue.Id);
                    var techizatTuruId = tValue.IdInt;

                    if (kayitliGerekliBilesenIdler.Count(a => a.BilesenHeaderId == bilesenHeaderId) == 0)
                    {
                        eklenecekArizaGerekliBilesenler.Add(new ArizaTanimGerekliBilesenHeader()
                        {
                            SeciliMi = true,
                            BilesenHeaderId = bilesenHeaderId,
                            ArizaTanimId = arizaId,
                            TechizatTuruId = techizatTuruId,
                            ArizaTanimArizaTechizatGrupId = arizaTanimArizaTechizatGrubu.Id
                        });
                    }
                }

                foreach (var item in kayitliGerekliBilesenIdler)
                {
                    if (gerekliBilesenler.Count(a => a.Id == item.BilesenHeaderId.ToString()) == 0)
                    {
                        silinecekArizaGerekliBilesenler.Add(item);
                    }
                }



                if (eklenecekArizaGerekliBilesenler.Any())
                {
                    _db.ArizaTanimGerekliBilesenHeaders.AddRange(eklenecekArizaGerekliBilesenler);
                    _db.SaveChanges();
                }
                if (silinecekArizaGerekliBilesenler.Any())
                {
                    _db.ArizaTanimGerekliBilesenHeaders.RemoveRange(silinecekArizaGerekliBilesenler);
                    _db.SaveChanges();
                }


                if (eklenecekArizaTechizatTurus.Any())
                {
                    _db.ArizaTanimGerekliTechizatTurus.AddRange(eklenecekArizaTechizatTurus);
                    _db.SaveChanges();
                }
                if (silinecekArizaTechizatTurus.Any())
                {
                    _db.ArizaTanimGerekliTechizatTurus.RemoveRange(silinecekArizaTechizatTurus);
                    _db.SaveChanges();
                }

                TempDataCreate(2);
            }
            else
            {
                TempDataCustom(1, "Mükerrer kayıt yapmaya çalıştınız ya da seçim yapmadınız");
            }


            return RedirectToAction("ArizaTanimEkleDuzenle", "ArizaTemel", new { id = arizaId });
        }
        public ActionResult ArizaTanimSil(int id)
        {
            var item = _db.ArizaTanims.Find(id);
            var idd = item.ArizaTechizatGrubuId;
            /*
             *    ArizaTanimGerekliBilesenHeader
                    ArizaSorumluDepartman
                ArizaAcilabilecekDepartman
                    ArizaTanimGerekliTechizatTuru
             */
            var ArizaTanimGerekliBilesenHeaders = _db.ArizaTanimGerekliBilesenHeaders.Where(a => a.ArizaTanimId == id).ToList();
            var ArizaSorumluDepartmans = _db.ArizaSorumluDepartmen.Where(a => a.ArizaTanimId == id).ToList();
            var ArizaAcilabilecekDepartmans = _db.ArizaAcilabilecekDepartmen.Where(a => a.ArizaTanimId == id).ToList();
            var ArizaTanimGerekliTechizatTurus = _db.ArizaTanimGerekliTechizatTurus.Where(a => a.ArizaTanimId == id).ToList();

            _db.ArizaTanimGerekliBilesenHeaders.RemoveRange(ArizaTanimGerekliBilesenHeaders);
            _db.SaveChanges();
            _db.ArizaSorumluDepartmen.RemoveRange(ArizaSorumluDepartmans);
            _db.SaveChanges();
            _db.ArizaAcilabilecekDepartmen.RemoveRange(ArizaAcilabilecekDepartmans);
            _db.SaveChanges();
            _db.ArizaTanimGerekliTechizatTurus.RemoveRange(ArizaTanimGerekliTechizatTurus);
            _db.SaveChanges();

            _db.ArizaTanims.Remove(item);
            _db.SaveChanges();
            TempDataCreate(3);
            return RedirectToAction("ArizaTanimListe", "ArizaTemel", new { id = idd });

        }


        public ActionResult ArizaTanimAlternatifSil(int id)
        {
            var item = _db.ArizaTanimArizaTechizatGrubus.Find(id);
            var idd = item.ArizaId;

            var ArizaTanimGerekliTechizatTurus = _db.ArizaTanimGerekliTechizatTurus.Where(a => a.ArizaTanimArizaTechizatGrupId == id).ToList();
            var ArizaTanimGerekliBilesenHeaders = _db.ArizaTanimGerekliBilesenHeaders.Where(a => a.ArizaTanimArizaTechizatGrupId == id).ToList();

            _db.ArizaTanimGerekliBilesenHeaders.RemoveRange(ArizaTanimGerekliBilesenHeaders);
            _db.SaveChanges();
            _db.ArizaTanimGerekliTechizatTurus.RemoveRange(ArizaTanimGerekliTechizatTurus);
            _db.SaveChanges();
            _db.ArizaTanimArizaTechizatGrubus.Remove(item);
            _db.SaveChanges();
            TempDataCreate(3);
            return RedirectToAction("ArizaTanimEkleDuzenle", "ArizaTemel", new { id = idd });

        }


        [HttpPost]
        public PartialViewResult ArizaTanimAra(string id)
        {
            var model = new TanimlamaArizaModel();
            model.DropItems = new List<DropItem>();
            var squery = string.Format(
                "SELECT        TOP (50) dbo.ArizaTanim.Id, dbo.ArizaTanimGrup.ArizaTanimGrupAdi, dbo.ArizaTanim.ArizaTanimAdi FROM            dbo.ArizaTanim INNER JOIN                         dbo.ArizaTanimGrup ON dbo.ArizaTanim.ArizaTanimGrupId  = dbo.ArizaTanimGrup.Id WHERE        (dbo.ArizaTanim.ArizaTanimAdi LIKE N'%{0}%') ORDER BY dbo.ArizaTanimGrup.ArizaTanimGrupAdi, dbo.ArizaTanim.ArizaTanimAdi",
                id);

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

            foreach (var aa in result)
            {
                var lst = aa.Values.ToList();

                model.DropItems.Add(new DropItem()
                {
                    Id = lst[0]?.ToString(),
                    Tanim1 = lst[1]?.ToString(),
                    Tanim2 = lst[2]?.ToString()
                });
            }
            return PartialView("_ArizaTanimAra", model);
        }


        #region Arıza Çözüm

        [HttpPost]
        public ActionResult ArizaCozumEkleDuzenle(TanimlamaArizaModel model)
        {
            var arizaCozum = model.ArizaTanimCozum;

            if (string.IsNullOrEmpty(arizaCozum.ArizaCozumAdi))
            {
                TempDataCreate(4);
            }
            else
            {
                if (arizaCozum.Id == 0)
                {
                    _db.ArizaTanimCozums.Add(arizaCozum);
                    _db.SaveChanges();
                }
                else
                {
                    var editItem = _db.ArizaTanimCozums.Find(arizaCozum.Id);
                    editItem.ArizaCozumAdi = arizaCozum.ArizaCozumAdi;
                    _db.SaveChanges();
                }
                TempDataCreate(2);
            }
            return RedirectToAction("ArizaTanimEkleDuzenle", "ArizaTemel", new { id = arizaCozum.ArizaTanimId });
        }


        public ActionResult ArizaCozumSil(int id)
        {
            var editItem = _db.ArizaTanimCozums.Find(id);
            var idd = editItem.ArizaTanimId;
            _db.ArizaTanimCozums.Remove(editItem);
            _db.SaveChanges();
            TempDataCreate(3);
            return RedirectToAction("ArizaTanimEkleDuzenle", "ArizaTemel", new { id = idd });
        }

        #endregion



        #endregion

    
    }
}