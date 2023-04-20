// See https://aka.ms/new-console-template for more information
using DNSmxResolver;
using DNSmxV2;
using System.Text;

ProgramArgumensResolver.Instance.ResolveArgs(args);

if (!ProgramArgumensResolver.Instance.argsValid)
{
    Console.WriteLine(ProgramArgumensResolver.Instance.errorMessage);
    return;
}

if (ProgramArgumensResolver.Instance.hosts.Count == 1)
{
    DNSResolver resolver;
    if (ProgramArgumensResolver.Instance.dns != string.Empty)
        resolver = new DNSResolver(ProgramArgumensResolver.Instance.hosts.First(), ProgramArgumensResolver.Instance.dns);
    else
        resolver = new DNSResolver(ProgramArgumensResolver.Instance.hosts.First());

    resolver.Resolve();

    Console.WriteLine(resolver.Raport);
    if (ProgramArgumensResolver.Instance.outputFile != string.Empty)
    {
        File.WriteAllText(ProgramArgumensResolver.Instance.outputFile, resolver.Raport);
    }
}
else if (ProgramArgumensResolver.Instance.hosts.Count > 0)
{
    List<DNSResolver> resolvers = new List<DNSResolver>();
    List<Thread> threads = new List<Thread>();

    foreach (var host in ProgramArgumensResolver.Instance.hosts)
    {
        DNSResolver resolver;
        if (ProgramArgumensResolver.Instance.dns != string.Empty)
            resolver = new DNSResolver(host, ProgramArgumensResolver.Instance.dns);
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
    if(ProgramArgumensResolver.Instance.outputFile != string.Empty)
    {
        File.WriteAllText(ProgramArgumensResolver.Instance.outputFile, stringBuilder.ToString());
    }

}



