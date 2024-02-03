using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using System.Windows;
using PolyBota.DAL;
using PolyBota.Entities;
using PolyBota.Web.Models;

namespace PolyBota.Web.Areas.Takvim.Model
{
    public class TakvimOperasyonModel:TakvimModelBase
    {
        public TakvimOperasyonModel()
        {
            DropTakvimTipler = new List<DropItem>();
            DropBreadCrumbs = new List<DropItem>();
            PotaKarts = new List<PotaKart>();
            KomponentTalimatGrups = new List<KomponentTalimatGrup>();
            StokKartOperasyons = new List<StokKartOperasyon>();
            PeriyotTanims = new List<PeriyotTanim>();
            StokKartDuruslar = new List<StokKartDuru>();
            PeriyotTipiTanims = new List<PeriyotTipiTanim>();
            BreadCrumbs = new List<BreadCrumb>();
            TakvimOperasyonItemDetayModels = new List<TakvimOperasyonItemDetayModel>();
        }


        public int ListelemeSekli { get; set; }//0-table 1-liste
        public List<BreadCrumb> BreadCrumbs { get; set; }
        public int SecimDurumu { get; set; }

        public List<DropItem> DropTakvimTipler { get; set; }

        public List<DropItem> DropBreadCrumbs { get; set; }

        public List<PotaKart> PotaKarts { get; set; }

        public int[,,,] ArrTable { get; set; } // satir,sutun, durus-operasyon,id

        public List<KomponentTalimatGrup> KomponentTalimatGrups { get; set; }

        public List<StokKartOperasyon> StokKartOperasyons { get; set; }

        public List<PeriyotTanim> PeriyotTanims { get; set; }

        public List<StokKartDuru> StokKartDuruslar { get; set; }

        public List<PeriyotTipiTanim> PeriyotTipiTanims { get; set; }

        public List<TakvimOperasyonItemDetayModel> TakvimOperasyonItemDetayModels { get; set; }


    }


    public class TakvimOperasyonItemDetayModel
    {
        public DateTime PlanlananTarih { get; set; }
        public int OperasyonId { get; set; }
        public int PotaKartId { get; set; }
        public int KomponentTalimatGrupId { get; set; }
        public int OperasyonDurumu { get; set; }
        public int PeriyotTanimId { get; set; }
        public string PotaKartAdi { get; set; }
        public string KomponentTalimatGrupAdi { get; set; }
        public string PeriyotTanimAdi { get; set; }
        public int PeriyotDonemi { get; set; }
    }
}