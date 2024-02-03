using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolyBota.DAL;
using PolyBota.Entities;

namespace PolyBota.Web.Areas.Ariza.Models
{
    public class ArizaDurumModel
    {
        public ArizaDurumModel()
        {
            ArizaDurumuTanims = new List<ArizaDurumuTanim>();
            ArizaDurumModelItems = new List<ArizaDurumModelItem>();
            User = new User();
            PagedListSrcn = new PagedListSrcn();
            TechizatTuruTanims = new List<TechizatTuruTanim>();
            PotaKarts = new List<PotaKart>();
            BilesenHeaders = new List<BilesenHeader>();
            BilesenItems = new List<BilesenItem>();
            
        }
        public List<ArizaDurumuTanim> ArizaDurumuTanims { get; set; }

        public List<ArizaDurumModelItem> ArizaDurumModelItems { get; set; }

        public User User { get; set; }

        public int KatId { get; set; }

        public PagedListSrcn PagedListSrcn { get; set; }

        public List<TechizatTuruTanim> TechizatTuruTanims { get; set; }

        public List<PotaKart> PotaKarts { get; set; }
        public List<BilesenHeader> BilesenHeaders { get; set; }
        public List<BilesenItem> BilesenItems { get; set; }
    }

    public class ArizaDurumModelItem
    {
        public ArizaDurumModelItem()
        {
            AcilanArizaTanim = new ArizaTanim();
            KapananArizaTanim = new ArizaTanim();
            ArizaBildirimHeader = new ArizaBildirimHeader();
            User = new User();
            DropArizaOzet = new List<DropItem>();
            ArizaBildirimItems = new List<ArizaBildirimItem>();
            ArizaTechizatGrubuTanim = new ArizaTechizatGrubuTanim();
        }
        public ArizaTanim AcilanArizaTanim { get; set; }
        public ArizaTanim KapananArizaTanim { get; set; }
        public ArizaBildirimHeader ArizaBildirimHeader { get; set; }

        public List<ArizaBildirimItem> ArizaBildirimItems { get; set; }
        public User User { get; set; }

        public List<DropItem> DropArizaOzet { get; set; }

        public ArizaTechizatGrubuTanim ArizaTechizatGrubuTanim { get; set; }

    }


}