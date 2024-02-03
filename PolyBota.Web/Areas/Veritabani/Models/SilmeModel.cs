using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolyBota.DAL;
using PolyBota.Entities;

namespace PolyBota.Web.Areas.Veritabani.Models
{
    public class SilmeModel
    {
        public SilmeModel()
        {
            DropItems = new List<DropItem>();
            PotaKarts = new List<PotaKart>();
        }
        public List<DropItem> DropItems { get; set; }

        public List<PotaKart> PotaKarts { get; set; }
    }
}