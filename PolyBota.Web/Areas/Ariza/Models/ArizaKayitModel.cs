using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolyBota.DAL;
using PolyBota.Entities;

namespace PolyBota.Web.Areas.Ariza.Models
{
    public class ArizaKayitModel
    {
        public ArizaKayitModel()
        {
            ArizaTechizatGrubuTanims = new List<ArizaTechizatGrubuTanim>();
            ArizaTechizatGrubuTanim = new ArizaTechizatGrubuTanim();
            ArizaTechizatItemTanims = new List<ArizaTechizatItemTanim>();
            TechizatTuruTanims = new List<TechizatTuruTanim>();
            ArizaPotaKartSecimModels = new List<ArizaPotaKartSecimModel>();
            ArizaTanims = new List<ArizaTanim>();
            PotaKarts = new List<PotaKart>();
            ArizaTanim = new ArizaTanim();
        }

        public int YonlendirmeDurumu { get; set; }
        public int PotaKartId { get; set; }
        public int ArizaTechizatGrubuId { get; set; }
        public List<PotaKart> PotaKarts { get; set; }
        public List<ArizaTechizatGrubuTanim> ArizaTechizatGrubuTanims { get; set; }
        public List<ArizaTechizatItemTanim> ArizaTechizatItemTanims { get; set; }
        public List<TechizatTuruTanim> TechizatTuruTanims { get; set; }
        public ArizaTechizatGrubuTanim ArizaTechizatGrubuTanim { get; set; }
        public List<ArizaPotaKartSecimModel> ArizaPotaKartSecimModels { get; set; }
     
        public int indexId { get; set; }

        public int ArizaTanimId { get; set; }

        public string ArizaNotu { get; set; }
        public List<ArizaTanim> ArizaTanims { get; set; }

        public ArizaTanim ArizaTanim { get; set; }
    }

    public class ArizaPotaKartSecimModel
    {
        public ArizaPotaKartSecimModel()
        {
            TechizatTuruTanim = new TechizatTuruTanim();
            PotaKarts = new List<PotaKart>();
            PotaKartBilesenModels = new List<PotaKartBilesenModel>();
            
        }
        public TechizatTuruTanim TechizatTuruTanim { get; set; }

        public int PotaKartId { get; set; }

        public List<PotaKart> PotaKarts { get; set; }

        public bool BilesenVarMi { get; set; }

        public List<PotaKartBilesenModel> PotaKartBilesenModels { get; set; }
        public int i { get; set; }

        public int ArizaTanimArizaTechizatGrubuId  { get; set; }
    }

    public class PotaKartBilesenModel
    {
        public PotaKartBilesenModel()
        {
            DropPotaKartBilesenItems = new List<DropItem>();
            BilesenHeader = new BilesenHeader();
        }

        public int SecilenBilesenItemId { get; set; }
        public BilesenHeader BilesenHeader { get; set; }

        public List<DropItem> DropPotaKartBilesenItems { get; set; }
    }


}