using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolyBota.DAL;
using PolyBota.Entities;
using PolyBota.Web.Models;

namespace PolyBota.Web.Areas.Kart.Models
{
    public class KartOperasyonModel: TakvimModelBase
    {
        public KartOperasyonModel()
        {
            StokKartOperasyons= new List<StokKartOperasyon>();
            StokKart= new StokKart();
            StokKarts = new List<StokKart>();
            KomponentTalimatGrups = new List<KomponentTalimatGrup>();
            User = new User();
            DropItems = new List<DropItem>();
            StokKartOperasyonKullanilanMalzemes = new List<StokKartOperasyonKullanilanMalzeme>();
            StokKartOperasyonKullanilanMalzeme = new StokKartOperasyonKullanilanMalzeme();
            StokKartOperasyon2 = new StokKartOperasyon();
        }
        public int intId { get; set; }
        public string strId { get; set; }
        public StokKart StokKart { get; set; }

        public User User { get; set; }
        public List<DropItem> DropItems { get; set; }

        public List<StokKart> StokKarts { get; set; }

        public List<StokKartOperasyon> StokKartOperasyons { get; set; }

        public List<KomponentTalimatGrup> KomponentTalimatGrups { get; set; }

        public List<TalimatTanim> TalimatTanims { get; set; }

        public KomponentTalimatGrup KomponentTalimatGrup { get; set; }

        public StokKartOperasyon StokKartOperasyon { get; set; }
        public StokKartOperasyon StokKartOperasyon2 { get; set; }
        public int[,,] ArrTable { get; set; }

        public List<StokKartOperasyonKullanilanMalzeme> StokKartOperasyonKullanilanMalzemes { get; set; }
        public StokKartOperasyonKullanilanMalzeme StokKartOperasyonKullanilanMalzeme { get; set; }

        public StokKartDuru StokKartDuru { get; set; }

        
    }
}