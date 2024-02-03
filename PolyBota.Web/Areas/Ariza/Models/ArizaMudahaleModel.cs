using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows;
using Microsoft.Office.Interop.Excel;
using PolyBota.DAL;
using PolyBota.Entities;

namespace PolyBota.Web.Areas.Ariza.Models
{
    public class ArizaMudahaleModel:PagedListSrcn
    {
        public ArizaMudahaleModel()
        {
            DropArizaDurumTanims = new List<DropItem>();
            DropArizaOzets = new List<DropItem>();
            ArizaDurumModelItems = new List<ArizaDurumModelItem>();
            PotaKarts = new List<PotaKart>();
            TechizatTuruTanims = new List<TechizatTuruTanim>();
            BilesenHeaders = new List<BilesenHeader>();
            BilesenItems = new List<BilesenItem>();
            ArizaDurumModelItem = new ArizaDurumModelItem();
            DropArizaBildirimDurums = new List<DropItem>();
            DropArizaKapatmaAlternatifs = new List<DropItem>();
            ArizaTanims = new List<ArizaTanim>();
            DropItem = new DropItem();
            ArizaPotaKartSecimModels = new List<ArizaPotaKartSecimModel>();
            ArizaTanim = new ArizaTanim();
            ArizaTanimCozums = new List<ArizaTanimCozum>();
            DropFiltreKullanici = new DropItem();
            DropFiltreArizaGrubu = new DropItem();
            DropFiltreArizaTanimi = new DropItem();
            DropFiltrePotaKart = new DropItem();

            DropArizaDegistirme = new DropItem();

            StokKarts = new List<StokKart>();

            DropKullanilanMalzemeler = new List<DropItem>();
        }


        public DropItem DropArizaDegistirme { get; set; }
        public DropItem DropFiltreKullanici { get; set; }
        public DropItem DropFiltreArizaGrubu { get; set; }

        public DropItem DropFiltreArizaTanimi { get; set; }

        public DropItem DropFiltrePotaKart { get; set; }

        public List<ArizaTanimCozum> ArizaTanimCozums { get; set; }
        public int indexId { get; set; }
        public DropItem DropItem { get; set; }
        public ArizaTanim ArizaTanim { get; set; }
        public List<ArizaTanim> ArizaTanims { get; set; }
        public List<DropItem> DropArizaDurumTanims { get; set; }
        public List<DropItem> DropArizaKapatmaAlternatifs { get; set; }
        public List<DropItem> DropArizaOzets { get; set; }
        public List<DropItem> DropArizaBildirimDurums { get; set; }
        public List<PotaKart> PotaKarts { get; set; }

        public List<ArizaDurumModelItem> ArizaDurumModelItems { get; set; }

        public List<TechizatTuruTanim> TechizatTuruTanims { get; set; }

        public List<BilesenHeader> BilesenHeaders { get; set; }

        public List<BilesenItem> BilesenItems { get; set; }

        public ArizaDurumModelItem ArizaDurumModelItem { get; set; }

        public List<ArizaPotaKartSecimModel> ArizaPotaKartSecimModels { get; set; }


        public List<StokKart> StokKarts { get; set; }

        public List<DropItem> DropKullanilanMalzemeler { get; set; }
    }
}