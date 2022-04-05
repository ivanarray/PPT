using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;

namespace App;

public static class IpHelper
{
    public static IEnumerable<IPAddress> GetTraceRoute(string hostname)
    {
        // following are similar to the defaults in the "traceroute" unix command.
        const int timeout = 2000;
        // ReSharper disable once InconsistentNaming
        const int maxTTL = 30;
        const int bufferSize = 32;
        var buffer = new byte[bufferSize];
        new Random().NextBytes(buffer);
        var watch = new Stopwatch();

        using var pinger = new Ping();
        watch.Start();
        for (var ttl = 1; ttl <= maxTTL; ttl++)
        {
            var options = new PingOptions(ttl, true);
            var reply = pinger.Send(hostname, timeout, buffer, options);

            switch (reply.Status)
            {
                case IPStatus.Success:
                    Console.WriteLine(
                        $"TTL = {ttl} последний ответ от {reply.Address}  прошло {watch.Elapsed.TotalSeconds} сек");
                    yield return reply.Address;
                    yield break;
                // we've found a route at this ttl
                case IPStatus.TtlExpired:
                    Console.WriteLine($"TTL = {ttl} ответ от {reply.Address}  прошло {watch.Elapsed.TotalSeconds} сек");
                    yield return reply.Address;
                    break;
                // if we reach a status other than expired or timed out, we're done searching or there has been an error
                case IPStatus.TimedOut:
                    Console.WriteLine($"TTL = {ttl} нет  ответа {reply.Status} прошло {watch.Elapsed.TotalSeconds} сек");
                    break;
                default:
                    throw new PingException($"Ошибка в запросе к {hostname} ping статус {reply.Status}");
            }
        }
    }
}