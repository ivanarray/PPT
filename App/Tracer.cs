namespace App;

public class Tracer : ITracer
{
    private readonly IIpChecker ipChecker;

    public Tracer(IIpChecker checker)
    {
        ipChecker = checker;
    }

    public async Task<IEnumerable<IpData>> TraceRoute(string host)
    {
        var addresses = IpHelper.GetTraceRoute(host);
        var tasks = addresses.Select(ip => ipChecker.CheckIp(ip))
            .ToList();

        await Task.WhenAll(tasks);

        return tasks.Select(t => t.Result).Where(d => d != IpData.Empty);
    }
}