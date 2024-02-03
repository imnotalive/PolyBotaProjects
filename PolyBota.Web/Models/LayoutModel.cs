using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolyBota.DAL;
using PolyBota.Entities;

namespace PolyBota.Web.Models
{
    public class LayoutModel
    {
        public LayoutModel()
        {
            DilPaketi = new List<DropItem>()
            {
                new DropItem(){Tanim = "TÜRKÇE", Id = "tr"},
                new DropItem(){Tanim = "ENGLISH", Id = "en"},
            };
            KullaniciLinkleri = new List<BaseUserLink>();
            User = new User();
            UserHeaderItemListe = new List<UserHeaderItem>();
            Haftalar = new List<int>();
            Gunler = new List<DateTime>();
            DropBildirimLinkler = new List<DropItem>();
        }
        public User User { get; set; }

        public string LangShortDef { get; set; }
        public List<DropItem> DropHizliArama { get; set; }
        public List<DropItem> DropBildirimLinkler { get; set; }
        public List<BaseUserLink> KullaniciLinkleri { get; set; }
        public List<DropItem> DilPaketi { get; set; }
        public string ResimUrl { get; set; }
        public List<UserHeaderItem> UserHeaderItemListe { get; set; }

        public List<int> Haftalar { get; set; }

        public List<DateTime> Gunler { get; set; }
    }

    public class UserHeaderItem
    {
        public UserHeaderItem()
        {
            UserLinkList = new List<BaseUserLink>();
            LinkGrupTanim = new SystemLinkGrupTanim();
        }

        public SystemLinkGrupTanim LinkGrupTanim { get; set; }

        public List<BaseUserLink> UserLinkList { get; set; }
    }

    public class UserYetkiSorguItem
    {
        public int RoleTanimId { get; set; }

        public int LinkGrupTanimId { get; set; }
        public string LinkGrupTanimAdi { get; set; }
        public string LinkGrupTanimAdiEng { get; set; }
        public string IclassName { get; set; }
        public string AreaName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string LinkTanimAdi { get; set; }

        public string LinkTanimAdiEng { get; set; }
        public int DetayLinkDurumu { get; set; }


    }
}