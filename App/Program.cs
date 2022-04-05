using CommandDotNet;
using ConsoleTables;

namespace App;

public class Program
{
    public static void Main(string[] args)
    {
        new AppRunner<Program>().Run(args);
    }

    [DefaultCommand]
    // ReSharper disable once UnusedMember.Global
    public  void Process([Positional("IP адрес или имя хоста", 1)] string host)
    {
        var tracer = new Tracer(new InfoByIpComChecker());
        var route = tracer.TraceRoute(host);
        var table = new ConsoleTable("IP", "AS","Country","Provider");
        var result = route.Result;
        foreach (var data in result)
        {
            table.AddRow(data.Ip,data.AS,data.Country,data.Provider);
        }
        Console.WriteLine(table);
    }
}