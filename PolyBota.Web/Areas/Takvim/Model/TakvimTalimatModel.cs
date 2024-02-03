using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolyBota.DAL;
using PolyBota.Entities;
using PolyBota.Web.Models;

namespace PolyBota.Web.Areas.Takvim.Model
{
    public class TakvimTalimatModel : TakvimModelBase
    {
        public TakvimTalimatModel()
        {
            DropTalimatPeriyots = new List<DropItem>();
            PotaKarts = new List<PotaKart>();
        }
        public List<DropItem> DropTalimatPeriyots { get; set; }

        public List<PotaKart> PotaKarts { get; set; }
    }
}