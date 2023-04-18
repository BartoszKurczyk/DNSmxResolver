using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNSmxResolver
{
    public class RaportHeader
    {
        public RaportHeader() 
        {
            DomainName = string.Empty;
            Type = string.Empty;
            Class = string.Empty;
        }
        public string DomainName { get; set; }
        public string Type { get; set; }

        public string Class { get; set; }
    }
}
