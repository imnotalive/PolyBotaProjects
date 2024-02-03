using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolyBota.DAL;
using PolyBota.Entities;

namespace PolyBota.Web.Areas.Takip.Models
{
    public class DurusStokKartModel : TakipModelBase
    {
        public DurusStokKartModel()
        {
            DropStokKarts = new List<DropItem>();
            StokKartDuru = new StokKartDuru();
            DurusGrubuTanims = new List<DurusGrubuTanim>();
            DurusTipiTanims = new List<DurusTipiTanim>();
            StokKart = new StokKart();
        }
        public int Id { get; set; }

        public int SecimId { get; set; }
        public List<DropItem> DropStokKarts { get; set; }

        public List<List<TakipTakvimModel>> TakipTakvimModels { get; set; }

        public StokKartDuru StokKartDuru { get; set; }

        public List<DurusGrubuTanim> DurusGrubuTanims { get; set; }

        public List<DurusTipiTanim> DurusTipiTanims { get; set; }

        public StokKart StokKart { get; set; }
    }
}