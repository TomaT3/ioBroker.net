using System;
using System.Threading;
using System.Threading.Tasks;
using ioBroker.net;

namespace ConsoleTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var ioBroker = new IoBrokerDotNet();
            ioBroker.ConnectionString = @"http://ioBroker:8084";

            

            await ioBroker.ConnectAsync(TimeSpan.FromSeconds(5));
            await ioBroker.SubscribeStateAsync<int>("linkeddevices.0.lights.GangOben.level", (value) => Console.WriteLine($"Received value: {value}"));
            await ioBroker.SetStateAsync<int>("linkeddevices.0.lights.GangOben.level", 35);

            var testread = await ioBroker.GetStateAsync<int>("linkeddevices.0.lights.fernsehlicht.level", TimeSpan.FromSeconds(5));
            if (testread.Success)
            {
                Console.WriteLine($"Success with value: {testread.Value}");
            }
            else
            {
                Console.WriteLine($"Error with Exception: {testread.Error}");
            }
            //await ioBroker.CreateStateAsync<int>("javascript.0.toilett.flushes");
            
            Thread.Sleep(5000000);
            Console.ReadKey();
        }

        
    }
}
