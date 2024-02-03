using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyBota.Entities
{
    public class BaseUserLink
    {
        public int BaseUrlLinkId { get; set; }
    public int LinkGrupTanimId { get; set; }
    public string LinkGrupTanimAdi { get; set; }
    public int AdminModulId { get; set; }
    public string AdminModulAdi { get; set; }
    public string AreaName { get; set; }
    public string ControllerName { get; set; }
    public string ActionName { get; set; }
    public string LinkTanimAdi { get; set; }
    public string LinkTanimAdiEng { get; set; }
    public int DetayLinkDurumu { get; set; }
}
}
