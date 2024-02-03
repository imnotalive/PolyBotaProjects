using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolyBota.DAL;

namespace PolyBota.Web.Areas.Tanimlamalar.Models
{
    public class TanimlamaBolumModel
    {
        public TanimlamaBolumModel()
        {
            Bolums = new List<Bolum>();
            Departmans = new List<Departman>();
            Bolum = new Bolum();
            Departman = new Departman();
        }
        public List<Bolum> Bolums { get; set; }

        public Bolum Bolum { get; set; }

        public Departman Departman { get; set; }

        public List<Departman> Departmans { get; set; }
    }
}