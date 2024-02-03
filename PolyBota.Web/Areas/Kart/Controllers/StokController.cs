using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using ClosedXML.Excel;
using PolyBota.DAL;
using PolyBota.Entities;
using PolyBota.Web.Areas.Kart.Models;
using PolyBota.Web.Controllers;

namespace PolyBota.Web.Areas.Kart.Controllers
{
    public class StokController : BaseController
    {

        public MemoryStream TaslakExcelOlustur()
        {
            XLWorkbook wb = new XLWorkbook();
            wb.Worksheets.Add("Stok Kartı", 1);
            wb.Worksheets.Add("Bölümler", 2);
            wb.Worksheets.Add("Stok Grupları", 3);
            wb.Worksheets.Add("Sicil Grupları", 4);
            var wsStokKarti = wb.Worksheet(1);
            var wsBolum = wb.Worksheet(2);
            var wsStokGrup = wb.Worksheet(3);
            var wsStokSicilGrup = wb.Worksheet(4);


            var bolumler = DropBolumKirilimlariGetir();

            int i = 1;

            wsBolum.Cell(i, 1).Value = "Bölüm Id";
            wsBolum.Cell(i, 2).Value = "Bölüm Adı";
            wsStokGrup.Cell(i, 1).Value = "Stok Grup Id";
            wsStokGrup.Cell(i, 2).Value = "Stok Grup Adı";

            wsStokGrup.Cell(i, 1).Value = "Sicil Özellik Id";
            wsStokGrup.Cell(i, 2).Value = "Sicil Özellik Adı";

            wsStokKarti.Cell(i, 1).Value = "Stok Grup Id";
            wsStokKarti.Cell(i, 2).Value = "Bölüm Id";
            wsStokKarti.Cell(i, 3).Value = "Stok Adı";

            foreach (var bolum in bolumler)
            {
                i++;
                wsBolum.Cell(i, 1).Value = bolum.IdInt;
                wsBolum.Cell(i, 2).Value = bolum.Tanim;

            }

            var kompTanimlar = DropKomponentTanimlariGetir();
            i = 1;

            foreach (var item in kompTanimlar)
            {
                i++;
                wsStokGrup.Cell(i, 1).Value = item.IdInt;
                wsStokGrup.Cell(i, 2).Value = item.Tanim;
            }

            var sicilTanimlar = DropSicilTanimlariGetir();
            i = 1;

            foreach (var item in sicilTanimlar)
            {
                i++;
                wsStokSicilGrup.Cell(i, 1).Value = item.IdInt;
                wsStokSicilGrup.Cell(i, 2).Value = item.Tanim;
            }

            i = 3;
            foreach (var item in sicilTanimlar)
            {
                i++;
                wsStokKarti.Cell(1, i).Value = item.Tanim;

            }

            MemoryStream excelStream = new MemoryStream();
            wb.SaveAs(excelStream);
            excelStream.Position = 0;



            return excelStream;

        }
        #region Metotlar

        public KartStokModel KartStokModeliBaseOlustur(int id)
        {
            var model = new KartStokModel()
            {
                StokKart = id == 0 ? new StokKart() : _db.StokKarts.Find(id),
                DropSicilOzelliks = new List<DropItem>(),
                SicilOzellikHeaderTanims = _db.SicilOzellikHeaderTanims.OrderBy(a => a.SicilOzellikHeaderTanimAdi).ToList(),
                SicilOzellikTanims = _db.SicilOzellikTanims.OrderBy(a => a.SicilOzellikTanimAdi).ToList(),
                
                Medya = new Medya() { BagliOlduguId = id },
                Medyas = _db.Medyas.Where(a => a.BagliOlduguTip == 0 && a.BagliOlduguId == id).OrderBy(a => a.MedyaAdi).ToList(),
                KomponentTanimGrups = _db.KomponentTanimGrups.OrderBy(a => a.KomponentTanimGrupAdi).ToList(),
                KomponentTanims = _db.KomponentTanims.OrderBy(a => a.KomponentTanimAdi).ToList()
            };





            //foreach (var item in model.SicilOzellikTanims)
            //{
            //    if (model.StokKartSicilOzelliks.Any(a => a.SicilOzellikId == item.Id))
            //    {
            //        var str = model.StokKartSicilOzelliks.First(a => a.SicilOzellikId == item.Id).OzellikDegeri;
            //        item.BaseValue = str;
            //        item.SeciliMi = true;
            //    }
            //}


            var bolums = _db.Bolums.OrderBy(a => a.BolumAdi).ToList();


            foreach (var bolum in bolums)
            {

                Departmans = new List<Departman>();

                DepartmanGetir(bolum.DepartmanId);

                Departmans.Reverse();

                var str = "";

                foreach (var item in Departmans)
                {
                    str += item.DepartmanAdi + " >> ";
                }

                Bolums = new List<Bolum>();
                BolumGetir(bolum.Id);

                Bolums.Reverse();

                foreach (var item in Bolums)
                {
                    str += item.BolumAdi;

                    if (item != Bolums.Last())
                    {
                        str += " >> ";
                    }
                }

                model.DropItems.Add(new DropItem()
                {
                    Id = bolum.Id.ToString(),
                    Tanim = str
                });
            }

            model.DropItems = model.DropItems.OrderBy(a => a.Tanim).ToList();

            return model;
        }


        #endregion


        // GET: Kart/Stok
        public ActionResult StokKartListe(int CurrentPage = 1, int PageShowCount = 50, int katId = 0)
        {

            var KomponentTanimGrups = _db.KomponentTanimGrups.OrderBy(a => a.KomponentTanimGrupAdi).ToList();
            var KomponentTanims = _db.KomponentTanims.OrderBy(a => a.KomponentTanimAdi).ToList();

            if (katId == 0)
            {
                katId = KomponentTanimGrups.First().Id;
            }

            var model = new KartStokModel()
            {
                KatId = katId,
                KomponentTanimGrups = KomponentTanimGrups,
                KomponentTanims = KomponentTanims
            };

            var itemIdler = _db.StokKarts.Where(a => a.KomponentTanimGrupId == katId).OrderBy(a => a.StokTanimAdi)
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

            foreach (var i in PageListBase.DataLists)
            {
                model.StokKarts.Add(_db.StokKarts.Find(i));
            }

            model.Bolums = _db.Bolums.ToList();
            return View(model);
        }

        public ActionResult StokKartDetay(int id = 0)
        {


            return View(KartStokModeliBaseOlustur(id));
        }

        #region Stok Kart Toplu Yükleme
        public ActionResult StokKartTaslak()
        {
            return View();

        }
        [HttpPost]
        public ActionResult StokKartTaslak(HttpPostedFileBase file)
        {

            bool sorunVarmi = false;
            if (file != null)
            {
                if (file.ContentLength < 1)
                {
                    sorunVarmi = true;
                }
                else
                {
                    sorunVarmi = !file.FileName.Contains(".xls") || !file.FileName.Contains(".xlsx");
                }
            }
            else
            {
                sorunVarmi = true;
            }

            if (sorunVarmi)
            {
                TempDataCustom(1, "Excel Formatında Yükleme Yapın");
            }
            else
            {

                var dropSicilOzellikler = DropSicilTanimlariGetir();
                var kompTanimGruplar = DropKomponentTanimlariGetir();
                var bolumler = DropBolumKirilimlariGetir();

                Guid guid = Guid.NewGuid();
                var path = "~/Upload/ItFiles/" + guid.ToString() + file.FileName;

                file.SaveAs(Server.MapPath(path));
                int olumluKayitSay = 0;
                using (var excelWorkbook = new XLWorkbook(Server.MapPath(path)))
                {
                    var nonEmptyDataRows = excelWorkbook.Worksheet(1).RowsUsed().ToList();

                    int excelSutunSayisi = nonEmptyDataRows[0].Cells().Count();

                    if ((dropSicilOzellikler.Count + 3) == excelSutunSayisi)
                    {
                        foreach (var dataRow in nonEmptyDataRows)
                        {
                            var strStokGrupId = dataRow.Cell(1).Value.ToString();
                            var strBolumId = dataRow.Cell(2).Value.ToString();
                            var strStokAdi = dataRow.Cell(3).Value.ToString();
                            int k = 3;

                            if (!string.IsNullOrEmpty(strStokGrupId) &&
                                !string.IsNullOrEmpty(strBolumId) &&
                                !string.IsNullOrEmpty(strStokAdi)
                                )
                            {

                                var temelBilgiSorunVarmi = false;

                                int StokGrupId = 0;
                                int BolumId = 0;

                                try
                                {
                                    StokGrupId = Convert.ToInt32(strStokGrupId);
                                    BolumId = Convert.ToInt32(strBolumId);

                                }
                                catch (Exception e)
                                {
                                    temelBilgiSorunVarmi = true;
                                }

                              

                                if (!temelBilgiSorunVarmi)
                                {
                                    if (kompTanimGruplar.Any(a => a.IdInt == StokGrupId))
                                    {
                                        var secilenKomp = kompTanimGruplar.First(a => a.IdInt == StokGrupId);


                                        if (bolumler.Any(a => a.IdInt == BolumId))
                                        {
                                            var secilenBolum = bolumler.First(a => a.IdInt == BolumId);

                                            var yeniStok = new StokKart()
                                            {
                                                KomponentTanimId = secilenKomp.IdInt,
                                                KomponentTanimGrupId =secilenKomp.IdInt2,
                                                StokTanimAdi = strStokAdi,
                                                SablonGrupId = 0,
                                                ImageUrl = null,
                                                StokKodu = "",
                                                BolumId = secilenBolum.IdInt,
                                                SeriNo = "",
                                                PotaId = "",
                                                LogoId = "",
                                                SicilNo = ""
                                            };
                                            _db.StokKarts.Add(yeniStok);
                                            _db.SaveChanges();
                                            /*
                                            var stokKartSicilOzellikleri = new List<StokKartSicilOzellik>();
                                            for (int i = 0; i < dropSicilOzellikler.Count; i++)
                                            {
                                                k++;

                                                var sicilOzellik = dropSicilOzellikler[i];

                                                var hucreDegeri = dataRow.Cell(k).Value.ToString();

                                                if (!string.IsNullOrEmpty(hucreDegeri))
                                                {
                                                    stokKartSicilOzellikleri.Add(new StokKartSicilOzellik()
                                                    {
                                                        SicilOzellikTanimAdi = null,
                                                        SicilOzellikId = sicilOzellik.IdInt,
                                                        StokKartId = yeniStok.Id,
                                                        OzellikDegeri = hucreDegeri
                                                    });
                                                }

                                            }

                                            olumluKayitSay++;
                                            if (stokKartSicilOzellikleri.Any())
                                            {
                                                _db.StokKartSicilOzelliks.AddRange(stokKartSicilOzellikleri);
                                                _db.SaveChanges();
                                            }
                                            */
                                        }

                                    }

                                }

                            }


                        }
                    }
                    else
                    {
                        TempDataCustom(1, "Excel Formatınız yanlış");
                    }

                }

                if (olumluKayitSay>0)
                {
                    TempDataCustom(0,string.Format("Toplam {0} Kayıt Yapılmıştır",olumluKayitSay));
                }
                else
                {

                    TempDataCustom(1, "Kayıt İşlemi Yapılamadı");
                }
            }
            return RedirectToAction("StokKartTaslak");
        }
        public ActionResult StokKartTaslakExcel()
        {
            var excelStream = TaslakExcelOlustur();


            return File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Stok Kart Taslak.xlsx");


        }

        #endregion







        public PartialViewResult StokKartTabGoster(int id, int tabId, int digerId = 0)
        {
            var stokKart = _db.StokKarts.Find(id);

            var model = new KartStokModel()
            {
                tabId = tabId,
                StokKart = stokKart,
            };
            if (tabId == 1)
            {
                model = KartStokModeliBaseOlustur(id);
                model.tabId = 1;



                return PartialView("_SokKartSicilBilgi", model);
            }
            if (tabId == 2)
            {
                //bileşen

                model.StokKartBilesenCinsis =
                    _db.StokKartBilesenCinsis.Where(a => a.StokKartId == stokKart.Id).ToList();

                model.BilesenCinsiGrups = _db.BilesenCinsiGrups.OrderBy(a => a.BilesenCinsiGrupAdi).ToList();
                model.BilesenCinsis = _db.BilesenCinsis.OrderBy(a => a.BilesenCinsiAdi).ToList();
                return PartialView("_StokKartBilesenCinsi", model);


                if (digerId == 0)
                {
                    model.SablonGrups = _db.SablonGrups.OrderBy(a => a.SablonGrupAdi).ToList();
                    if (stokKart.SablonGrupId != 0)
                    {
                        model.SablonGrupItems =
                            _db.SablonGrupItems.Where(a => a.SablonGrupId == stokKart.SablonGrupId).ToList();

                        var idler = model.SablonGrupItems.Select(a => a.KomponentTanimId).Distinct().ToList();

                        model.KomponentTanims = _db.KomponentTanims.Where(a => idler.Any(b => b == a.Id)).ToList();

                    }

                }
                else
                {
                    // Sablon Grup Item 

                    var sablonGrupItem = _db.SablonGrupItems.Find(digerId);


                    var uygunstokKartlar = _db.StokKarts.Where(a => a.KomponentTanimId == sablonGrupItem.KomponentTanimId)
                        .OrderBy(a => a.StokTanimAdi).ToList();
                    model.StokKarts = uygunstokKartlar;


                    model.SablonGrupItem = sablonGrupItem;
                    model.KomponentTanim = _db.KomponentTanims.Find(sablonGrupItem.KomponentTanimId);

                    if (_db.StokKartKomponents.Any(a => a.StokKartId == stokKart.Id && a.KomponentTanimId == sablonGrupItem.KomponentTanimId))
                    {
                        model.StokKartKomponent = _db.StokKartKomponents.First(a =>
                            a.StokKartId == stokKart.Id && a.KomponentTanimId == sablonGrupItem.KomponentTanimId);

                    }
                    else
                    {
                        model.StokKartKomponent = new StokKartKomponent()
                        {
                            KomponentTanimId = sablonGrupItem.KomponentTanimId,
                            StokKartId = stokKart.Id
                        };
                    }
                    return PartialView("_StokKartSablonGrupItem", model);
                }




            }
            if (tabId == 3)
            {
                // periyodik Bakım
                if (digerId == 0)
                {
                    model.StokKartOperasyon = new StokKartOperasyon() { StokKartId = stokKart.Id, PlanlananTarih = DateTime.Now };

                    model.StokKartOperasyons = _db.StokKartOperasyons.Where(a=>a.StokKartId==stokKart.Id).OrderByDescending(a => a.GerceklesenTarih).ToList();
                    model.KomponentTalimatGrups =
                        _db.KomponentTalimatGrups.OrderBy(a => a.KomponentTalimatGrupAdi).ToList();

                    
                    var lst = HaftaGetir(DateTime.Now);
                    model.BaslangicHafta = lst[0];
                    model.GunumuzHaftfa = lst[1];
                    model.BitisHafta = lst[2];
                }

                return PartialView("_StokKartPeriyodikBakım", model);
            }

            if (tabId == 4)
            {
                // stok kart hatırlatma

                model.StokKartHatirlatmas = _db.StokKartHatirlatmas.Where(a => a.StokKartId == id)
                    .OrderByDescending(a => a.HatirlatmaTarihi).ToList();
                return PartialView("_StokKartHatirlatma", model);
            }



            return PartialView("");
        }

        [HttpPost]
        public JsonResult StokKartDetayKaydet(int id, KartStokModel model)
        {
            int state = 0;//0-bilgi eksik, 1-işlem okey, 2-redirect


            int tabId = id;

            var stokKart = model.StokKart;

            if (tabId == 10)
            {
                tabId = 1;

                // stok kart temel bilgiler

                if (stokKart.StokTanimAdi != null && stokKart.KomponentTanimId != 0)
                {
                    state = 2;

                    var komp = _db.KomponentTanims.Find(stokKart.KomponentTanimId);

                    if (stokKart.Id == 0)
                    {
                        var yeniItem = new StokKart()
                        {
                            StokTanimAdi = stokKart.StokTanimAdi,
                            KomponentTanimId = komp.Id,
                            KomponentTanimGrupId = komp.KomponentTanimGrupId,
                            PotaId = stokKart.PotaId,
                            LogoId = stokKart.LogoId,
                            SeriNo = stokKart.SeriNo,
                            StokKodu = stokKart.StokKodu,
                            SicilNo = stokKart.SicilNo,
                            LotNo = stokKart.LotNo
                        };
                        _db.StokKarts.Add(yeniItem);
                        _db.SaveChanges();
                        stokKart.Id = yeniItem.Id;
                    }
                    else
                    {
                        var editItem = _db.StokKarts.Find(stokKart.Id);
                        editItem.StokTanimAdi = stokKart.StokTanimAdi;
                        editItem.KomponentTanimId = komp.Id;
                        editItem.KomponentTanimGrupId = komp.KomponentTanimGrupId;
                        editItem.PotaId = stokKart.PotaId;
                        editItem.LogoId = stokKart.LogoId;
                        editItem.SeriNo = stokKart.SeriNo;
                        editItem.StokKodu = stokKart.StokKodu;
                        editItem.SicilNo = stokKart.SicilNo;
                        editItem.LotNo = stokKart.LotNo;
                        _db.SaveChanges();
                    }
                    TempDataCreate(2);
                }
            }

            if (tabId == 11)
            {
                // Sicil Özel Bilgiler

                /*
                var lst = model.SicilOzellikTanims.Where(a => a.SeciliMi).ToList();

                var kayitliListte = _db.StokKartSicilOzelliks.Where(a => a.StokKartId == stokKart.Id).ToList();

                var silinecekListe = new List<StokKartSicilOzellik>();

                foreach (var item in kayitliListte)
                {
                    if (lst.Count(a => a.Id == item.SicilOzellikId) == 0)
                    {
                        silinecekListe.Add(item);
                    }
                }

                _db.StokKartSicilOzelliks.RemoveRange(silinecekListe);
                _db.SaveChanges();
                
                foreach (var item in lst)
                {
                    if (kayitliListte.Any(a => a.SicilOzellikId == item.Id))
                    {

                        var editItem = _db.StokKartSicilOzelliks.First(a =>
                            a.StokKartId == stokKart.Id && a.SicilOzellikId == item.Id);

                        editItem.OzellikDegeri = item.BaseValue;
                        _db.SaveChanges();
                    }
                    else
                    {
                        var yeniItem = new StokKartSicilOzellik()
                        {
                            StokKartId = stokKart.Id,
                            SicilOzellikId = item.Id,
                            OzellikDegeri = item.BaseValue
                        };
                        _db.StokKartSicilOzelliks.Add(yeniItem);
                        _db.SaveChanges();
                    }
                }
*/
                state = 2;
                TempDataCreate(2);
            }

            if (tabId == 12)
            {
                var editStok = _db.StokKarts.Find(stokKart.Id);
                editStok.BolumId = stokKart.BolumId;

                _db.SaveChanges();
                tabId = 1;
                state = 2;
                TempDataCreate(2);

            }


            if (tabId == 21)
            {
                // stok kart bileşen kayıt ekleme

                var stokBilesen = model.StokKartBilesenCinsi;
                if (stokBilesen.Id == 0)
                {
                    var yeniItem = new StokKartBilesenCinsi()
                    {
                        BagliOlduguId = stokBilesen.BagliOlduguId,
                        StokKartId = stokKart.Id,
                        BilesenCinsiId = stokBilesen.BilesenCinsiId
                    };
                    _db.StokKartBilesenCinsis.Add(yeniItem);
                    _db.SaveChanges();
                }
                else
                {
                    var editItem = _db.StokKartBilesenCinsis.Find(stokBilesen.Id);
                    editItem.StokBilesenAdi = stokBilesen.StokBilesenAdi;
                    _db.SaveChanges();
                }

                tabId = 2;
                state = 1;
            }


            if (tabId == 31)
            {
                var operasyon = model.StokKartOperasyon;
                var editItem = new StokKartOperasyon();

                if (operasyon.Id == 0)
                {
                    var yeniItem = new StokKartOperasyon()
                    {
                        KomponentTalimatGrupId = operasyon.KomponentTalimatGrupId,
                        StokKartId = stokKart.Id,
                        PlanlananTarih = operasyon.PlanlananTarih,
                        GerceklesenTarih = operasyon.PlanlananTarih
                    };


                    _db.StokKartOperasyons.Add(yeniItem);
                    _db.SaveChanges();
                    editItem = yeniItem;
                }
                else
                {
                    editItem = _db.StokKartOperasyons.Find(operasyon.Id);
                }

                editItem = _db.StokKartOperasyons.Find(editItem.Id);

                editItem.PlanlananTarih = operasyon.PlanlananTarih;

                editItem.PlanlananHafta = SecilenHaftaNoGetir(editItem.PlanlananTarih);
                editItem.GerceklesenHafta = SecilenHaftaNoGetir(editItem.PlanlananTarih);

                if (editItem.OperasyonDurumu < 2)
                {
                    if (editItem.PlanlananTarih < DateTime.Now)
                    {
                        editItem.OperasyonDurumu = 0;
                    }
                    else
                    {
                        editItem.OperasyonDurumu = 1;
                    }
                }

                _db.SaveChanges();

                tabId = 3;
                state = 1;
            }

            if (tabId == 41)
            {
                var hatirlatma = model.StokKartHatirlatma;
                var editItem = new StokKartHatirlatma();

                if (hatirlatma.Id == 0)
                {
                    var yeniItem = new StokKartHatirlatma()
                    {
                        StokKartId = stokKart.Id,
                        Aciklama = hatirlatma.Aciklama,
                        HatirlatmaTarihi = hatirlatma.HatirlatmaTarihi,
                        HatirlatmaDurumu = 0,
                        KayitTarihi = DateTime.Now,
                        KayitYapan = User.Id
                    };


                    _db.StokKartHatirlatmas.Add(yeniItem);
                    _db.SaveChanges();
                    editItem = yeniItem;
                }
                else
                {
                    editItem = _db.StokKartHatirlatmas.Find(hatirlatma.Id);
                }

                editItem = _db.StokKartHatirlatmas.Find(editItem.Id);

                editItem.HatirlatmaTarihi = hatirlatma.HatirlatmaTarihi;
                editItem.Aciklama = hatirlatma.Aciklama;
                editItem.HatirlatmaDurumu = hatirlatma.HatirlatmaDurumu;
                _db.SaveChanges();



                tabId = 4;
                state = 1;
            }

            return new JsonResult { Data = new { Id = stokKart.Id, state = state, tabId = tabId }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        public PartialViewResult StokKartModalTipGoster(int id, int itemId, int tip, int state)
        {
            var model = new KartStokModel()
            {
                StokKart = _db.StokKarts.Find(id),
                StokKartOperasyon = new StokKartOperasyon() { PlanlananTarih = DateTime.Now },
                StokKartBilesenCinsi = new StokKartBilesenCinsi() { StokKartId = id, }
            };

            if (tip == 1)
            {
                // bileşen

                model.BilesenCinsiGrups = _db.BilesenCinsiGrups.OrderBy(a => a.BilesenCinsiGrupAdi).ToList();
                model.BilesenCinsis = _db.BilesenCinsis.OrderBy(a => a.BilesenCinsiAdi).ToList();

                if (state == 0)
                {
                    // altına ekleniyor
                    model.StokKartBilesenCinsi.BagliOlduguId = itemId;
                    return PartialView("_ModalStokKartBilesenSecimi", model);
                }
                else
                {
                    // düzenleme yapılacak
                    if (itemId != 0)
                    {
                        model.StokKartBilesenCinsi = _db.StokKartBilesenCinsis.Find(itemId);
                    }

                    return PartialView("_ModalStokKartBilesenTanimDuzenle", model);
                }


            }
            if (tip == 2)
            {
                // periyodik bakım
                model.KomponentTalimatGrups = _db.KomponentTalimatGrups.OrderBy(a => a.KomponentTalimatGrupAdi).ToList();
                if (itemId == 0)
                {
                    model.StokKartOperasyon = new StokKartOperasyon() { StokKartId = id, PlanlananTarih = DateTime.Now };
                    // yeni periyodik bakım
                }
                else
                {
                    model.StokKartOperasyon = _db.StokKartOperasyons.Find(itemId);
                }
                return PartialView("_ModalStokKartOperasyonEkleDuzenle", model);
            }

            if (tip == 3)
            {
                // stok kart hatırlatma
                if (itemId == 0)
                {
                    model.StokKartHatirlatma = new StokKartHatirlatma() { StokKartId = id, HatirlatmaTarihi = DateTime.Now.AddDays(7) };
                    // yeni hatırlatma
                }
                else
                {
                    model.StokKartHatirlatma = _db.StokKartHatirlatmas.Find(itemId);
                }
                return PartialView("_ModalStokKartHatirlatmaEkleDuzenle", model);
            }

            return PartialView("", model);
        }




        public JsonResult StokKartItemSil(int id, int ItemId, int tip)
        {
            int state = 0;//0-bilgi eksik, 1-işlem okey, 2-redirect
            int tabId = 0;

            if (tip == 1)
            {
                // stok kart bileşen sil

                var item = _db.StokKartBilesenCinsis.Find(ItemId);
                _db.StokKartBilesenCinsis.Remove(item);
                _db.SaveChanges();
                tabId = 2;
                state = 1;

            }
            return new JsonResult { Data = new { Id = id, state = state, tabId = tabId }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }



        #region Stok kart & Medya
        [HttpPost]
        public ActionResult StokKartResimDegistir(HttpPostedFileBase file, KartStokModel model)
        {
            int state = 0;//0-bilgi eksik, 1-işlem okey, 2-redirect


            int tabId = 4;

            var stokKart = model.StokKart;

            bool sorunVarmi = false;
            if (file != null)
            {
                if (file.ContentLength < 1)
                {
                    sorunVarmi = true;
                }
            }
            else
            {
                sorunVarmi = true;
            }

            if (sorunVarmi)
            {
                TempDataCustom(1, "Resim Yükleyiniz");
            }
            else
            {
                state = 1;
                Guid guid = Guid.NewGuid();
                var path = "~/Upload/StokResimler/" + guid.ToString() + file.FileName;

                file.SaveAs(Server.MapPath(path));
                var editItem = _db.StokKarts.Find(stokKart.Id);
                editItem.ImageUrl = path;
                _db.SaveChanges();
                TempDataCreate(2);
            }

            return RedirectToAction("StokKartDetay", "Stok", new { id = stokKart.Id });
        }

        [HttpPost]
        public ActionResult StokKartMedyaEkle(HttpPostedFileBase file, KartStokModel model)
        {
            int state = 0;//0-bilgi eksik, 1-işlem okey, 2-redirect


            int tabId = 4;

            var stokKart = model.StokKart;

            bool sorunVarmi = false;
            if (file != null)
            {
                if (file.ContentLength < 1)
                {
                    sorunVarmi = true;
                }
            }
            else
            {
                sorunVarmi = true;
            }

            if (sorunVarmi == false)
            {
                sorunVarmi = model.Medya.MedyaAdi == null;
            }
            if (sorunVarmi)
            {

                TempDataCustom(1, "Resim Yükleyiniz");
            }
            else
            {
                state = 1;
                Guid guid = Guid.NewGuid();
                var path = "~/Upload/StokMedya/" + guid.ToString() + file.FileName;

                file.SaveAs(Server.MapPath(path));

                var medya = new Medya()
                {
                    BagliOlduguId = stokKart.Id,
                    FileName = path,
                    BagliOlduguTip = 0,
                    MedyaAdi = model.Medya.MedyaAdi
                };
                _db.Medyas.Add(medya);
                _db.SaveChanges();
                TempDataCreate(2);
            }

            return RedirectToAction("StokKartDetay", "Stok", new { id = stokKart.Id });
        }

        public ActionResult StokKartMedyaSil(int id)
        {
            var item = _db.Medyas.Find(id);
            var idd = item.BagliOlduguId;

            _db.Medyas.Remove(item);
            _db.SaveChanges();
            TempDataCreate(3);
            return RedirectToAction("StokKartDetay", "Stok", new { id = idd });
        }


        #endregion

    }
}