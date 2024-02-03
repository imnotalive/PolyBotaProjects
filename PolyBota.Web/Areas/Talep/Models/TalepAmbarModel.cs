using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolyBota.DAL;
using PolyBota.Entities;

namespace PolyBota.Web.Areas.Talep.Models
{
    public class TalepAmbarModel
    {
        public TalepAmbarModel()
        {
            StokKarts = new List<StokKart>();
            Ambars = new List<Ambar>();
            AmbarStokKarts = new List<AmbarStokKartModel>();
            PagedListSrcn = new PagedListSrcn();
            User = new User();
            TigerAmbars = new List<Ambar>();
        }

        public List<User> Users { get; set; }
        public PagedListSrcn PagedListSrcn { get; set; }
        public List<Ambar> Ambars { get; set; }

        public List<StokKart> StokKarts { get; set; }

        public int Id { get; set; }
        public List<AmbarStokKartModel> AmbarStokKarts { get; set; }
        public List<DropItem> DropItems { get; set; }

        public StokKart StokKart { get; set; }
        public Ambar Ambar { get; set; }
        public List<AmbarStokTalep> AmbarStokTaleps { get; set; }
        public AmbarStokTalep AmbarStokTalep { get; set; }

        public List<Ambar> TigerAmbars { get; set; }

        public int tip { get; set; }

        public int durum { get; set; }

        public int MiktarInt { get; set; }
        public User User { get; set; }
    }


    public class AmbarStokKartModel
    {
        public AmbarStokKartModel()
        {
            StokKart = new StokKart();
            PTKSAmbarStokKarts = new List<AmbarStokKart>();
            AmbarStokKarts = new List<AmbarStokKart>();
        }
        public StokKart StokKart { get; set; }
        public List<AmbarStokKart> PTKSAmbarStokKarts { get; set; }
        public List<AmbarStokKart> AmbarStokKarts { get; set; }
    }

 
}