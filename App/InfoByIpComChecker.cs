using System.Net;
using System.Text.RegularExpressions;
using AngleSharp;

namespace App;

public class InfoByIpComChecker : IIpChecker
{
    private const string RouterNetworkCom = @"https://ru.infobyip.com/";
    private const string DomainClassName = "center results wide home";
    private const string GeographyClassName = "results wide home";
    private const string BadConnection = "Кажется что-то не так с вашим интернет соеденением";

    private const string UserAgent =
        @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.4844.84 Safari/537.36";

    private static readonly Regex IspAsnReg = new("ISP (?<isp>.+)\nASN (?<asn>\\d+)");
    private static readonly Regex CountryReg = new("(?<=Страна) (.+) \\(");


    public async Task<IpData> CheckIp(IPAddress ip)
    {
        var uri = $"{RouterNetworkCom}ip-{ip}.html";
        var page = await GetAsync(uri);
        var config = Configuration.Default;
        var context = BrowsingContext.New(config);
        using var doc = await context.OpenAsync(r => r.Content(page));
        var domain = doc.GetElementsByClassName(DomainClassName).FirstOrDefault()?.TextContent;
        var geography = doc.GetElementsByClassName(GeographyClassName).Skip(1).FirstOrDefault()?.TextContent;
        
        if (domain is null || geography is null) return IpData.Empty;
        
        var asnAndIsp = GetAsnAndIsp(domain);
        var country = GetCountry(geography);

        return new IpData(ip, asnAndIsp.asn, country, asnAndIsp.isp);
    }

    private string GetCountry(string geography)
    {
        return CountryReg.Match(geography).Groups[1].Value;
    }

    private (string asn, string isp) GetAsnAndIsp(string domain)
    {
        var match = IspAsnReg.Match(domain);
        var asn = match.Groups["asn"].Value;
        var isp = match.Groups["isp"].Value;
        return (asn, isp);
    }

    private static async Task<string> GetAsync(string uri)
    {
        try
        {
            var req = (HttpWebRequest)WebRequest.Create(uri);
            req.UserAgent = UserAgent;
            using var res = req.GetResponse();
            using var streamReader = new StreamReader(res.GetResponseStream());
            var result = streamReader.ReadToEndAsync();
            return await result;
        }
        catch(AggregateException e)
        {
            throw new HttpRequestException($"{BadConnection}\n\n {e.StackTrace}");
        }
    }
}