using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolyBota.DAL;

namespace PolyBota.Web.Areas.Tanimlamalar.Models
{
    public class TanimlamaSicilOzellikModel
    {
        public TanimlamaSicilOzellikModel()
        {
            SicilOzellikTanims = new List<SicilOzellikTanim>();
            SicilOzellikHeaderTanims = new List<SicilOzellikHeaderTanim>();
            SicilOzellikHeaderTanim = new SicilOzellikHeaderTanim();
            SicilOzellikTanim = new SicilOzellikTanim();
        }
        public List<SicilOzellikHeaderTanim> SicilOzellikHeaderTanims { get; set; }
        public SicilOzellikHeaderTanim SicilOzellikHeaderTanim { get; set; }

        public List<SicilOzellikTanim> SicilOzellikTanims { get; set; }

        public SicilOzellikTanim SicilOzellikTanim { get; set; }
    }
}