using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyBota.Entities
{
  public  class BreadCrumb
    {
        public BreadCrumb()
        {
            
        }
        public string ParentId { get; set; }
        public string ParentName { get; set; }
        public string ChildId { get; set; }
        public string ChildName { get; set; }

        public bool SeciliMi { get; set; }
    }
}
