using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyBota.Entities
{
    public class DatabaseConnectionRecord
    {
        public string Id { get; set; }
        public string RecordedDate { get; set; }
        public string IpAdress { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DatabaseName { get; set; }
        public string PortNo { get; set; }
    }
}
