using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Office.Interop.Excel;
using PolyBota.DAL;
using PolyBota.Entities;

namespace PolyBota.Web.Areas.Raporlama.Models
{
    public class RaporlamaArizaModel
    {
        public RaporlamaArizaModel()
        {
            BaslangicTarihi = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            
            BitisTarihi = DateTime.Now;
            DropSorumluDepartmans = new List<DropItem>();
            DropArizaTechizatGruplar = new List<DropItem>();
            DropArizaDurumus = new List<DropItem>();
            RaporlamaArizaModelItems = new List<RaporlamaArizaModelItem>();
            RaporlamaMakineBazliAnalizItems = new List<RaporlamaMakineBazliAnalizItem>();
            DropItems = new List<DropItem>();
            RaporlamaMakineBazliAnalizPk = new RaporlamaMakineBazliAnalizPK();
        }

        public DateTime BaslangicTarihi { get; set; }

        public DateTime BitisTarihi { get; set; }
        public List<DropItem> DropItems { get; set; }
        public List<DropItem> DropSorumluDepartmans { get; set; }
        public List<DropItem> DropArizaDurumus { get; set; }
        public List<DropItem> DropArizaTechizatGruplar { get; set; }
        public List<RaporlamaArizaModelItem> RaporlamaArizaModelItems { get; set; }
        public RaporlamaMakineBazliAnalizPK RaporlamaMakineBazliAnalizPk { get; set; }
        public List<RaporlamaMakineBazliAnalizItem> RaporlamaMakineBazliAnalizItems { get; set; }
    }

    public class RaporlamaArizaModelItem
    {
        public RaporlamaArizaModelItem()
        {
            RaporlamaArizaModelItemPotaKarts = new List<RaporlamaArizaModelItemPotaKart>();
        }
        public int ArizaTechizatGrubuId { get; set; }
        public int Id { get; set; }
        public int DepartmanId { get; set; }
        public string DepartmanAdi { get; set; }
        public string ArizaTechizatGrubuAdi { get; set; }

        public DateTime KayitTarihi { get; set; }

        public string AcilanArizaTanimAdi { get; set; }

        public string KapananArizaTanimAdi { get; set; }
        public string KayitYapanKulAdi { get; set; }

        public int ArizaDurumu { get; set; }

        public string ArizaDurumuAdi { get; set; }
        public List<RaporlamaArizaModelItemPotaKart> RaporlamaArizaModelItemPotaKarts { get; set; }
    }


    public class RaporlamaArizaModelItemPotaKart
    {
        public RaporlamaArizaModelItemPotaKart()
        {
            DropBilesenHeaderItems = new List<DropItem>();

        }
        public int PotaKartId { get; set; }
        public string PotaKartAdi { get; set; }

        public List<DropItem> DropBilesenHeaderItems { get; set; }
    }


    public class RaporlamaAcilanArizaGenelItem
    {

        public int TechizatTuruId { get; set; }
        public string TechizatTuruTanimAdi { get; set; }
        public string PotaKartAdi { get; set; }
        public int PotaKartId { get; set; }
        public int BilesenHeaderId { get; set; }
        public string BilesenHeaderAdi { get; set; }
        public int BilesenItemId { get; set; }
        public string BilesenDegeri { get; set; }
       

    }

    public class RaporlamaMakineBazliAnalizItem
    {
        public TechizatTuruTanim TechizatTuruTanim { get; set; }

        public List<RaporlamaMakineBazliAnalizPK> PkList { get; set; }
    }
    public class RaporlamaMakineBazliAnalizPK
    {
        public RaporlamaMakineBazliAnalizPK()
        {
            PotaKart = new PotaKart();
            DropBilesenHeaderItems = new List<DropItem>();
        }
        public PotaKart PotaKart { get; set; }

        public List<DropItem> DropBilesenHeaderItems { get; set; }
    }
}