using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolyBota.DAL;
using PolyBota.Entities;

namespace PolyBota.Web.Areas.Tanimlamalar.Models
{
    public class TanimlamaAmbarModel
    {
        public TanimlamaAmbarModel()
        {
            AmbarTipiTanims = new List<AmbarTipiTanim>();
              Ambars = new List<Ambar>();
            Ambar = new Ambar();
        }
        public List<AmbarTipiTanim> AmbarTipiTanims { get; set; }
        public List<Ambar> Ambars { get; set; }

        public Ambar Ambar { get; set; }

        public int Id { get; set; }
    }
}