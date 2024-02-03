using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Web;
using PolyBota.DAL;
using PolyBota.Entities;

namespace PolyBota.Web.Areas.Tanimlamalar.Models
{
    public class TanimlamaKompTalimatModel
    {
        public TanimlamaKompTalimatModel()
        {
            KomponentTanimGrups = new List<KomponentTanimGrup>();
            KomponentTalimatGrups = new List<KomponentTalimatGrup>();
            KomponentTalimatGrup = new KomponentTalimatGrup();
            TalimatGrupTanims = new List<TalimatGrupTanim>();
            KomponentTanims = new List<KomponentTanim>();
            KomponentTalimats = new List<KomponentTalimat>();

            KomponentTalimatGrup = new KomponentTalimatGrup();
            KomponentTalimat = new KomponentTalimat();
            DropPeriyotlar = new List<DropItem>();
            DropKomponentler = new List<DropItem>();
            TalimatTanims = new List<TalimatTanim>();

            KomponentTanim = new KomponentTanim();
            KomponentTalimat = new KomponentTalimat();
            DropBilgiDepartmanlar = new List<DropItem>();
            DropDepartmanlar = new List<DropItem>();
        }

        public List<DropItem> DropDepartmanlar { get; set; }
        public List<DropItem> DropBilgiDepartmanlar { get; set; }
        public List<KomponentTanimGrup> KomponentTanimGrups { get; set; }
        public List<KomponentTanim> KomponentTanims { get; set; }
        public List<KomponentTalimatGrup> KomponentTalimatGrups { get; set; }

        public List<KomponentTalimat> KomponentTalimats { get; set; }

        public KomponentTalimatGrup KomponentTalimatGrup { get; set; }

        public KomponentTalimat KomponentTalimat { get; set; }

        public List<DropItem> DropPeriyotlar { get; set; }
        public List<DropItem> DropKomponentler { get; set; }

        public List<TalimatGrupTanim> TalimatGrupTanims { get; set; }

        public List<TalimatTanim> TalimatTanims { get; set; }

        public KomponentTanim KomponentTanim { get; set; }
    }
}