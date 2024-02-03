using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolyBota.DAL;
using PolyBota.Entities;

namespace PolyBota.Web.Areas.Takip.Models
{
    public class TakvimBakimModel
    {
        public TakvimBakimModel()
        {
            Haftalar = new List<int>();
            Yillar = new List<int>();
            BolumStoklar = new List<DropItem>();
            DropPeriyotlar = new List<DropItem>();
            StokKartOperasyons = new List<StokKartOperasyon>();
            KomponentTalimatGrups = new List<KomponentTalimatGrup>();
            StokKartOperasyonDurumTanims = new List<StokKartOperasyonDurumTanim>();
            StokKarts = new List<StokKart>();
        }
        public int SecilenYil { get; set; }
        public int BaslangicHafta { get; set; }
        public int BitisHafta { get; set; }

        public List<int> Haftalar { get; set; }
        public List<int> Yillar { get; set; }

        public List<DropItem> BolumStoklar { get; set; }
        public List<DropItem> DropPeriyotlar { get; set; }

        public List<StokKartOperasyon> StokKartOperasyons { get; set; }


        public int[,,] TabloArray { get; set; }

        public List<KomponentTalimatGrup> KomponentTalimatGrups { get; set; }

        public List<StokKartOperasyonDurumTanim> StokKartOperasyonDurumTanims { get; set; }

        public List<StokKart> StokKarts { get; set; }

    }
}