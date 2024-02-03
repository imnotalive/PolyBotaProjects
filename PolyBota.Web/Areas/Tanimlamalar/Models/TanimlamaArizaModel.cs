using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using PolyBota.DAL;
using PolyBota.Entities;

namespace PolyBota.Web.Areas.Tanimlamalar.Models
{
    public class TanimlamaArizaModel
    {
        public TanimlamaArizaModel()
        {
            ArizaTechizatGrubuTanims = new List<ArizaTechizatGrubuTanim>();
            ArizaTechizatItemTanims = new List<ArizaTechizatItemTanim>();
            TechizatTuruTanims = new List<TechizatTuruTanim>();
            ArizaTechizatGrubuTanim = new ArizaTechizatGrubuTanim();
            DropItems = new List<DropItem>();
            DropDepartmanlar = new List<DropItem>();
            PagedListSrcn = new PagedListSrcn();
            ArizaTanims = new List<ArizaTanim>();
            ArizaTanim = new ArizaTanim();
            ArizaTanimGrup = new ArizaTanimGrup();
            ArizaSorumluDepartmans = new List<ArizaSorumluDepartman>();
            DropBilesenHeader = new List<DropItem>();
            BilesenHeaders = new List<BilesenHeader>();
            ArizaTanimGerekliBilesenHeaders = new List<ArizaTanimGerekliBilesenHeader>();
            ArizaTanimGerekliTechizatTurus = new List<ArizaTanimGerekliTechizatTuru>();
            ArizaAcilabilecekDepartmans = new List<ArizaAcilabilecekDepartman>();
            ArizaTanimGrups = new List<ArizaTanimGrup>();
            ArizaTanimCozums = new List<ArizaTanimCozum>();
            ArizaTanimCozum = new ArizaTanimCozum();
            ArizaTanimArizaTechizatGrubu = new ArizaTanimArizaTechizatGrubu();
            TanimlamaArizaAlternatifGosterimItems = new List<TanimlamaArizaAlternatifGosterimItem>();
            DropItems2 = new List<DropItem>();
        }


        public List<TanimlamaArizaAlternatifGosterimItem> TanimlamaArizaAlternatifGosterimItems { get; set; }
        public ArizaTanimArizaTechizatGrubu ArizaTanimArizaTechizatGrubu { get; set; }
        public ArizaTanimCozum ArizaTanimCozum { get; set; }
        public List<ArizaTanimCozum> ArizaTanimCozums { get; set; }
        public ArizaTanimGrup ArizaTanimGrup { get; set; }
        public List<ArizaTanimGrup> ArizaTanimGrups { get; set; }
        public int KatId { get; set; }
        public PagedListSrcn PagedListSrcn { get; set; }
        public List<ArizaTechizatGrubuTanim> ArizaTechizatGrubuTanims { get; set; }
        public List<ArizaTechizatItemTanim> ArizaTechizatItemTanims { get; set; }

        public ArizaTechizatGrubuTanim ArizaTechizatGrubuTanim { get; set; }

        public List<TechizatTuruTanim> TechizatTuruTanims { get; set; }

        public List<DropItem> DropItems { get; set; }

        public List<DropItem> DropDepartmanlar { get; set; }

        public List<ArizaTanim> ArizaTanims { get; set; }

        public ArizaTanim ArizaTanim { get; set; }

        public List<ArizaSorumluDepartman> ArizaSorumluDepartmans { get; set; }
        public List<ArizaAcilabilecekDepartman> ArizaAcilabilecekDepartmans { get; set; }
        public List<DropItem> DropBilesenHeader { get; set; }

        public List<BilesenHeader> BilesenHeaders { get; set; }

        public List<ArizaTanimGerekliBilesenHeader> ArizaTanimGerekliBilesenHeaders { get; set; }
        public List<ArizaTanimGerekliTechizatTuru> ArizaTanimGerekliTechizatTurus { get; set; }
        public List<DropItem> DropItems2 { get; set; }
    }



    public class TanimlamaArizaAlternatifGosterimItem
    {
        public TanimlamaArizaAlternatifGosterimItem()
        {
            DropTechizatTuruBilesenHeader = new List<DropItem>();
            ArizaTechizatGrubuTanim = new ArizaTechizatGrubuTanim();
        }
        public int ArizaTanimArizaTechizatGrupId { get; set; }
        public ArizaTechizatGrubuTanim ArizaTechizatGrubuTanim { get; set; }

        public List<DropItem> DropTechizatTuruBilesenHeader { get; set; }
       
        
    }
   
    public class TanimlamaArizaAlternatifOzetItem
    {
        public int ArizaTanimArizaTechizatGrupId { get; set; }
        public int ArizaId { get; set; }

        public int ArizaTechizatGrubuId { get; set; }
        public int TechizatTuruId { get; set; }
        public int BilesenHeaderId { get; set; }

        public string ArizaTanimAdi { get; set; }
        public string ArizaTechizatGrubuAdi { get; set; }
        public string TechizatTuruTanimAdi { get; set; }
        public string BilesenHeaderAdi { get; set; }

    }
}