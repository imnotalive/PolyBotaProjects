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
    
    public partial class ArizaBildirimHeader
    {
        public int Id { get; set; }
        public System.DateTime KayitTarihi { get; set; }
        public int KayitYapan { get; set; }
        public int ArizaDurumu { get; set; }
        public int KapananArizaTanimId { get; set; }
        public int AcilanArizaTanimId { get; set; }
        public string AcilanArizaNotu { get; set; }
        public int ArizaCozumId { get; set; }
        public string KapananArizaNotu { get; set; }
        public int SilinsinMi { get; set; }
        public int ArizaTechizatGrubuId { get; set; }
        public int AktarilanArizaHeaderId { get; set; }
    }
}
