using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolyBota.DAL;

namespace PolyBota.Web.Areas.Tanimlamalar.Models
{
    public class TanimlamaTalimatModel
    {
        public TanimlamaTalimatModel()
        {
            TalimatGrupTanims = new List<TalimatGrupTanim>();
            TalimatTanims = new List<TalimatTanim>();

            TalimatGrupTanim = new TalimatGrupTanim();
            TalimatTanim = new TalimatTanim();
        }
        public List<TalimatGrupTanim> TalimatGrupTanims { get; set; }
        public TalimatGrupTanim TalimatGrupTanim { get; set; }

        public List<TalimatTanim> TalimatTanims { get; set; }
        public TalimatTanim TalimatTanim { get; set; }
    }
}