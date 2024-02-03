using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using PolyBota.DAL;
using PolyBota.Entities;

namespace PolyBota.Web.Areas.Tanimlamalar.Models
{
    public class TanimlamaTechizatTuruModel
    {
        public TanimlamaTechizatTuruModel()
        {
            DropTechizatTuruTanims = new List<DropItem>();
            TechizatTuruTanim = new TechizatTuruTanim();
            BilesenHeaders = new List<BilesenHeader>();
        }
        public List<DropItem> DropTechizatTuruTanims { get; set; }

        public List<BilesenHeader> BilesenHeaders { get; set; }
        public TechizatTuruTanim TechizatTuruTanim { get; set; }


    }
}