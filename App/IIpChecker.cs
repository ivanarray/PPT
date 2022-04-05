using System.Net;

namespace App;

public interface IIpChecker
{
    Task<IpData> CheckIp(IPAddress ip);
}