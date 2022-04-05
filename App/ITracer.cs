namespace App;

public interface ITracer
{
    Task<IEnumerable<IpData>> TraceRoute(string host);
}