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
    
    public partial class StokKartDuru
    {
        public int Id { get; set; }
        public int StokKartId { get; set; }
        public int DurusTipi { get; set; }
        public System.DateTime DurusBaslangic { get; set; }
        public System.DateTime DurusBitis { get; set; }
        public decimal ToplamSureDk { get; set; }
        public int DurusGrubuId { get; set; }
    }
}
