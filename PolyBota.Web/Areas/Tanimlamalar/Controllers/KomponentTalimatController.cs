using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Drawing;
using PolyBota.DAL;
using PolyBota.Entities;
using PolyBota.Web.Areas.Tanimlamalar.Models;
using PolyBota.Web.Controllers;

namespace PolyBota.Web.Areas.Tanimlamalar.Controllers
{
    public class KomponentTalimatController : BaseController
    {
        // GET: Tanimlamalar/KomponentTalimat

        public List<DropItem> PeriyotlariGetir()
        {
            var lst = new List<DropItem>();

            var gruplar = _db.PeriyotTipiTanims.AsNoTracking().OrderBy(a => a.PeriyotTipiAdi).ToList();
            var itemlar = _db.PeriyotTanims.AsNoTracking().OrderBy(a => a.PeriyotTanimAdi).ToList();

            foreach (var item in itemlar)
            {
                var str = "";
                if (gruplar.Any(a => a.Id == item.PeriyotTipi))
                {
                    str = gruplar.First(a => a.Id == item.PeriyotTipi).PeriyotTipiAdi;

                    str = string.Format("{0} - {1} >> {2} Hafta", str, item.PeriyotTanimAdi, item.PeriyotDonemi);

                    lst.Add(new DropItem()
                    {
                        Tanim = str,
                        Id = item.Id.ToString()
                    });
                }
            }

            return lst.OrderBy(a => a.Tanim).ToList();
        }
        public List<DropItem> KomponentleriGetir()
        {
            var lst = new List<DropItem>();

            var gruplar = _db.KomponentTanimGrups.AsNoTracking().OrderBy(a => a.KomponentTanimGrupAdi).ToList();
            var itemlar = _db.KomponentTanims.AsNoTracking().OrderBy(a => a.KomponentTanimAdi).ToList();

            foreach (var item in itemlar)
            {
                var str = "";
                if (gruplar.Any(a => a.Id == item.KomponentTanimGrupId))
                {
                    str = gruplar.First(a => a.Id == item.KomponentTanimGrupId).KomponentTanimGrupAdi;

                    str = string.Format("{0} >> {1}", str, item.KomponentTanimAdi);

                    lst.Add(new DropItem()
                    {
                        Tanim = str,
                        Id = item.Id.ToString()
                    });
                }
            }

            return lst.OrderBy(a => a.Tanim).ToList();
        }


        public ActionResult KomponentTalimatListe()
        {
            var model = new TanimlamaKompTalimatModel()
            {
                KomponentTalimatGrups = _db.KomponentTalimatGrups.OrderBy(a => a.KomponentTalimatGrupAdi).ToList(),
                DropPeriyotlar = PeriyotlariGetir(),
                DropKomponentler = KomponentleriGetir(),
            };
            return View(model);
        }

        public ActionResult KomponentTalimatGrupDetay(int id = 0)
        {
            var model = new TanimlamaKompTalimatModel()
            {
                DropPeriyotlar = PeriyotlariGetir(),
                DropKomponentler = KomponentleriGetir(),
                DropDepartmanlar = new List<DropItem>()
             
            };
            var DropDepartmanlar = DropDepartmanKirilimlariGetir();
            if (id != 0)
            {
                model.KomponentTalimatGrup = _db.KomponentTalimatGrups.Find(id);

                model.KomponentTalimats = _db.KomponentTalimats.Where(a => a.KomponentTalimatGrupId == id).ToList();

                var idler = model.KomponentTalimats.Select(a => a.TalimatTanimId).Distinct().ToList();

                foreach (var i in idler)
                {
                    model.TalimatTanims.Add(_db.TalimatTanims.Find(i));
                }

                //  model.TalimatTanims = model.TalimatTanims.OrderBy(a => a.TalimatAciklama).ToList();

                model.TalimatTanims = _db.TalimatTanims.OrderBy(a => a.TalimatAciklama).ToList();
                model.TalimatGrupTanims = _db.TalimatGrupTanims.OrderBy(a => a.TalimatGrupTanimAdi).ToList();


                var lst = new List<KomponentTalimat>();
                foreach (var item in model.TalimatTanims)
                {
                    lst.AddRange(model.KomponentTalimats.Where(a => a.TalimatTanimId == item.Id));
                }


                model.KomponentTalimats = lst;

                var kayitliDepts = _db.KomponentTalimatGrupDepartmen.Where(a => a.KomponentTalimatGrupId == id)
                    .ToList();
                var kayitliBilgiDepts = _db.KomponentTalimatGrupBilgiDepartmen.Where(a => a.KomponentTalimatGrupId == id)
                    .ToList();
                foreach (var dropItem in DropDepartmanlar)
                {
                    var secilimi = kayitliDepts.Any(a => a.DepartmanId == dropItem.IdInt);
                    model.DropDepartmanlar.Add(new DropItem()
                    {
                        Id = dropItem.Id,
                        SeciliMi = secilimi,
                        IdInt = dropItem.IdInt,
                        Tanim = dropItem.Tanim
                    });

                }

                foreach (var dropItem in DropDepartmanlar)
                {


                    var secilimi = kayitliBilgiDepts.Any(a => a.DepartmanId == dropItem.IdInt);
                    model.DropBilgiDepartmanlar.Add(new DropItem()
                    {
                        Id = dropItem.Id,
                        SeciliMi = secilimi,
                        IdInt = dropItem.IdInt,
                        Tanim = dropItem.Tanim
                    });
                }
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult KomponentTalimatGrupDetay(TanimlamaKompTalimatModel model)
        {
            var grup = model.KomponentTalimatGrup;

            if (grup.KomponentTalimatGrupAdi == null)
            {
                TempDataCreate(4);
            }
            else
            {
                if (grup.Id == 0)
                {
                    var yeniItem = new KomponentTalimatGrup()
                    {
                        KomponentTalimatGrupAdi = grup.KomponentTalimatGrupAdi,

                        PeriyotTanimId = grup.PeriyotTanimId,
                    };
                    _db.KomponentTalimatGrups.Add(yeniItem);
                    _db.SaveChanges();

                    grup.Id = yeniItem.Id;
                }
                else
                {
                    var editItem = _db.KomponentTalimatGrups.Find(grup.Id);
                    editItem.KomponentTalimatGrupAdi = grup.KomponentTalimatGrupAdi;

                    editItem.PeriyotTanimId = grup.PeriyotTanimId;
                    _db.SaveChanges();

                }

                var eklenecekDepartmans = new List<KomponentTalimatGrupDepartman>();
                var silinecekDepartmans = new List<KomponentTalimatGrupDepartman>();
                var seciliDepartmanlar =
                    model.DropDepartmanlar.Where(a => a.SeciliMi).ToList().Select(a => a.IdInt).ToList();

                var eklenecekDepartmansBilgi = new List<KomponentTalimatGrupBilgiDepartman>();
                var silinecekDepartmansBilgi = new List<KomponentTalimatGrupBilgiDepartman>();
                var seciliDepartmanlarBilgi =
                    model.DropBilgiDepartmanlar.Where(a => a.SeciliMi).ToList().Select(a => a.IdInt).ToList();


                var kayitliKompDepartmans = _db.KomponentTalimatGrupDepartmen
                    .Where(a => a.KomponentTalimatGrupId == grup.Id).ToList();
                var kayitliKompDepartmansBilgi = _db.KomponentTalimatGrupBilgiDepartmen
                    .Where(a => a.KomponentTalimatGrupId == grup.Id).ToList();

                #region Departman Sorumlu

                foreach (var item in seciliDepartmanlar)
                {
                    if (kayitliKompDepartmans.Count(a => a.DepartmanId == item) == 0)
                    {
                        eklenecekDepartmans.Add(new KomponentTalimatGrupDepartman()
                        {
                            DepartmanId = item,
                            KomponentTalimatGrupId = grup.Id
                        });
                    }
                }
                foreach (var item in kayitliKompDepartmans)
                {
                    if (seciliDepartmanlar.Count(a => a == item.DepartmanId) == 0)
                    {
                        silinecekDepartmans.Add(item);
                    }
                }

                if (eklenecekDepartmans.Any())
                {
                    _db.KomponentTalimatGrupDepartmen.AddRange(eklenecekDepartmans);
                    _db.SaveChanges();

                }

                if (silinecekDepartmans.Any())
                {
                    _db.KomponentTalimatGrupDepartmen.RemoveRange(silinecekDepartmans);
                    _db.SaveChanges();

                }

                #endregion


                #region Departman Bilgi

                foreach (var item in seciliDepartmanlarBilgi)
                {
                    if (kayitliKompDepartmansBilgi.Count(a => a.DepartmanId == item) == 0)
                    {
                        eklenecekDepartmansBilgi.Add(new KomponentTalimatGrupBilgiDepartman()
                        {
                            DepartmanId = item,
                            KomponentTalimatGrupId = grup.Id
                        });
                    }
                }
                foreach (var item in kayitliKompDepartmansBilgi)
                {
                    if (seciliDepartmanlarBilgi.Count(a => a == item.DepartmanId) == 0)
                    {
                        silinecekDepartmansBilgi.Add(item);
                    }
                }

                if (eklenecekDepartmansBilgi.Any())
                {
                    _db.KomponentTalimatGrupBilgiDepartmen.AddRange(eklenecekDepartmansBilgi);
                    _db.SaveChanges();

                }

                if (silinecekDepartmansBilgi.Any())
                {
                    _db.KomponentTalimatGrupBilgiDepartmen.RemoveRange(silinecekDepartmansBilgi);
                    _db.SaveChanges();

                }

                #endregion
                TempDataCreate(2);
            }

            return RedirectToAction("KomponentTalimatGrupDetay", "KomponentTalimat", new { id = grup.Id });
        }




        [HttpPost]
        public ActionResult KomponentTalimatTopluEkle(TanimlamaKompTalimatModel model)
        {
            var grup = model.KomponentTalimatGrup;

            var seciliListe = model.TalimatTanims.Where(a => a.SeciliMi).Select(a => a.Id).ToList();

            var kayitliListe = _db.KomponentTalimats.Where(a => a.KomponentTalimatGrupId == grup.Id)
                .Select(a => a.TalimatTanimId).ToList();
            var eklenecekIdler = new List<int>();

            foreach (var i in seciliListe)
            {
                if (kayitliListe.Count(a => a == i) == 0)
                {
                    eklenecekIdler.Add(i);
                }
            }

            if (eklenecekIdler.Any())
            {
                var eklenecekListe = eklenecekIdler.Select(a => new KomponentTalimat()
                { KomponentTalimatGrupId = grup.Id, TalimatTanimId = a }).ToList();

                _db.KomponentTalimats.AddRange(eklenecekListe);
                _db.SaveChanges();
                TempDataCreate(2);
            }


            return RedirectToAction("KomponentTalimatGrupDetay", "KomponentTalimat", new { id = grup.Id });
        }

        public PartialViewResult KomponentTalimatEkleDuzenle(int id = 0, int talimatId = 0)
        {
            var model = new TanimlamaKompTalimatModel()
            {
                TalimatGrupTanims = _db.TalimatGrupTanims.OrderBy(a => a.TalimatGrupTanimAdi).ToList(),
                TalimatTanims = _db.TalimatTanims.OrderBy(a => a.TalimatAciklama).ToList(),

            };

            if (talimatId != 0)
            {
                model.KomponentTalimat = _db.KomponentTalimats.Find(talimatId);
            }
            else
            {
                model.KomponentTalimat = new KomponentTalimat() { KomponentTalimatGrupId = id };
            }
            return PartialView("_KomponentTalimatEkleDuzenle", model);
        }

        [HttpPost]
        public ActionResult KomponentTalimatEkleDuzenle(TanimlamaKompTalimatModel model)
        {
            var talimat = model.KomponentTalimat;
            if (talimat.TalimatTanimId == 0)
            {
                TempDataCreate(4);
            }
            else
            {
                if (talimat.Id == 0)
                {
                    var yeniItem = new KomponentTalimat()
                    {
                        KomponentTalimatGrupId = talimat.KomponentTalimatGrupId,
                        TalimatTanimId = talimat.TalimatTanimId
                    };
                    _db.KomponentTalimats.Add(yeniItem);
                    _db.SaveChanges();
                }
                else
                {
                    var editItem = _db.KomponentTalimats.Find(talimat.Id);
                    editItem.TalimatTanimId = talimat.TalimatTanimId;
                    _db.SaveChanges();
                }
            }



            return RedirectToAction("KomponentTalimatGrupDetay", "KomponentTalimat",
                new { id = talimat.KomponentTalimatGrupId });


        }

        public ActionResult KomponentTalimatSil(int id)
        {

            var item = _db.KomponentTalimats.Find(id);

            var idd = item.KomponentTalimatGrupId;

            _db.KomponentTalimats.Remove(item);
            _db.SaveChanges();
            TempDataCreate(3);
            return RedirectToAction("KomponentTalimatGrupDetay", "KomponentTalimat", new { id = idd });
        }
        public ActionResult KomponentTalimatGrupSil(int id)
        {


            var item = _db.KomponentTalimatGrups.Find(id);
            _db.KomponentTalimatGrups.Remove(item);
            _db.SaveChanges();
            var items = _db.KomponentTalimats.Where(a=> a.KomponentTalimatGrupId== id);
            _db.KomponentTalimats.RemoveRange(items);
            _db.SaveChanges();
            TempDataCreate(3);
            return RedirectToAction("KomponentTalimatListe");
        }
    }
}