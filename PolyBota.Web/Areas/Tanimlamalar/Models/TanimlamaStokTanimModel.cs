using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolyBota.DAL;

namespace PolyBota.Web.Areas.Tanimlamalar.Models
{
    public class TanimlamaStokTanimModel
    {
        public TanimlamaStokTanimModel()
        {
            StokKarts = new List<StokKart>();
            KomponentTanimGrups = new List<KomponentTanimGrup>();
            KomponentTanims = new List<KomponentTanim>();
            StokKart = new StokKart();
        }

        public int Id { get; set; }
        public List<StokKart> StokKarts { get; set; }

        public StokKart StokKart { get; set; }

        public List<KomponentTanimGrup> KomponentTanimGrups { get; set; }
        public List<KomponentTanim> KomponentTanims { get; set; }


    }
}