//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PolyBota.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class ZamanEtuduItemDetail
    {
        public int Id { get; set; }
        public System.DateTime Tarih { get; set; }
        public int PotaKartId { get; set; }
        public int BilesenItemId { get; set; }
        public string Aciklama { get; set; }
        public int UserId { get; set; }
        public decimal OlcumDegeri { get; set; }
        public int KayitYapanId { get; set; }
        public int ZamanEtuduKategoriId { get; set; }
    }
}
