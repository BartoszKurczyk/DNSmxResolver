using DNSmxResolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNSmxTests
{
    [TestClass]
    public class DNSmxResolverTests
    {
        [TestMethod]
        public void ResolveGmail()
        {
            TestableDNSResolver resolver = new TestableDNSResolver("gmail.com");

            resolver.Resolve();

            var header = resolver.RaportHeader;

            Assert.IsNotNull(header);
            Assert.IsTrue(header.DomainName.Equals("gmail.com"));
            Assert.IsTrue(header.Type.Equals("MX"));
            Assert.IsTrue(header.Class.Equals("IN"));

            var bodies = resolver.RaportBodies;

            Assert.IsTrue(bodies.All(x=>x.DomainName.Equals("gmail.com")));
            Assert.IsTrue(bodies.All(x => x.Type.Equals("MX")));
            Assert.IsTrue(bodies.All(x => x.Class.Equals("IN")));

        }
    }

    public class TestableDNSResolver : DNSResolver
    {
        public TestableDNSResolver(string hostToResolve, string dnsServerIP = "8.8.8.8") : base(hostToResolve, dnsServerIP)
        {
        }

        public RaportHeader RaportHeader
        {
            get
            {
                return _raportHeader;
            }
        }

        public List<RaportBody> RaportBodies
        {
            get
            {
                return _raportBodies;
            }
        }
    }
}
