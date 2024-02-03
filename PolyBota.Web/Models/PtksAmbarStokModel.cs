using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolyBota.DAL;
using PolyBota.Entities;

namespace PolyBota.Web.Models
{
    public class PtksAmbarStokModel
    {
        public PtksAmbarStokModel()
        {
            Ambar = new Ambar();
            StokKoduMiktars = new List<DropItem>();
        }
        public Ambar Ambar { get; set; }

        public List<DropItem> StokKoduMiktars { get; set; }
    }
}