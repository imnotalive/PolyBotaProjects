using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using PolyBota.DAL;

namespace PolyBota.Web.Areas.Tanimlamalar.Models
{
    public class TanimlamaKomponentModel
    {
        public TanimlamaKomponentModel()
        {
            KomponentTanims = new List<KomponentTanim>();
            KomponentTanimGrups = new List<KomponentTanimGrup>();
            KomponentTanimGrup = new KomponentTanimGrup();
            KomponentTanim = new KomponentTanim();
        }
        public List<KomponentTanim> KomponentTanims { get; set; }
        public KomponentTanim KomponentTanim { get; set; }

        public List<KomponentTanimGrup> KomponentTanimGrups { get; set; }

        public KomponentTanimGrup KomponentTanimGrup { get; set; }

    }
}