using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolyBota.DAL;
using PolyBota.Entities;

namespace PolyBota.Web.Areas.Endustri.Models
{
    public class ZamanEtuduModel
    {
        public ZamanEtuduModel()
        {

            PotaKarts = new List<PotaKart>();
            ZamanEtuduItems = new List<ZamanEtuduItem>();
            DropItem = new DropItem();
            ZamanEtuduHeader = new ZamanEtuduHeader(){Tarih = DateTime.Now};
            ZamanEtuduModelItem = new ZamanEtuduModelItem();
            ZamanEtuduModelItems = new List<ZamanEtuduModelItem>();
            PagedListSrcn = new PagedListSrcn();
            DropItems = new List<DropItem>();
            Users = new List<User>();
            ZamanEtuduItemDetail = new ZamanEtuduItemDetail(){Tarih = DateTime.Now};
            ZamanEtuduItemDetails = new List<ZamanEtuduItemDetail>();
            ZamanEtuduKategoris = new List<ZamanEtuduKategori>();
        }

        public List<ZamanEtuduItemDetail> ZamanEtuduItemDetails { get; set; }

        public ZamanEtuduItemDetail ZamanEtuduItemDetail { get; set; }
        public List<ZamanEtuduKategori> ZamanEtuduKategoris { get; set; }
        public List<User> Users { get; set; }
        public int KatId { get; set; }
        public List<DropItem> DropItems { get; set; }
        public PagedListSrcn PagedListSrcn { get; set; }
        public DropItem DropItem { get; set; }

        public List<PotaKart> PotaKarts { get; set; }

        public ZamanEtuduHeader ZamanEtuduHeader { get; set; }


        public List<ZamanEtuduItem> ZamanEtuduItems { get; set; }

        public List<ZamanEtuduModelItem> ZamanEtuduModelItems { get; set; }

        public ZamanEtuduModelItem ZamanEtuduModelItem { get; set; }

    }

    public class ZamanEtuduModelItem
    {
        public ZamanEtuduModelItem()
        {
            ZamanEtuduItemDegers = new List<ZamanEtuduItemDeger>();
            ZamanEtuduItem = new ZamanEtuduItem();
            DropItem = new DropItem();
            DropItems = new List<DropItem>();
        }
        public ZamanEtuduItem ZamanEtuduItem { get; set; }
        public List<DropItem> DropItems { get; set; }
        public DropItem DropItem { get; set; }
        public List<ZamanEtuduItemDeger> ZamanEtuduItemDegers { get; set; }
    }

}