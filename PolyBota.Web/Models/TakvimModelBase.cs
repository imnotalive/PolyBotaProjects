using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolyBota.DAL;
using PolyBota.Entities;

namespace PolyBota.Web.Models
{
    public class TakvimModelBase
    {

        public TakvimModelBase()
        {
            BaslangisTarihi = new DateTime(DateTime.Now.Year, 1, 1);
            BitisTarihi = new DateTime(DateTime.Now.Year + 1, 1, 1);
            GosterimSekli = 1;
            GosterimList = new List<DropItem>()
            {
                new DropItem(){Id = "0", Tanim = "Aylık"},
                new DropItem(){Id = "1", Tanim = "Haftalık"},
                new DropItem(){Id = "2", Tanim = "Günlük"},
            };
            TableHeaderlar = new List<DropItem>();
        }
        public DateTime BaslangisTarihi { get; set; }

        public DateTime BitisTarihi { get; set; }
        public List<DropItem> GosterimList { get; set; }

        public int GosterimSekli { get; set; }

        public List<DropItem> TableHeaderlar { get; set; }
    }


    public class TakvimModelSatirItem
    {
        public TakvimModelSatirItem()
        {
            StokKartOperasyons = new List<StokKartOperasyon>();
            StokKartDurus = new List<StokKartDuru>();
        }
        public StokKart StokKart { get; set; }

        public DateTime Baslangic { get; set; }
        public DateTime Bitis { get; set; }

        public List<StokKartOperasyon> StokKartOperasyons { get; set; }



        public List<StokKartDuru> StokKartDurus { get; set; }
    }
}