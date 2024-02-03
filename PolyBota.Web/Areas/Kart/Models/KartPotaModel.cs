using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolyBota.DAL;
using PolyBota.Entities;
using PolyBota.Web.Models;

namespace PolyBota.Web.Areas.Kart.Models
{
    public class KartPotaModel : TakvimModelBase
    {
        public KartPotaModel()
        {
            TechizatTuruBilesenHeaders = new List<TechizatTuruBilesenHeader>();
            TechizatTuruTanims = new List<TechizatTuruTanim>();
            TuruTanims = new List<TechizatTuruTanim>();
            BilesenHeaders = new List<BilesenHeader>();
            BilesenItems = new List<BilesenItem>();
            PotaKarts = new List<PotaKart>();
            PotaKart = new PotaKart();
            DropBolumler = new List<DropItem>();
            DropKomponentler = new List<DropItem>();
            SicilOzellikTanims = new List<SicilOzellikTanim>();
            PotaKartSicilOzelliks = new List<PotaKartSicilOzellik>();
            PotaKartAgacs = new List<PotaKartAgac>();
            KomponentTanimGrups = new List<KomponentTanimGrup>();
            KomponentTanims = new List<KomponentTanim>();
            PagedListSrcn = new PagedListSrcn();
            Bolums = new List<Bolum>();
            SicilOzellikHeaderTanims = new List<SicilOzellikHeaderTanim>();
            Medyas = new List<Medya>();
            Medya = new Medya();
            KomponentTalimatGrups = new List<KomponentTalimatGrup>();
            StokKartOperasyons = new List<StokKartOperasyon>();
            PotaKlasorKlasors = new List<PotaKlasorKlasor>();
            PotaKlasorKlasor = new PotaKlasorKlasor();
            StokKartDurus = new List<StokKartDuru>();
            StokKartDuru = new StokKartDuru();
            User = new User();
            KomponentTalimatGrup = new KomponentTalimatGrup();
            TalimatTanims = new List<TalimatTanim>();
            StokKartOperasyonKullanilanMalzemes = new List<StokKartOperasyonKullanilanMalzeme>();
            StokKarts = new List<StokKart>();
            DropItems = new List<DropItem>();
            StokKartOperasyonKullanilanMalzeme = new StokKartOperasyonKullanilanMalzeme();
            PotaKartEntegreKarts = new List<PotaKartEntegreKart>();
            DurusGrubuTanims = new List<DurusGrubuTanim>();
            DurusTipiTanims = new List<DurusTipiTanim>();
            DropTechizatTopluGiris = new List<DropItem>();
            BreadCrumbs = new List<BreadCrumb>();
            DropPeriyodikOperasyonItem = new DropItem()
            {
                ItemValues = new List<ItemValue>()
                {
                    new ItemValue() {Text = "Günlük", IdInt = 0},
                    new ItemValue() {Text = "Haftalık", IdInt = 1},
                    new ItemValue() {Text = "Aylık", IdInt = 2}
                },
                IdInt = 1,
                IdInt2 = 2
            };
        }

        public List<BreadCrumb> BreadCrumbs { get; set; }
        public List<PotaKartEntegreKart> PotaKartEntegreKarts { get; set; }
        public List<DropItem> DropTechizatTopluGiris { get; set; }
        public int SecimId { get; set; }
        public StokKartOperasyonKullanilanMalzeme StokKartOperasyonKullanilanMalzeme { get; set; }
        public List<KomponentTalimatGrup> KomponentTalimatGrups { get; set; }
        public List<Bolum> Bolums { get; set; }
        public PagedListSrcn PagedListSrcn { get; set; }
        public List<TechizatTuruBilesenHeader> TechizatTuruBilesenHeaders { get; set; }
        public List<TechizatTuruTanim> TechizatTuruTanims { get; set; }

        public List<TechizatTuruTanim> TuruTanims { get; set; }

        public List<BilesenHeader> BilesenHeaders { get; set; }

        public List<BilesenItem> BilesenItems { get; set; }

        public List<PotaKart> PotaKarts { get; set; }

        public PotaKart PotaKart { get; set; }

        public List<DropItem> DropBolumler { get; set; }
        public List<DropItem> DropKomponentler { get; set; }

        public List<SicilOzellikTanim> SicilOzellikTanims { get; set; }

        public List<PotaKartSicilOzellik> PotaKartSicilOzelliks { get; set; }
        public int tabId { get; set; }
        public List<PotaKartAgac> PotaKartAgacs { get; set; }
        public int KatId { get; set; }
        public List<KomponentTanimGrup> KomponentTanimGrups { get; set; }
        public List<KomponentTanim> KomponentTanims { get; set; }
        public List<SicilOzellikHeaderTanim> SicilOzellikHeaderTanims { get; set; }
        public Medya Medya { get; set; }
        public List<Medya> Medyas { get; set; }

        public List<StokKartOperasyon> StokKartOperasyons { get; set; }


        public int[,,] ArrtTable { get; set; }

        public List<PotaKlasorKlasor> PotaKlasorKlasors { get; set; }
        public PotaKlasorKlasor PotaKlasorKlasor { get; set; }

        public List<StokKartDuru> StokKartDurus { get; set; }

        public StokKartDuru StokKartDuru { get; set; }

        public StokKartOperasyon StokKartOperasyon { get; set; }
        public StokKartOperasyon StokKartOperasyon2 { get; set; }

        //
        public User User { get; set; }

        public KomponentTalimatGrup KomponentTalimatGrup { get; set; }
        public List<TalimatTanim> TalimatTanims { get; set; }
        public List<StokKartOperasyonKullanilanMalzeme> StokKartOperasyonKullanilanMalzemes { get; set; }

        public List<StokKart> StokKarts { get; set; }

        public List<DropItem> DropItems { get; set; }

        public List<DurusGrubuTanim> DurusGrubuTanims { get; set; }

        public List<DurusTipiTanim> DurusTipiTanims { get; set; }

        public DropItem DropPeriyodikOperasyonItem { get; set; }

    }
}