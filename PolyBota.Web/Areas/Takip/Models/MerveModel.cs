using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolyBota.DAL;
using PolyBota.Entities;

namespace PolyBota.Web.Areas.Takip.Models
{
    public class MerveModel
    {
        public MerveModel()
        {
            Users = new List<User>();
            DropItems = new List<DropItem>();
            User = new User();
        }

        public List<User> Users { get; set; }

        public User User { get; set; }

        public ToDoModel ToDoModel { get; set; }

        public List<DropItem> DropItems { get; set; }


    }
}