using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolyBota.DAL;

namespace PolyBota.Web.Areas.Tanimlamalar.Models
{
    public class TanimlamaDepartmanModel
    {
        public TanimlamaDepartmanModel()
        {
            Departmans = new List<Departman>();
            Departman = new Departman();
        }
        public List<Departman> Departmans { get; set; }

        public Departman Departman { get; set; }
    }
}