using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolyBota.DAL;

namespace PolyBota.Web.Areas.Tanimlamalar.Models
{
    public class TanimlamaSablonModel
    {
        public TanimlamaSablonModel()
        {
            SablonGrups = new List<SablonGrup>();
            SablonGrupItems = new List<SablonGrupItem>();
            KomponentTanims = new List<KomponentTanim>();
            KomponentTanimGrups = new List<KomponentTanimGrup>();

            SablonGrup = new SablonGrup();
            SablonGrupItem = new SablonGrupItem();
        }
        public List<SablonGrup> SablonGrups { get; set; }
        public List<SablonGrupItem> SablonGrupItems { get; set; }

        public List<KomponentTanim> KomponentTanims { get; set; }

        public List<KomponentTanimGrup> KomponentTanimGrups { get; set; }

        public SablonGrup SablonGrup { get; set; }

        public SablonGrupItem SablonGrupItem { get; set; }
    }
}