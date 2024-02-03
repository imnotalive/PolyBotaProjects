using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolyBota.DAL;

namespace PolyBota.Web.Areas.Tanimlamalar.Models
{
    public class TanimlamaZeModel
    {
        public TanimlamaZeModel()
        {
            ZamanEtuduKategoris = new List<ZamanEtuduKategori>();
            ZamanEtuduKategori = new ZamanEtuduKategori();
        }
        public List<ZamanEtuduKategori> ZamanEtuduKategoris { get; set; }

        public ZamanEtuduKategori ZamanEtuduKategori { get; set; }  
    }
}