using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolyBota.DAL;
using PolyBota.Entities;

namespace PolyBota.Web.Areas.Raporlama.Models
{
    public class RaporlamaZeModel
    {
        public RaporlamaZeModel()
        {
            DropItems = new List<DropItem>();
            DropUsers = new List<DropItem>();
            DropZeKategoriler = new List<DropItem>();

            BaslangicTarihi = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            BitisTarihi = DateTime.Now;

            RaporlamaZePotakartItems = new List<RaporlamaZePotakartItem>();
            RaporlamaZeSonucItems = new List<RaporlamaZeSonucItem>();
            DropFiltrePotaKart = new DropItem();
        }

        public DateTime BaslangicTarihi { get; set; }

        public DateTime BitisTarihi { get; set; }

        public DropItem DropFiltrePotaKart { get; set; }

        public List<DropItem> DropZeKategoriler { get; set; }
        public List<DropItem> DropUsers { get; set; }
        public List<DropItem> DropItems { get; set; }
        public List<RaporlamaZePotakartItem> RaporlamaZePotakartItems { get; set; }
        public List<RaporlamaZeSonucItem> RaporlamaZeSonucItems { get; set; }

        public int maxGozlemSayisi { get; set; }
    }

    public class RaporlamaZeSonucItem
    {
        public RaporlamaZeSonucItem()
        {
            PotaKart = new PotaKart();
            BilesenItem = new BilesenItem();
            BilesenHeader = new BilesenHeader();
            ZamanEtuduKategori = new ZamanEtuduKategori();
            DropItems = new List<DropItem>();
            RaporlamaZeSonucItemDetails = new List<RaporlamaZeSonucItemDetail>();
        }
        public DateTime Tarih { get; set; }
        public PotaKart PotaKart { get; set; }
 
        public BilesenItem BilesenItem { get; set; }

        public BilesenHeader BilesenHeader { get; set; }
        public ZamanEtuduKategori ZamanEtuduKategori { get; set; }

        public List<DropItem> DropItems { get; set; }

        public List<RaporlamaZeSonucItemDetail> RaporlamaZeSonucItemDetails { get; set; }

      
    }

    public class RaporlamaZeSonucItemDetail
    {
        public RaporlamaZeSonucItemDetail()
        {
            ZamanEtuduItemDetail = new ZamanEtuduItemDetail();
           
            UserGozleyen = new User();
       
        }
        public ZamanEtuduItemDetail ZamanEtuduItemDetail { get; set; }

      

        public User UserGozleyen { get; set; }

   
    }

    public class RaporlamaZePotakartItem
    {
        public RaporlamaZePotakartItem()
        {
            DropBilesenHeaderItems = new List<DropItem>();
            DropItemPotaKart = new DropItem();
        }
        public DropItem DropItemPotaKart { get; set; }

        public List<DropItem> DropBilesenHeaderItems { get; set; }
    }
    public class RaporlamaPkVeBilesenItem
    {
        public int PotaKartId { get; set; }
        public int BilesenItemId { get; set; }
        public int BilesenHeaderId { get; set; }

        public string PotaKartAdi { get; set; }
        public string BilesenHeaderAdi { get; set; }
        public string BilesenDegeri { get; set; }

    }
}