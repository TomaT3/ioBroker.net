using System;
using System.Threading.Tasks;
using ioBroker.net;

namespace ConsoleTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var ioBroker = new IoBrokerDotNet(@"http://192.168.178.199:8084");
            await ioBroker.ConnectAsync(TimeSpan.FromSeconds(5));
            var testread = await ioBroker.GetStateAsync<bool>("discovery.0.scanRunning", TimeSpan.FromSeconds(5));

            //await ioBroker.CreateStateAsync<int>("javascript.0.toilett.flushes");
            
            
            Console.ReadKey();
        }
    }
}
