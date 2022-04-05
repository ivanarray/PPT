using System.Net;

namespace App;

// ReSharper disable InconsistentNaming
public record IpData(IPAddress Ip, string AS, string Country, string Provider)
{
    public static IpData Empty = new IpData(IPAddress.None, string.Empty, string.Empty, string.Empty);
}