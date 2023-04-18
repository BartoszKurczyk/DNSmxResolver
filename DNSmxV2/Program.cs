// See https://aka.ms/new-console-template for more information
using DNSmxResolver;
using DNSmxV2;
using System.Text;

ProgramArgumensResolver.ResolveArgs(args);

if (!ProgramArgumensResolver.argsValid)
{
    Console.WriteLine(ProgramArgumensResolver.errorMessage);
    return;
}

if (ProgramArgumensResolver.hosts.Count == 1)
{
    DNSResolver resolver;
    if (ProgramArgumensResolver.dns != string.Empty)
        resolver = new DNSResolver(ProgramArgumensResolver.hosts.First(), ProgramArgumensResolver.dns);
    else
        resolver = new DNSResolver(ProgramArgumensResolver.hosts.First());

    resolver.Resolve();

    Console.WriteLine(resolver.Raport);
    if (ProgramArgumensResolver.outputFile != string.Empty)
    {
        File.WriteAllText(ProgramArgumensResolver.outputFile, resolver.Raport);
    }
}
else if (ProgramArgumensResolver.hosts.Count > 0)
{
    List<DNSResolver> resolvers = new List<DNSResolver>();
    List<Thread> threads = new List<Thread>();

    foreach (var host in ProgramArgumensResolver.hosts)
    {
        DNSResolver resolver;
        if (ProgramArgumensResolver.dns != string.Empty)
            resolver = new DNSResolver(host, ProgramArgumensResolver.dns);
        else
            resolver = new DNSResolver(host);

        resolvers.Add(resolver);

        var th = new Thread(resolvers.Last().Resolve);
        threads.Add(th);
    }

    threads.ForEach(x => x.Start());
    threads.ForEach(x => x.Join());

    StringBuilder stringBuilder = new StringBuilder();

    foreach (var resolver in resolvers)
    {
        stringBuilder.AppendLine(resolver.Raport);
        stringBuilder.AppendLine();
    }

    Console.WriteLine(stringBuilder.ToString());
    if(ProgramArgumensResolver.outputFile != string.Empty)
    {
        File.WriteAllText(ProgramArgumensResolver.outputFile, stringBuilder.ToString());
    }

}



