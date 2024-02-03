using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolyBota.DAL;
using PolyBota.Entities;

namespace PolyBota.Web.Areas.Takip.Models
{
    public class MakineTakipModel:TakipModelBase
    {
        public MakineTakipModel()
        {
            Departmans = new List<Departman>();
            DropBreadCrumbs = new List<DropItem>();
            Bolums = new List<Bolum>();
            StokKarts = new List<StokKart>();
            PeriyotTanims = new List<PeriyotTanim>();
            PeriyotTipiTanims = new List<PeriyotTipiTanim>();
        }
        public List<Departman> Departmans { get; set; }

        public List<DropItem> DropBreadCrumbs { get; set; }

        public int Id { get; set; }

        public List<Bolum> Bolums { get; set; }

        public Bolum Bolum { get; set; }

        public List<StokKart> StokKarts { get; set; }

        public List<DurusTipiTanim> DurusTipiTanims { get; set; }
        public List<List<TakipTakvimModel>> TakipTakvimModels { get; set; }

        public List<KomponentTalimatGrup> KomponentTalimatGrups { get; set; }

        public List<PeriyotTanim> PeriyotTanims { get; set; }

        public List<PeriyotTipiTanim> PeriyotTipiTanims { get; set; }




       
        public StokKart StokKart { get; set; }

        public KomponentTalimat KomponentTalimat { get; set; }
        public StokKartOperasyon StokKartOperasyon { get; set; }
        public PeriyotTanim PeriyotTanim { get; set; }

    }
}