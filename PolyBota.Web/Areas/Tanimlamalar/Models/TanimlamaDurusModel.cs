using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolyBota.DAL;

namespace PolyBota.Web.Areas.Tanimlamalar.Models
{
    public class TanimlamaDurusModel
    {
        public TanimlamaDurusModel()
        {
            DurusGrubuTanims = new List<DurusGrubuTanim>();
            DurusTipiTanims = new List<DurusTipiTanim>();
            DurusTipiTanim = new DurusTipiTanim();
        }

        public int Id { get; set; }
        public List<DurusGrubuTanim> DurusGrubuTanims { get; set; }
        public List<DurusTipiTanim> DurusTipiTanims { get; set; }

        public DurusTipiTanim DurusTipiTanim { get; set; }
    }
}