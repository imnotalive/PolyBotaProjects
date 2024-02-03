using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using PolyBota.DAL;
using PolyBota.Entities;
using PolyBota.Helpers;
using PolyBota.Web.Areas.Yonetim.Models;
using PolyBota.Web.Controllers;

namespace PolyBota.Web.Areas.Yonetim.Controllers
{
    public class TanimlamalarController : BaseController
    {
        #region Metotlar

        public void LinkTanimKontrol(List<LinkTanim> MustBeLinkler)
        {
            #region Deneme

            var tanimliListe = this._db.SystemBaseUrlLinks.ToList();
            var eklenecekListe = new List<LinkTanim>();
            var ortakmustBeListe = new List<LinkTanim>();
            var silinecekBaseListe = new List<SystemBaseUrlLink>();
            var ortakTanimliListe = new List<SystemBaseUrlLink>();
            foreach (var mustbe in MustBeLinkler)
            {
                if (tanimliListe.Any(a => a.ActionName == mustbe.ActionName && a.ControllerName == mustbe.ControllerName && a.AreaName == mustbe.AreaName))
                {
                    ortakmustBeListe.Add(mustbe);
                }
                else
                {
                    eklenecekListe.Add(mustbe);
                }
            }

            foreach (var tanimli in tanimliListe)
            {
                if (MustBeLinkler.Any(a => a.ActionName == tanimli.ActionName && a.ControllerName == tanimli.ControllerName && a.AreaName == tanimli.AreaName))
                {
                    ortakTanimliListe.Add(tanimli);
                }
                else
                {
                    silinecekBaseListe.Add(tanimli);
                }
            }

            #endregion Deneme

            if (eklenecekListe.Any())
            {
                var topluLinkler = new List<SystemBaseUrlLink>();
                foreach (var item in eklenecekListe)
                {
                    topluLinkler.Add(new SystemBaseUrlLink()
                    {
                        ActionName = item.ActionName,
                        ControllerName = item.ControllerName,
                        AreaName = item.AreaName,

                        LinkGrupTanimId = 0,
                        LinkTanimAdi = "",
                        LinkTanimAdiEng = "",
                        DetayLinkDurumu = 0
                    });
                }

                this._db.SystemBaseUrlLinks.AddRange(topluLinkler);
                this._db.SaveChanges();
            }

            if (silinecekBaseListe.Any())
            {
                var idler = silinecekBaseListe.Select(a => a.Id).Distinct().ToList();

                var roleBaseler = this._db.SystemRoleBaseUrlLinks
                    .Where(a => idler.Any(b => b == a.BaseUrlLinkId)).ToList();
                this._db.SystemRoleBaseUrlLinks.RemoveRange(roleBaseler);
                this._db.SaveChanges();
                this._db.SystemBaseUrlLinks.RemoveRange(silinecekBaseListe);
                this._db.SaveChanges();
            }
            var liste = new List<string>();
            int i = 0;
            foreach (var item in eklenecekListe)
            {
                i++;
                var detayLink = string.Format("{3}. {0}/{1}/{2}", item.AreaName, item.ControllerName, item.ActionName, i);
                liste.Add(detayLink);
            }

            ViewBag.liste = liste;
        }

        public void LinkUpdate()
        {
            Assembly asm = Assembly.GetAssembly(typeof(MvcApplication));

            var TumListe = asm.GetTypes().
                SelectMany(t => t.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                .Where(d => d.ReturnType.Name == "ActionResult").Select(n => new
                {
                    Controller = n.DeclaringType?.Name.Replace("Controller", ""),
                    Action = n.Name,
                    ReturnType = n.ReturnType.Name,
                    Attributes = string.Join(",", n.GetCustomAttributes().Select(a => a.GetType().Name.Replace("Attribute", ""))),
                    Area = n.DeclaringType.Namespace.ToString().Replace("PolyBota.Web" + ".", "").Replace("Areas.", "").Replace(".Controllers", "").Replace("Controllers", "")
                }).OrderByDescending(a => a.Area).ToList();

            var rejectList = TumListe.Where(a => a.Attributes.Contains("HttpPost")).ToList();

            TumListe = TumListe.Except(rejectList).ToList();

            var mustbeList = TumListe.Select(
                    a => new LinkTanim { ActionName = a.Action, AreaName = a.Area, ControllerName = a.Controller })
                .ToList();
            LinkTanimKontrol(mustbeList);
        }

        #endregion Metotlar

        // GET: Yonetim/Tanimlamalar

        #region Modül CRUD

        public ActionResult ModulListe()
        {
            var model = new TanimlamaModel()
            {
                AdminModulListe = this._db.SystemModuls.OrderBy(a => a.ModulTanim).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult ModulEkleGuncelle(SystemModul model)
        {
            model.ModulTanim = model.ModulTanim.ToUpper();
            if (model.Id == 0)
            {
                this._db.SystemModuls.Add(model);
                this._db.SaveChanges();
                TempDataCreate(1);
            }
            else
            {
                var edtiItem = this._db.SystemModuls.Find(model.Id);
                edtiItem.ModulTanim = model.ModulTanim;
                this._db.SaveChanges();
                TempDataCreate(2);
            }

            return RedirectToAction("ModulListe");
        }

        #endregion Modül CRUD

        #region Base Link CRUD

        public ActionResult LinkListe()
        {
            LinkUpdate();

            var model = new TanimlamaModel()
            {
                AdminModulListe = this._db.SystemModuls.OrderBy(a => a.ModulTanim).ToList(),
                LinkGrupTanimListe = _db.SystemLinkGrupTanims.OrderBy(a => a.ModulAdi).ThenBy(a => a.LinkGrupTanimAdi).ToList()
            };
            model.BaseUrlLinkler = _db.SystemBaseUrlLinks
                .OrderBy(a => a.LinkTanimAdi)
                .ThenBy(a => a.AreaName)
                .ThenBy(a => a.ControllerName)
                .ThenBy(a => a.ActionName)
                .ToList();

            var dropModulListe = model.LinkGrupTanimListe
                .Select(a => new DropItem() { Tanim = string.Format("{0} - {1}", a.ModulAdi, a.LinkGrupTanimAdi), IdInt = a.Id }).ToList();

            model.GenelDropItemList1 = dropModulListe;

            return View(model);
        }

        [HttpPost]
        public ActionResult LinkListeGuncelle(TanimlamaModel model)
        {
            var guncellenecekListe = model.BaseUrlLinkler.Where(a => a.LinkTanimAdi != null && a.LinkGrupTanimId != 0)
                .ToList();
            foreach (var item in guncellenecekListe)
            {
                var editItem = this._db.SystemBaseUrlLinks.Find(item.Id);
                if (editItem.LinkTanimAdi != item.LinkTanimAdi || editItem.LinkGrupTanimId != item.LinkGrupTanimId ||
                    editItem.LinkTanimAdiEng != item.LinkTanimAdiEng || editItem.DetayLinkDurumu != item.DetayLinkDurumu)
                {
                    // var linkGup = _db.SCHWEGLER_Pera_WebPortalAdminLinkGrupTan.Find(item.LinkGrupTanimId);
                    //editItem.AdminModulAdi = linkGup.AdminModulAdi;
                    //editItem.AdminModulId = linkGup.AdminModulId;

                    // editItem.LinkGrupTanimAdi = linkGup.LinkGrupTanimAdi;
                    editItem.LinkGrupTanimId = item.LinkGrupTanimId;
                    editItem.LinkTanimAdi = item.LinkTanimAdi;
                    editItem.LinkTanimAdiEng = item.LinkTanimAdiEng;
                    editItem.DetayLinkDurumu = item.DetayLinkDurumu;
                    this._db.SaveChanges();
                }
            }

            TempDataCreate(2);
            return RedirectToAction("LinkListe");
        }

        #endregion Base Link CRUD

        #region Link Grup CRUD

        public ActionResult LinkGrupListe()
        {
            var model = new TanimlamaModel()
            {
                LinkGrupTanimListe = this._db.SystemLinkGrupTanims.OrderBy(a => a.LinkGrupTanimAdi).ThenBy(a => a.LinkGrupTanimAdi).ToList()
            };

            return View(model);
        }

        public ActionResult LinkGrupEkleDuzenle(int id = 0)
        {
            var model = new TanimlamaModel();
            var dropModulListe = this._db.SystemModuls
                .Select(a => new DropItem() { Tanim = a.ModulTanim, IdInt = a.Id }).ToList();
            model.GenelSelect1 = new SelectList(dropModulListe, "IdInt", "Tanim");

            if (id != 0)
            {
                model.LinkGrupTanim = this._db.SystemLinkGrupTanims.Find(id);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult LinkGrupEkleDuzenle(TanimlamaModel model)
        {
            var item = model.LinkGrupTanim;
            var modul = this._db.SystemModuls.Find(item.SystemModulId);

            if (item.Id == 0)
            {
                var yeniKayit = new SystemLinkGrupTanim()
                {
                    ModulAdi = modul.ModulTanim,
                    LinkGrupTanimAdi = item.LinkGrupTanimAdi,
                    LinkGrupTanimAdiEng = item.LinkGrupTanimAdiEng,
                    SystemModulId = modul.Id,
                    IclassName = item.IclassName
                };
                this._db.SystemLinkGrupTanims.Add(yeniKayit);
                this._db.SaveChanges();
                TempDataCreate(1);
            }
            else
            {
                var editKayit = this._db.SystemLinkGrupTanims.Find(item.Id);

                editKayit.LinkGrupTanimAdi = item.LinkGrupTanimAdi;
                editKayit.LinkGrupTanimAdiEng = item.LinkGrupTanimAdiEng;
                editKayit.SystemModulId = modul.Id;
                editKayit.ModulAdi = modul.ModulTanim;
                editKayit.IclassName = item.IclassName;
                this._db.SaveChanges();
                TempDataCreate(2);
            }
            return RedirectToAction("LinkGrupListe");
        }

        #endregion Link Grup CRUD

        #region Role Tanım CRUD

        public ActionResult RoleTanimListe()
        {
            var model = new TanimlamaModel()
            {
                RoleTanimListe = this._db.SystemRoleTanims.OrderBy(a => a.RoleTanimAdi).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult RoleTanimEkleGuncelle(SystemRoleTanim model)
        {
            if (model.RoleTanimAdi == null)
            {
                TempDataCreate(4);
            }
            else
            {
                if (model.Id == 0)
                {
                    if (_db.SystemRoleTanims.Any(a => a.RoleTanimAdi == model.RoleTanimAdi.ToUpper()))
                    {
                        TempDataCreate(5);
                    }
                    else
                    {
                        var yeniKayit = new SystemRoleTanim()
                        {
                            RoleTanimAdi = model.RoleTanimAdi.ToUpper(),
                            RoleTanimOzel = model.RoleTanimOzel
                        };
                        _db.SystemRoleTanims.Add(yeniKayit);
                        _db.SaveChanges();
                        TempDataCreate(1);
                    }
                }
                else
                {
                    var editItem = _db.SystemRoleTanims.First(a => a.Id == model.Id);
                    editItem.RoleTanimAdi = model.RoleTanimAdi.ToUpper();
                    editItem.RoleTanimOzel = model.RoleTanimOzel?.ToUpper();

                    this._db.SaveChanges();
                    TempDataCreate(2);
                }
            }
            return RedirectToAction("RoleTanimListe");
        }

        public ActionResult RoleTanimYetki(int id)
        {
            var model = new TanimlamaModel() { ModulLinkGrupModeller = new List<ModulLinkGrupModel>() };
            var RoleTanim = this._db.SystemRoleTanims.First(a => a.Id == id);

            model.RoleTanim = RoleTanim;

            var RoleYetkileri = this._db.SystemRoleBaseUrlLinks.Where(a => a.RoleTanimId == id).ToList();

            var tumModuller = this._db.SystemModuls.OrderBy(a => a.ModulTanim).ToList();
            var tumLinkGrupTanimlar = this._db.SystemLinkGrupTanims.ToList();

            var tumBaseLinkUrller = this._db.SystemBaseUrlLinks.ToList();

            model.LinkGrupTanimListe = tumLinkGrupTanimlar;

            foreach (var modul in tumModuller)
            {
                var modulLinkGrupModel = new ModulLinkGrupModel();
                foreach (var linkGrupTanim in tumLinkGrupTanimlar.Where(a => a.SystemModulId == modul.Id).OrderBy(a => a.LinkGrupTanimAdi).ToList())
                {
                    modulLinkGrupModel.AdminModul = modul;

                    var linkGrupBaseUrl = new LinkGrupBaseUrl()
                    {
                        LinkGrupTanim = linkGrupTanim
                    };
                    foreach (var baseLink in tumBaseLinkUrller.Where(a => a.LinkGrupTanimId == linkGrupTanim.Id).OrderBy(a => a.LinkTanimAdi).ToList())
                    {
                        var modulLinkGrupRoleBaseUrl = new ModulLinkGrupRoleBaseUrl();
                        modulLinkGrupRoleBaseUrl.AdminModul = modul;
                        modulLinkGrupRoleBaseUrl.BaseUrlLink = baseLink;
                        modulLinkGrupRoleBaseUrl.SeciliMi = RoleYetkileri.Any(a => a.BaseUrlLinkId == baseLink.Id);

                        if (modulLinkGrupRoleBaseUrl.SeciliMi)
                        {
                            modulLinkGrupRoleBaseUrl.RoleBaseUrlLink = RoleYetkileri.First(a => a.BaseUrlLinkId == baseLink.Id);
                        }
                        else
                        {
                            modulLinkGrupRoleBaseUrl.RoleBaseUrlLink = new SystemRoleBaseUrlLink()
                            {
                                BaseUrlLinkId = baseLink.Id
                            };
                        }

                        linkGrupBaseUrl.ModulLinkGrupRoleBaseUrlListe.Add(modulLinkGrupRoleBaseUrl);
                    }

                    modulLinkGrupModel.LinkGrupBaseUrlListe.Add(linkGrupBaseUrl);
                }
                model.ModulLinkGrupModeller.Add(modulLinkGrupModel);
            }
            return this.View(model);
        }

        [HttpPost]
        public ActionResult RoleTanimYetki(TanimlamaModel model)
        {
            var rolTanim = model.RoleTanim;
            var silinecekRoleYetkiIdler = new List<int>();
            var olmasiGerekenYetkiler = new List<SystemRoleBaseUrlLink>();
            var eklenecekYetkiler = new List<SystemRoleBaseUrlLink>();

            foreach (var modulLinkGrupModel in model.ModulLinkGrupModeller)
            {
                foreach (var linkGrupBaseUrl in modulLinkGrupModel.LinkGrupBaseUrlListe)
                {
                    foreach (var modulLinkGrupRoleBaseUrl in linkGrupBaseUrl.ModulLinkGrupRoleBaseUrlListe)
                    {
                        var roleBaseUrl = modulLinkGrupRoleBaseUrl.RoleBaseUrlLink;

                        if (modulLinkGrupRoleBaseUrl.SeciliMi)
                        {
                            if (roleBaseUrl.Id == 0)
                            {
                                roleBaseUrl.RoleTanimId = model.RoleTanim.Id;
                                eklenecekYetkiler.Add(roleBaseUrl);
                            }
                        }
                        else
                        {
                            if (roleBaseUrl.BaseUrlLinkId != 0)
                            {
                                roleBaseUrl.RoleTanimId = model.RoleTanim.Id;
                                silinecekRoleYetkiIdler.Add(roleBaseUrl.Id);
                            }
                        }
                    }
                }
            }

            if (silinecekRoleYetkiIdler.Any())
            {
                var silinecekRoleYetkileri =
                    this._db.SystemRoleBaseUrlLinks.Where(
                        a => silinecekRoleYetkiIdler.Any(b => b == a.Id));

                this._db.SystemRoleBaseUrlLinks.RemoveRange(silinecekRoleYetkileri);
                this._db.SaveChanges();
            }

            if (eklenecekYetkiler.Any())
            {
                this._db.SystemRoleBaseUrlLinks.AddRange(eklenecekYetkiler);
                this._db.SaveChanges();
            }
            TempDataCreate(2);
            return RedirectToAction("RoleTanimYetki", "Tanimlamalar", new { id = rolTanim.Id });
        }

        #endregion Role Tanım CRUD

        #region User Role CRUD

        public ActionResult UserSil(int id)
        {
            var user = _db.Users.Find(id);
            _db.Users.Remove(user);
            _db.SaveChanges();

            TempDataCreate(3);
            return RedirectToAction("UserRoleListe");
        }

        public ActionResult UserRoleListe()
        {
            var model = new TanimlamaModel() { UserList = new List<User>() };

            model.UserList = this._db.Users.OrderBy(a => a.Name).ToList();

            model.RoleTanimListe = this._db.SystemRoleTanims.ToList();

            return View(model);
        }

        public ActionResult UserRoleDetay(int id)
        {
            var model = new TanimlamaModel();

            var editUser = this._db.Users.First(a => a.Id == id);
            var rolIdler = editUser.UserRoleIds.StrArrayeCevir("-");

            var roller = this._db.SystemRoleTanims.OrderBy(a => a.RoleTanimAdi).ToList();

            model.User = editUser;
            foreach (var item in roller)
            {
                bool seciliMi = rolIdler.Any(a => a == item.Id.ToString());
                model.GenelDropItemList1.Add(new DropItem()
                {
                    Id = item.Id.ToString(),
                    Tanim = item.RoleTanimAdi,
                    SeciliMi = seciliMi
                });
            }

            var departmans = DropDepartmanKirilimlariGetir();

            var userDepartmans = _db.UserDepartmen.Where(a => a.UserId == id).ToList();

            foreach (var drop in departmans)
            {
                drop.SeciliMi = userDepartmans.Any(a => a.DepartmanId == drop.IdInt);
                model.DropItems.Add(drop);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult UserRoleDetay(TanimlamaModel model)
        {
            var user = model.User;
            var editUser = this._db.Users.First(a => a.Id == model.User.Id);

            var liste = model.GenelDropItemList1.Where(a => a.SeciliMi).Select(a => a.Id).ToList();

            var baseId = _db.SystemRoleTanims.First(a => a.RoleTanimOzel == "BASE").Id.ToString();

            string yetki = "";

            if (liste.Count(a => a == baseId) == 0)
            {
                liste.Add(baseId);
            }
            foreach (var i in liste)
            {
                yetki += i;

                if (i != liste.Last())
                {
                    yetki += "-";
                }
            }

            editUser.Name = user.Name;
            editUser.Title = user.Title;
            editUser.PersonelCode = user.PersonelCode;
            editUser.Password = user.Password;
            editUser.UserName = user.UserName?.ToLower();
            editUser.UserRoleIds = "-" + yetki + "-";
            editUser.ArizaMudahaleYetki = user.ArizaMudahaleYetki;
            editUser.ArizaBildirimYetki = user.ArizaBildirimYetki;
            editUser.EmailAdres = string.IsNullOrWhiteSpace(user.EmailAdres) ? "" : user.EmailAdres.Contains("@") ? user.EmailAdres : null;
            this._db.SaveChanges();

            var olmasiGerekenUserDepts = model.DropItems.Where(a => a.SeciliMi).ToList().Select(a => a.IdInt).ToList();

            var kayitliUserDepts = _db.UserDepartmen.Where(a => a.UserId == editUser.Id).ToList();

            var silinecekUserDepts = new List<UserDepartman>();
            var eklenecekUserDepts = new List<UserDepartman>();

            foreach (var item in kayitliUserDepts)
            {
                if (olmasiGerekenUserDepts.Count(a => a == item.DepartmanId) == 0)
                {
                    silinecekUserDepts.Add(item);
                }
            }

            foreach (var item in olmasiGerekenUserDepts)
            {
                if (kayitliUserDepts.Count(a => a.DepartmanId == item) == 0)
                {
                    eklenecekUserDepts.Add(new UserDepartman()
                    {
                        DepartmanId = item,
                        UserId = editUser.Id
                    });
                }
            }

            if (eklenecekUserDepts.Any())
            {
                _db.UserDepartmen.AddRange(eklenecekUserDepts);
                _db.SaveChanges();
            }

            if (silinecekUserDepts.Any())
            {
                _db.UserDepartmen.RemoveRange(silinecekUserDepts);
                _db.SaveChanges();
            }
            TempDataCreate(2);
            return RedirectToAction("UserRoleDetay", "Tanimlamalar", new { id = user.Id });
        }

        [HttpPost]
        public ActionResult UserEkle(User user)
        {
            if (string.IsNullOrEmpty(user.Name) || string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password))
            {
                TempDataCreate(4);
            }
            else
            {
                user.UserName = user.UserName.ToLower();
                if (_db.Users.Any(a => a.UserName == user.UserName))
                {
                    TempDataCustom(1, "Bu kullanıcı daha önce kullanılmıştır");
                }
                else
                {
                    _db.Users.Add(user);
                    _db.SaveChanges();
                    TempDataCreate(1);

                    return RedirectToAction("UserRoleDetay", "Tanimlamalar", new { id = user.Id });
                }
            }

            return RedirectToAction("UserRoleListe");
        }

        [HttpPost]
        public ActionResult UserResimDegistir(HttpPostedFileBase file, TanimlamaModel model)
        {
            var user = model.User;

            bool sorunVarmi = false;
            if (file != null)
            {
                if (file.ContentLength < 1)
                {
                    sorunVarmi = true;
                }
            }
            else
            {
                sorunVarmi = true;
            }

            if (sorunVarmi)
            {
                TempDataCustom(1, "Resim Yükleyiniz");
            }
            else
            {
                Guid guid = Guid.NewGuid();
                var path = "~/Upload/PersonelResimler/" + guid.ToString() + file.FileName;

                file.SaveAs(Server.MapPath(path));
                var editItem = _db.Users.Find(user.Id);
                editItem.ImageUrl = path;
                _db.SaveChanges();
                TempDataCreate(2);
            }
            return RedirectToAction("UserRoleDetay", "Tanimlamalar", new { id = user.Id });
        }

        #endregion User Role CRUD
    }
}