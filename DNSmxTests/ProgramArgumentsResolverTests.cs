

namespace DNSmxTests
{
    [TestClass]
    public class ProgramArgumentsResolverTests
    {
        [TestCleanup()]
        public void Cleanup()
        {
            ProgramArgumensResolver.hosts.Clear();
            ProgramArgumensResolver.inputFile = string.Empty;
            ProgramArgumensResolver.outputFile = string.Empty;
            ProgramArgumensResolver.argsValid = false;
            ProgramArgumensResolver.errorMessage = string.Empty;
        }
        [TestMethod]
        public void WithOneDomain()
        {
            string[] args = { "gmail.com" };

            ProgramArgumensResolver.ResolveArgs(args);

            Assert.IsTrue(ProgramArgumensResolver.argsValid, "");
            Assert.IsTrue(ProgramArgumensResolver.hosts.Count == 1, "");
            Assert.IsTrue(ProgramArgumensResolver.hosts[0].Equals("gmail.com"));
        }

        [TestMethod]
        public void WithMultipleDomain()
        {
            string[] args = { "gmail.com", "hotmail.com", "aol.com", "yahoo.com" };

            ProgramArgumensResolver.ResolveArgs(args);

            Assert.IsTrue(ProgramArgumensResolver.argsValid, "");
            Assert.IsTrue(ProgramArgumensResolver.hosts.Count == 4, "");
            Assert.IsTrue(ProgramArgumensResolver.hosts[0].Equals("gmail.com"));
            Assert.IsTrue(ProgramArgumensResolver.hosts[1].Equals("hotmail.com"));
            Assert.IsTrue(ProgramArgumensResolver.hosts[2].Equals("aol.com"));
            Assert.IsTrue(ProgramArgumensResolver.hosts[3].Equals("yahoo.com"));
        }

        [TestMethod]
        public void WithDnsFlagAndMultipleDomain()
        {
            string[] args = { "gmail.com", "hotmail.com", "aol.com", "yahoo.com", "-dns", "8.8.8.8" };

            ProgramArgumensResolver.ResolveArgs(args);

            Assert.IsTrue(ProgramArgumensResolver.argsValid);
            Assert.IsTrue(ProgramArgumensResolver.hosts.Count == 4);
            Assert.IsTrue(ProgramArgumensResolver.hosts[0].Equals("gmail.com"));
            Assert.IsTrue(ProgramArgumensResolver.hosts[1].Equals("hotmail.com"));
            Assert.IsTrue(ProgramArgumensResolver.hosts[2].Equals("aol.com"));
            Assert.IsTrue(ProgramArgumensResolver.hosts[3].Equals("yahoo.com"));

            Assert.IsTrue(ProgramArgumensResolver.dns.Equals("8.8.8.8"));
        }

        [TestMethod]
        public void WithMultipleDnsFlagAnd()
        {
            string[] args = { "gmail.com", "-dns", "8.8.8.8", "-dns", "1.1.1.1" };

            ProgramArgumensResolver.ResolveArgs(args);

            Assert.IsFalse(ProgramArgumensResolver.argsValid);
        }

        [TestMethod]
        public void WithInputFlagAndMultipleDomain()
        {
            string[] args = { "gmail.com", "hotmail.com", "aol.com", "yahoo.com", "-input", "TestResources\\testFile.txt" };

            ProgramArgumensResolver.ResolveArgs(args);

            Assert.IsTrue(ProgramArgumensResolver.argsValid);
            Assert.IsTrue(ProgramArgumensResolver.hosts.Count == 4);
            Assert.IsTrue(ProgramArgumensResolver.hosts[0].Equals("test1"));
            Assert.IsTrue(ProgramArgumensResolver.hosts[1].Equals("test2"));
            Assert.IsTrue(ProgramArgumensResolver.hosts[2].Equals("test3"));
            Assert.IsTrue(ProgramArgumensResolver.hosts[3].Equals("test4"));

            Assert.IsTrue(ProgramArgumensResolver.inputFile.Equals("TestResources\\testFile.txt"));
        }

        [TestMethod]
        public void WithMultipleInputFlagAnd()
        {
            string[] args = { "gmail.com", "-input", "TestResources\\testFile.txt", "-input", "TestResources\\testFile2.txt" };

            ProgramArgumensResolver.ResolveArgs(args);

            Assert.IsFalse(ProgramArgumensResolver.argsValid);
        }

        [TestMethod]
        public void WithOutputFlagAndMultipleDomain()
        {
            string[] args = { "gmail.com", "hotmail.com", "aol.com", "yahoo.com", "-output", "TestResources\\output.txt" };

            ProgramArgumensResolver.ResolveArgs(args);

            Assert.IsTrue(ProgramArgumensResolver.argsValid);
            Assert.IsTrue(ProgramArgumensResolver.hosts.Count == 4);
            Assert.IsTrue(ProgramArgumensResolver.hosts[0].Equals("gmail.com"));
            Assert.IsTrue(ProgramArgumensResolver.hosts[1].Equals("hotmail.com"));
            Assert.IsTrue(ProgramArgumensResolver.hosts[2].Equals("aol.com"));
            Assert.IsTrue(ProgramArgumensResolver.hosts[3].Equals("yahoo.com"));

            Assert.IsTrue(ProgramArgumensResolver.outputFile.Equals("TestResources\\output.txt"));
        }

        [TestMethod]
        public void WithMultipleOutputFlagAnd()
        {
            string[] args = { "gmail.com", "-output", "TestResources\\output.txt", "-output", "TestResources\\output2.txt" };

            ProgramArgumensResolver.ResolveArgs(args);

            Assert.IsFalse(ProgramArgumensResolver.argsValid);
        }
    }
}