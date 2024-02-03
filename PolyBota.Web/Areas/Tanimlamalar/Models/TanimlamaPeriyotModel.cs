using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolyBota.DAL;

namespace PolyBota.Web.Areas.Tanimlamalar.Models
{
    public class TanimlamaPeriyotModel
    {
        public TanimlamaPeriyotModel()
        {
            PeriyotTipiTanims = new List<PeriyotTipiTanim>();
            PeriyotTanims = new List<PeriyotTanim>();
            PeriyotTanim = new PeriyotTanim();
        }
        public List<PeriyotTipiTanim> PeriyotTipiTanims { get; set; }

        public List<PeriyotTanim> PeriyotTanims { get; set; }

        public PeriyotTanim PeriyotTanim { get; set; }

        public int Id { get; set; }
    }
}