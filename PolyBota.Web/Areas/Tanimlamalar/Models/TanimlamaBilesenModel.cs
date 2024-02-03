using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolyBota.DAL;

namespace PolyBota.Web.Areas.Tanimlamalar.Models
{
    public class TanimlamaBilesenModel
    {
        public TanimlamaBilesenModel()
        {
            BilesenCinsiGrups = new List<BilesenCinsiGrup>();
            BilesenCinsis = new List<BilesenCinsi>();
            BilesenCinsis = new List<BilesenCinsi>();
            BilesenCinsiGrup = new BilesenCinsiGrup();
        }
        public List<BilesenCinsiGrup> BilesenCinsiGrups { get; set; }
        public BilesenCinsiGrup BilesenCinsiGrup { get; set; }

        public List<BilesenCinsi> BilesenCinsis { get; set; }

        public BilesenCinsi BilesenCinsi { get; set; }
    }
}