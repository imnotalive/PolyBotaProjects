using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Web;
using PolyBota.DAL;
using PolyBota.Entities;

using PolyBota.Web.Models;

namespace PolyBota.Web.Areas.Raporlama.Models
{
    public class RaporlamaOperasyonModel: TakvimModelBase
    {
        public RaporlamaOperasyonModel()
        {
            PeriyotTanimKomponentTalimatGrupModels = new List<PeriyotTanimKomponentTalimatGrupModel>();

            BaslangisTarihi = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            BitisTarihi = new DateTime(DateTime.Now.Year, DateTime.Now.Month+1, 1);
            GosterimSekli = 0;
            KomponentTalimatGrups = new List<KomponentTalimatGrup>();
            KartOperasyons = new List<StokKartOperasyon>();
            PotaKarts = new List<PotaKart>();
            DropOperasyonDurumlari = new List<DropItem>()
            {
                new DropItem(){Id = "1", SeciliMi = true, Tanim = "Operasyon Tamamlandı", Tanim2 = "alert-success"},
                new DropItem(){Id = "0", SeciliMi = true, Tanim = "Operasyon Tarihi Henüz Gelmedi", Tanim2 = "alert-secondary"},
                new DropItem(){Id = "2", SeciliMi = true, Tanim = "Operasyon Gecikti", Tanim2 = "alert-danger"},
               
              
            };
        }
        public List<PeriyotTanimKomponentTalimatGrupModel> PeriyotTanimKomponentTalimatGrupModels { get; set; }

        public List<KomponentTalimatGrup> KomponentTalimatGrups { get; set; }

        public List<StokKartOperasyon> KartOperasyons { get; set; }

        public List<PotaKart> PotaKarts { get; set; }
        public List<DropItem> DropOperasyonDurumlari { get; set; }

        public int[,,] ArrTable { get; set; }

    }

    public class PeriyotTanimKomponentTalimatGrupModel
    {
        public PeriyotTanimKomponentTalimatGrupModel()
        {
            PeriyotTipiTanim = new PeriyotTipiTanim();
            DropPeriyotTanimKompGrups = new List<DropItem>();
        }
        public PeriyotTipiTanim PeriyotTipiTanim { get; set; }
        public List<DropItem> DropPeriyotTanimKompGrups { get; set; }
    }
}