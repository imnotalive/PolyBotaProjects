using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebSockets;
using PolyBota.DAL;
using PolyBota.Entities;

namespace PolyBota.Web.Areas.Kart.Models
{
    public class KartStokModel
    {
        public KartStokModel()
        {
            PagedListSrcn = new PagedListSrcn();
            StokKart = new StokKart();
            SablonGrups = new List<SablonGrup>();
            SicilOzellikTanims = new List<SicilOzellikTanim>();
            SablonGrupItems = new List<SablonGrupItem>();
            KomponentTanims = new List<KomponentTanim>();
            StokKarts = new List<StokKart>();
            KomponentTanim = new KomponentTanim();
            SablonGrupItem = new SablonGrupItem();
            StokKartKomponent = new StokKartKomponent();
            KomponentTanimGrups = new List<KomponentTanimGrup>();
            DropItems = new List<DropItem>();
            StokKartHatirlatmas = new List<StokKartHatirlatma>();
            KomponentTalimatGrups = new List<KomponentTalimatGrup>();
            Medyas = new List<Medya>();
            Medya = new Medya();
            BilesenCinsiGrups = new List<BilesenCinsiGrup>();
            Bolums = new List<Bolum>();
            StokKartBilesenCinsis = new List<StokKartBilesenCinsi>();

            StokKartBilesenCinsis = new  List<StokKartBilesenCinsi>();
            StokKartBilesenCinsi = new StokKartBilesenCinsi();

            StokKartHatirlatma = new StokKartHatirlatma();
        }

      
        public PagedListSrcn PagedListSrcn { get; set; }
        public StokKart StokKart { get; set; }

        public List<SablonGrup> SablonGrups { get; set; }

        public List<SablonGrupItem> SablonGrupItems { get; set; }

        public List<SicilOzellikTanim> SicilOzellikTanims { get; set; }

        public List<SicilOzellikHeaderTanim> SicilOzellikHeaderTanims { get; set; }

    

        public List<StokKartHatirlatma> StokKartHatirlatmas { get; set; }

        public StokKartHatirlatma StokKartHatirlatma { get; set; }

        public int tabId { get; set; }

        public int KatId { get; set; }

        public List<DropItem> DropSicilOzelliks { get; set; }
        public List<KomponentTanimGrup> KomponentTanimGrups { get; set; }
        public List<KomponentTanim> KomponentTanims { get; set; }

        public List<StokKart> StokKarts { get; set; }

        public KomponentTanim KomponentTanim { get; set; }

        public SablonGrupItem SablonGrupItem { get; set; }

        public StokKartKomponent StokKartKomponent { get; set; }

        public List<DropItem> DropItems { get; set; }

        public int BaslangicHafta { get; set; }

        public int GunumuzHaftfa { get; set; }
        public int BitisHafta {get; set; }

        public List<StokKartOperasyon> StokKartOperasyons { get; set; }

        public StokKartOperasyon StokKartOperasyon { get; set; }

        public List<StokKartOperasyonDurumTanim> StokKartOperasyonDurumTanims { get; set; }

        public List<Bolum> Bolums { get; set; }

        public List<KomponentTalimatGrup> KomponentTalimatGrups { get; set; }
        public Medya Medya { get; set; }

        public List<Medya> Medyas { get; set; }


        public List<BilesenCinsiGrup> BilesenCinsiGrups { get; set; }

        public List<BilesenCinsi> BilesenCinsis { get; set; }

        public List<StokKartBilesenCinsi> StokKartBilesenCinsis { get; set; }
        public StokKartBilesenCinsi StokKartBilesenCinsi { get; set; }

    }
}