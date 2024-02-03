using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text.pdf;
using PolyBota.BLL;
using PolyBota.DAL;
using PolyBota.Entities;
using PolyBota.Helpers;
using PolyBota.Web.Areas.Kart.Models;
using PolyBota.Web.Controllers;
using Image = iTextSharp.text.Image;
using iTextSharp.text;


namespace PolyBota.Web.Areas.Kart.Controllers
{
    public class OperasyonController : BaseController
    {
        // GET: Kart/Operasyon



        private KartOperasyonModel KartOperasyonModeliGetir(KartOperasyonModel model)
        {
            var stokKart = _db.StokKarts.Find(model.StokKart.Id);

            model.StokKart = stokKart;
            var header = DropTakvimTableHeader(model.BaslangisTarihi, model.BitisTarihi, model.GosterimSekli);

            var baslangic = model.BaslangisTarihi.Date;
            var bitis = model.BitisTarihi.AddDays(1).Date;
            var operasyonlar = _db.StokKartOperasyons
                .Where(a => a.StokKartId == stokKart.Id && a.PlanlananTarih >=
                    baslangic && a.PlanlananTarih < bitis).OrderBy(a => a.PlanlananTarih)
                .ToList();
            model.KomponentTalimatGrups = _db.KomponentTalimatGrups.ToList();
            model.StokKartOperasyons = operasyonlar;
            model.TableHeaderlar = header;


            model.ArrTable = new int[operasyonlar.Count, header.Count, 2];

            int i = -1;

            foreach (var operasyon in operasyonlar)
            {
                i++;
                int j = -1;
                foreach (var dropItem in header)
                {
                    j++;

                    if (operasyon.PlanlananTarih >= dropItem.DateTime && operasyon.PlanlananTarih < dropItem.DateTime2)
                    {
                        model.ArrTable[i, j, 0] = operasyon.Id;
                    }
                }
            }

            return model;
        }

        #region Stok kart operasyonları
        public ActionResult KartOperasyonlar(int id = 159)
        {


            var model = new KartOperasyonModel()
            {
                StokKart = new StokKart() { Id = id },
                GosterimSekli = 1,
                StokKartOperasyon = new StokKartOperasyon(){PlanlananTarih = DateTime.Now}
            };

            model = KartOperasyonModeliGetir(model);
            return View(model);
        }

        [HttpPost]
        public PartialViewResult OperasyonListesi(KartOperasyonModel model)
        {
            return PartialView("_OperasyonListesi", KartOperasyonModeliGetir(model));
        }

        [HttpPost]
        public ActionResult OperasyonEkle(KartOperasyonModel model)
        {
            var operasyon = model.StokKartOperasyon;

            var yeniItem = new StokKartOperasyon()
            {
                KomponentTalimatGrupId = operasyon.KomponentTalimatGrupId,
                StokKartId = model.StokKart.Id,
                OperasyonDurumu = 0,
                PlanlananTarih = operasyon.PlanlananTarih,
                IlkPlanlananTarih = operasyon.PlanlananTarih,
                PlanlananHafta = 0,
                GerceklesenTarih = operasyon.PlanlananTarih,
                KayitYapanId = User.Id,
                YapilanTalimatlarStr = "",
                GerceklesenHafta = 0
            };
            _db.StokKartOperasyons.Add(yeniItem);
            _db.SaveChanges();
            TempDataCreate(1);
            return RedirectToAction("KartOperasyonlar", "Operasyon", new {id= model.StokKart.Id });
        }
        #endregion

        #region Operasyon Detay ve Malzeme Kullanımı

        public ActionResult OperasyonDetay(int id)
        {
            var model = new KartOperasyonModel()
            {
                StokKartOperasyon = new StokKartOperasyon(),
                KomponentTalimatGrup = new KomponentTalimatGrup(),
                TalimatTanims = new List<TalimatTanim>()
            };

            var operasyon = _db.StokKartOperasyons.Find(id);
            var stokKart = _db.StokKarts.Find(operasyon.StokKartId);
            var kompTalimatGrup = _db.KomponentTalimatGrups.Find(operasyon.KomponentTalimatGrupId);
            var operasyonMalzemeler = _db.StokKartOperasyonKullanilanMalzemes.Where(a => a.OperasyonId == id)
                .OrderBy(a => a.KayitTarihi).ToList();

            var stokIdler = operasyonMalzemeler.Select(a => a.KullanilanStokKartId).Distinct().ToList();

            var guncelPeriyot = _db.PeriyotTanims.Find(kompTalimatGrup.PeriyotTanimId);

            model.StokKartOperasyon2 = new StokKartOperasyon()
            {
                PlanlananTarih = operasyon.PlanlananTarih.AddDays(7 * guncelPeriyot.PeriyotDonemi),
                KomponentTalimatGrupId = operasyon.KomponentTalimatGrupId
            };



            var talimatTanimIdler = _db.KomponentTalimats.Where(a => a.KomponentTalimatGrupId == kompTalimatGrup.Id)
                .Select(a => a.TalimatTanimId).ToList();

            var talimatTanims = new List<TalimatTanim>();

            foreach (var i in talimatTanimIdler)
            {
                if (_db.TalimatTanims.Any(a => a.Id == i))
                {
                    talimatTanims.Add(_db.TalimatTanims.First(a => a.Id == i));
                }
            }

            model.User = _db.Users.Find(operasyon.KayitYapanId);

            model.StokKart = stokKart;
            model.StokKartOperasyon = operasyon;
            model.KomponentTalimatGrup = kompTalimatGrup;
            model.TalimatTanims = talimatTanims;
            model.KomponentTalimatGrups = _db.KomponentTalimatGrups.ToList();
            model.StokKartOperasyonKullanilanMalzemes = operasyonMalzemeler;
            model.StokKarts = _db.StokKarts.Where(a => stokIdler.Any(b => b == a.Id)).ToList();


            var strList = operasyon.YapilanTalimatlarStr.StrArrayeCevir("-").ToList();

            foreach (var itt in model.TalimatTanims)
            {
                itt.SeciliMi = strList.Any(a => a == itt.Id.ToString());
            }
            return View(model);
        }

        public PartialViewResult OperasyonaMalzemeEkle(int id)
        {
            var model = new KartOperasyonModel()
            {
                StokKartOperasyon = new StokKartOperasyon() { Id = id }
            };

            var squery =
                "SELECT TOP (100) PERCENT dbo.Ambar.AmbarAdi, dbo.AmbarStokKart.Id, dbo.AmbarStokKart.ToplamMiktar, dbo.StokKart.StokKodu, dbo.StokKart.StokTanimAdi FROM            dbo.StokKart INNER JOIN dbo.AmbarStokKart ON dbo.StokKart.Id = dbo.AmbarStokKart.StokKartId INNER JOIN dbo.Ambar ON dbo.AmbarStokKart.AmbarId = dbo.Ambar.Id WHERE        (dbo.Ambar.AmbarTipi = 1) AND (dbo.AmbarStokKart.ToplamMiktar > 0) ORDER BY dbo.AmbarStokKart.ToplamMiktar DESC, dbo.Ambar.AmbarAdi, dbo.StokKart.StokTanimAdi";

            var result = BllMssql.CustomSql(squery, SuaHelper.defaultSql()).ToList();

            foreach (var itt in result)
            {
                var lst = itt.Values.ToList();

                string ambarAdi = lst[0].ToString();
                string ambarstokKartId = lst[1].ToString();

                decimal toplamMiktar = (decimal)(lst[2] ?? 0);
                string stokKodu = lst[3].ToString();
                string stokTanimAdi = lst[4].ToString();

                var tanim = string.Format("({0}) {1}-{2} -- {3} Adet", ambarAdi, stokTanimAdi, stokKodu, toplamMiktar);



                model.DropItems.Add(new DropItem() { Id = ambarstokKartId, Tanim = tanim });
            }


            return PartialView("_OperasyonaMalzemeEkle", model);
        }

        [HttpPost]
        public JsonResult OperasyonaMalzemeEkle(KartOperasyonModel model)
        {
            int state = 0;
            string title = "";
            string icon = "warning";

            var operasyon = _db.StokKartOperasyons.Find(model.StokKartOperasyon.Id);
            var kompTalimatGrup = _db.KomponentTalimatGrups.Find(operasyon.KomponentTalimatGrupId);
            var kullanilanMalzeme = model.StokKartOperasyonKullanilanMalzeme;
            var miktar = kullanilanMalzeme.Miktar;

            if (model.intId != 0 && miktar > 0)
            {
                var ambarStokKart = _db.AmbarStokKarts.Find(model.intId);

                if (ambarStokKart.ToplamMiktar >= miktar)
                {
                    string aciklama = string.Format("{0} Malzeme Kullanımı - {1}", kompTalimatGrup.KomponentTalimatGrupAdi, User.Name);

                    var hareketId = BotaAmbarStokHareketOlustur(ambarStokKart.Id, 1, miktar, aciklama, 0, 0);

                    var yeniItem = new StokKartOperasyonKullanilanMalzeme()
                    {
                        Miktar = miktar,
                        KayitTarihi = DateTime.Now,
                        KullanilanStokKartId = ambarStokKart.StokKartId,
                        KullanilanAmbar = ambarStokKart.AmbarId,
                        OperasyonId = operasyon.Id,
                        AmbarStokKartHareketId = hareketId
                    };
                    _db.StokKartOperasyonKullanilanMalzemes.Add(yeniItem);
                    _db.SaveChanges();



                    state = 1;
                    TempDataCreate(2);
                }
                else
                {
                    title = "Miktar Yetersiz !!";
                }

            }
            else
            {
                title = "Bilgileri Kontrol Ediniz!!";
            }

            return new JsonResult { Data = new { Id = operasyon.Id, state = state, icon = icon, title = title }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult OperasyonDuzenle(int id, KartOperasyonModel model)
        {
            var operasyon = model.StokKartOperasyon;


            int state = 0;
            string title = "";
            string icon = "warning";
            var editOperasyon = _db.StokKartOperasyons.Find(operasyon.Id);
            if (id == 1)
            {
                // bilgiler güncellenecek


                if (editOperasyon.KayitYapanId == User.Id)
                {
                    if (editOperasyon.OperasyonDurumu == 1)
                    {
                        // gerçekleştirilmiş
                        title = "Operasyon Gerçekleştiği için Düzenleme yapılamaz";
                    }
                    else
                    {
                        editOperasyon.OperasyonOncesiAciklama = operasyon.OperasyonOncesiAciklama;
                        editOperasyon.PlanlananTarih = operasyon.PlanlananTarih;
                        editOperasyon.KomponentTalimatGrupId = operasyon.KomponentTalimatGrupId;
                        _db.SaveChanges();
                        state = 1;
                        TempDataCreate(2);
                    }
                }
                else
                {
                    title = "Kayıt Yapan olmadığınız için güncelleme yapamazsınız";

                }
            }
            if (id == 2)
            {
                // operasyon tamamlanacak
                var talimatIdler = model.TalimatTanims.Where(a => a.SeciliMi).ToList().Select(a => a.Id);
                if (editOperasyon.OperasyonDurumu == 1)
                {
                    title = "Operasyon Daha Önce Onaylanmıştır";
                }
                else
                {
                    editOperasyon.GerceklesenTarih = operasyon.GerceklesenTarih;
                    editOperasyon.OperasyonSonrasiAciklama = operasyon.OperasyonSonrasiAciklama;
                    editOperasyon.OperasyonDurumu = 1;
                    var strIdler = "";
                    foreach (var i in talimatIdler)
                    {
                        strIdler += i;
                        if (i != talimatIdler.Last())
                        {
                            strIdler += "-";
                        }
                    }

                    editOperasyon.YapilanTalimatlarStr = strIdler;
                    _db.SaveChanges();
                    state = 1;

                    if (model.intId==0)
                    {
                        // yeni periyodik Operasyon

                        var periyodikOperasyon = model.StokKartOperasyon2;

                        var yeniItem = new StokKartOperasyon()
                        {
                            PlanlananTarih = periyodikOperasyon.PlanlananTarih,
                            KomponentTalimatGrupId = periyodikOperasyon.KomponentTalimatGrupId,
                            StokKartId = editOperasyon.StokKartId,
                            OperasyonDurumu = 0,
                            PlanlananHafta = 0,
                            GerceklesenTarih = periyodikOperasyon.PlanlananTarih,
                            GerceklesenHafta = 0,
                            IlkPlanlananTarih = periyodikOperasyon.PlanlananTarih,
                            KayitYapanId = User.Id,
                            YapilanTalimatlarStr = ""
                            
                        };
                        _db.StokKartOperasyons.Add(yeniItem);
                        _db.SaveChanges();
                    }
                    TempDataCreate(2);
                }


            }
            return new JsonResult { Data = new { Id = operasyon.Id, state = state, icon = icon, title = title }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion


        #region Operasyon PDF
        public MemoryStream PdfiGetir(string yol, int operasyonId, int tip)
        {

            try
            {

                var operasyon = _db.StokKartOperasyons.Find(operasyonId);
                var stokKart = _db.StokKarts.Find(operasyon.StokKartId);
                var talimatGrup = _db.KomponentTalimatGrups.Find(operasyon.KomponentTalimatGrupId);
                var periyot = _db.PeriyotTanims.Find(talimatGrup.PeriyotTanimId);

                var talimatTanimIdler = _db.KomponentTalimats
                      .Where(a => a.KomponentTalimatGrupId == operasyon.KomponentTalimatGrupId)
                      .Select(a => a.TalimatTanimId).ToList();


                /*
                var talimatTanims = new List<TalimatTanim>();

                foreach (var i in talimatTanimIdler)
                {
                    talimatTanims.Add(_db.TalimatTanims.Find(i));
                }

                */

                var talimatTanims = _db.TalimatTanims.Where(a => talimatTanimIdler.Any(b => b == a.Id))
                    .OrderBy(a => a.TalimatAciklama).ToList();


                #region Harici Fontlar
                Font italikFont = new Font(BaseFont.CreateFont(Server.MapPath("~/fonts/Montserrat/Montserrat-Italic.ttf"), BaseFont.IDENTITY_H, iTextSharp.text.pdf.BaseFont.EMBEDDED));

                iTextSharp.text.Font boldFont = new iTextSharp.text.Font(iTextSharp.text.pdf.BaseFont.CreateFont(Server.MapPath("~/fonts/Montserrat/Montserrat-Bold.ttf"), iTextSharp.text.pdf.BaseFont.IDENTITY_H, iTextSharp.text.pdf.BaseFont.EMBEDDED));

                iTextSharp.text.Font normalFont = new iTextSharp.text.Font(iTextSharp.text.pdf.BaseFont.CreateFont(Server.MapPath("~/fonts/Montserrat/Montserrat-Regular.ttf"), iTextSharp.text.pdf.BaseFont.IDENTITY_H, iTextSharp.text.pdf.BaseFont.EMBEDDED));

                iTextSharp.text.Font normalFontYesil = new iTextSharp.text.Font(iTextSharp.text.pdf.BaseFont.CreateFont(Server.MapPath("~/fonts/Montserrat/Montserrat-Regular.ttf"), iTextSharp.text.pdf.BaseFont.IDENTITY_H, iTextSharp.text.pdf.BaseFont.EMBEDDED));

                iTextSharp.text.Font normalFontBeyaz = new iTextSharp.text.Font(iTextSharp.text.pdf.BaseFont.CreateFont(Server.MapPath("~/fonts/Montserrat/Montserrat-Bold.ttf"), iTextSharp.text.pdf.BaseFont.IDENTITY_H, iTextSharp.text.pdf.BaseFont.EMBEDDED));

                boldFont.SetStyle("Bold");
                boldFont.Color = BaseColor.BLACK;
                boldFont.Size = 9;

                //baslik.Color = BaseColor.RED;
                italikFont.Size = 7;
                normalFontYesil.SetColor(50, 105, 70);
                normalFontBeyaz.SetColor(255, 255, 255);
                normalFont.Size = 8;
                #endregion

                #region Tanimlalamalar

                PdfPTable pdfUstDetay = new PdfPTable(3);

                PdfPTable pdfTalimatlar = new PdfPTable(2);
                PdfPTable pdfKullanilanMalzemeler = new PdfPTable(3);


                pdfUstDetay.WidthPercentage = 80;
                pdfUstDetay.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                pdfUstDetay.DefaultCell.VerticalAlignment = Element.ALIGN_LEFT;
                pdfUstDetay.DefaultCell.BorderWidth = 0;

                pdfTalimatlar.WidthPercentage = 90;
                pdfKullanilanMalzemeler.WidthPercentage = 90;
                #endregion


                #region Üst Detay
                var dropLst = new List<DropItem>()
                {

                    new DropItem() {Tanim1 = " ", Tanim2 = " "},
                    new DropItem() {Tanim1 = " ", Tanim2 = " "},
                    new DropItem() {Tanim1 = "Stok", Tanim2 = stokKart.StokTanimAdi},
                    new DropItem() {Tanim1 = "Stok Kodu", Tanim2 = stokKart.StokKodu},
                    new DropItem() {Tanim1 = "Sicil No", Tanim2 = stokKart.SicilNo},
                    new DropItem() {Tanim1 = "Seri No", Tanim2 = stokKart.SeriNo},
                    new DropItem() {Tanim1 = " ", Tanim2 = " "},


                };

                foreach (var drop in dropLst)
                {
                    pdfUstDetay.AddCell(new Phrase("", normalFont));
                    pdfUstDetay.AddCell(new Phrase(drop.Tanim1, normalFont));
                    pdfUstDetay.AddCell(new Phrase(drop.Tanim2, normalFont));
                }
                pdfUstDetay.AddCell(new Phrase("TALİMAT", normalFont));
                pdfUstDetay.AddCell(new Phrase(talimatGrup.KomponentTalimatGrupAdi, boldFont));
                pdfUstDetay.AddCell(new Phrase("", normalFont));


                pdfUstDetay.AddCell(new Phrase("PERİYOT", normalFont));
                pdfUstDetay.AddCell(new Phrase(periyot.PeriyotTanimAdi, boldFont));
                pdfUstDetay.AddCell(new Phrase("", normalFont));

                pdfUstDetay.AddCell(new Phrase("PERİYOT DÖNEMİ", normalFont));
                pdfUstDetay.AddCell(new Phrase(periyot.PeriyotDonemi + " Hafta", boldFont));
                pdfUstDetay.AddCell(new Phrase("", normalFont));
                #endregion
                #region Resim Yukleme
                iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Resimler/bgAntet.png"));
                // jpg.ScaleToFit(50, 10);

                var sayfaDuzeni = PageSize.A4;
                jpg.Alignment = Image.UNDERLYING;
                jpg.ScaleToFit(sayfaDuzeni.Width, sayfaDuzeni.Height);
                #endregion



                var tableWidths = new List<float>();
                tableWidths.Add(1);
                tableWidths.Add(4);
                pdfTalimatlar.SetWidths(tableWidths.ToArray());

                pdfTalimatlar.AddCell(new Phrase("Durum", boldFont));
                pdfTalimatlar.AddCell(new Phrase("Talimat", boldFont));

                foreach (var talimat in talimatTanims)
                {
                    pdfTalimatlar.AddCell(new Phrase(" ", italikFont));
                    pdfTalimatlar.AddCell(new Phrase(talimat.TalimatAciklama, italikFont));
                }




                pdfKullanilanMalzemeler.AddCell(new Phrase("Stok Kodu", boldFont));
                pdfKullanilanMalzemeler.AddCell(new Phrase("Stok Tanımı", boldFont));
                pdfKullanilanMalzemeler.AddCell(new Phrase("Miktar", boldFont));

                for (int h = 0; h < 7; h++)
                {

                    pdfKullanilanMalzemeler.AddCell(new Phrase(" ", normalFont));
                    pdfKullanilanMalzemeler.AddCell(new Phrase(" ", normalFont));
                    pdfKullanilanMalzemeler.AddCell(new Phrase(" ", normalFont));
                }

                tableWidths = new List<float>();
                tableWidths.Add(1);
                tableWidths.Add(3);
                tableWidths.Add(1);
                pdfKullanilanMalzemeler.SetWidths(tableWidths.ToArray());
                #region PDF Olusturma Alani

                pdfKullanilanMalzemeler.SpacingBefore = 20;


                MemoryStream workStream = new MemoryStream();
                Document pdfDoc = new Document(sayfaDuzeni, 0, 0, 0, 0f);
                //pdfDoc.SetPageSize(PageSize.A4.Rotate());
                PdfWriter.GetInstance(pdfDoc, workStream).CloseStream = false;

                pdfDoc.Open();
                pdfDoc.Add(jpg);
                pdfDoc.Add(pdfUstDetay);
                pdfDoc.Add(pdfTalimatlar);
                pdfDoc.Add(pdfKullanilanMalzemeler);

                pdfDoc.Close();

                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                workStream.Position = 0;
                return workStream;

                #endregion


            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public ActionResult OperasyonPdfIndir(int id, int tip)
        {
            string strPDFFileName = string.Format(Guid.NewGuid().ToString());
            string yol = Server.MapPath("~/Pdfler/" + strPDFFileName);


            var workStream = PdfiGetir(yol, id, tip);

            var cd = new System.Net.Mime.ContentDisposition
            {
                // for example foo.bak
                FileName = DateTime.Now.ToString() + ".pdf",
                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = false,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(workStream.ToArray(), "application/pdf");
        }
        #endregion

    }
}