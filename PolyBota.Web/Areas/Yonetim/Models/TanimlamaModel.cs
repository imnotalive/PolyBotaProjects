using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PolyBota.DAL;
using PolyBota.Entities;

namespace PolyBota.Web.Areas.Yonetim.Models
{
    public class TanimlamaModel
    {
        public TanimlamaModel()
        {
            AdminModulListe = new List<SystemModul>();
            LinkGrupTanimListe = new List<SystemLinkGrupTanim>();
            BaseUrlLinkler = new List<SystemBaseUrlLink>();
            RoleTanimListe = new List<SystemRoleTanim>();
            GenelDropItemList1 = new List<DropItem>();

            AdminModul = new SystemModul();
            LinkGrupTanim = new SystemLinkGrupTanim();
            BaseUrlLink = new SystemBaseUrlLink();
            RoleTanim = new SystemRoleTanim();
            User = new User();
            DropItems = new List<DropItem>();
        }

        public List<DropItem> DropItems { get; set; }
        public List<SystemModul> AdminModulListe { get; set; }
        public List<SystemLinkGrupTanim> LinkGrupTanimListe { get; set; }

        public List<SystemRoleTanim> RoleTanimListe { get; set; }
        public List<SystemBaseUrlLink> BaseUrlLinkler { get; set; }


        public List<ModulLinkGrupModel> ModulLinkGrupModeller { get; set; }


        public SelectList GenelSelect1 { get; set; }
        public SystemModul AdminModul { get; set; }
        public SystemLinkGrupTanim LinkGrupTanim { get; set; }

        public SystemRoleTanim RoleTanim { get; set; }
        public SystemBaseUrlLink BaseUrlLink { get; set; }

        public List<DropItem> GenelDropItemList1 { get; set; }

        public User User { get; set; }
        public List<User> UserList { get; set; }
    }

    public class ModulLinkGrupModel
    {
        public ModulLinkGrupModel()
        {
            AdminModul = new SystemModul();
            LinkGrupBaseUrlListe = new List<LinkGrupBaseUrl>();
        }
        public SystemModul AdminModul { get; set; }

        public List<LinkGrupBaseUrl> LinkGrupBaseUrlListe { get; set; }

    }
    public class LinkGrupBaseUrl
    {
        public LinkGrupBaseUrl()
        {
            ModulLinkGrupRoleBaseUrlListe = new List<ModulLinkGrupRoleBaseUrl>();
            LinkGrupTanim = new SystemLinkGrupTanim();
        }
        public SystemLinkGrupTanim LinkGrupTanim { get; set; }

        public List<ModulLinkGrupRoleBaseUrl> ModulLinkGrupRoleBaseUrlListe { get; set; }

    }
    public class ModulLinkGrupRoleBaseUrl
    {
        public ModulLinkGrupRoleBaseUrl()
        {
            AdminModul = new SystemModul();
            LinkGrupTanim = new SystemLinkGrupTanim();

            RoleTanim = new SystemRoleTanim();
            RoleBaseUrlLink = new SystemRoleBaseUrlLink();

            BaseUrlLink = new SystemBaseUrlLink();
            User = new User();
        }
        public SystemModul AdminModul { get; set; }

        public SystemLinkGrupTanim LinkGrupTanim { get; set; }

        public SystemRoleTanim RoleTanim { get; set; }

        public SystemRoleBaseUrlLink RoleBaseUrlLink { get; set; }

        public SystemBaseUrlLink BaseUrlLink { get; set; }

        public User User { get; set; }

        public bool SeciliMi { get; set; }

        public LinkTanim LinkTanim { get; set; }


    }
}