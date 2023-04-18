using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNSmxResolver
{
    public class RaportBody
    {
        public RaportBody()
        {
            DomainName = string.Empty;
            Type = string.Empty;
            Class = string.Empty;
            HostName = string.Empty;
        }

        public string DomainName { get; set; }
        public string Type { get; set; }

        public string Class { get; set; }

        public int TTL { get; set; }

        public int Priority { get; set; }

        public string HostName { get; set; }
    }
}
