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
            //ioBroker.ConnectionString = @"http://192.168.178.192:8084";
            ioBroker.ConnectionString = @"http://iobroker:8084";
            await ioBroker.ConnectAsync(TimeSpan.FromSeconds(5));
            await ioBroker.SubscribeStateAsync<bool>("linkeddevices.0.motionsensors.gangoben.presence", MotionDetectionChanged);
            await ioBroker.SubscribeStateAsync<int>("linkeddevices.0.lights.GangOben.level", LightChanged);
            await ioBroker.SubscribeStateAsync<string>("javascript.0.managers.lights.EgGang.Mode", StringChanged);

            var boolValue = await ioBroker.TryGetStateAsync<bool>("linkeddevices.0.motionsensors.gangoben.presence");
            var intValue = await ioBroker.TryGetStateAsync<int>("linkeddevices.0.lights.GangOben.level");
            var stringValue = await ioBroker.TryGetStateAsync<string>("javascript.0.managers.lights.EgGang.Mode");


            //var tempCountId = "javascript.0.socketio.0.Test_12345";
            //var tempCount = ioBroker.TryGetStateAsync<bool>(tempCountId, TimeSpan.FromSeconds(5)).Result;
            //var newValue = false;
            //await ioBroker.SetStateAsync<bool>(tempCountId, newValue);

            //await ioBroker.SubscribeStateAsync<int>("linkeddevices.0.lights.GangOben.level", (value) => Console.WriteLine($"Received value: {value}"));
            //await ioBroker.SetStateAsync<int>("linkeddevices.0.lights.GangOben.level", 35);

            //var testread = await ioBroker.GetStateAsync<int>("linkeddevices.0.lights.fernsehlicht.level", TimeSpan.FromSeconds(5));
            //if (testread.Success)
            //{
            //    Console.WriteLine($"Success with value: {testread.Value}");
            //}
            //else
            //{
            //    Console.WriteLine($"Error with Exception: {testread.Error}");
            //}
            ////await ioBroker.CreateStateAsync<int>("javascript.0.toilett.flushes");

            //await TryWriteAndRead(ioBroker, tempCountId);
            //await TryWriteAndRead(ioBroker, tempCountId);

            Console.ReadKey();
        }

        private static void StringChanged(string stringChanged)
        {
            Console.WriteLine($"StringChanged: {stringChanged}");
        }

        private static void MotionDetectionChanged(bool motionDetected)
        {
            Console.WriteLine($"MotionDetectionChanged: {motionDetected}");
        }

        private static void LightChanged(int lightChanged)
        {
            Console.WriteLine($"LightChanged: {lightChanged}");
        }

        private static async Task TryWriteAndRead(IoBrokerDotNet ioBroker, string tempCountId)
        {
            Thread.Sleep(TimeSpan.FromSeconds(30));
            var newValue = false;
            var result = await ioBroker.TrySetStateAsync<bool>(tempCountId, newValue);
            if (result.Success)
            {
                Console.WriteLine($"result: {result.Success}");
            }
            else
            {
                Console.WriteLine($"Error while writing: {result.Error}");
            }

            var resultWithoutConnection = ioBroker.TryGetStateAsync<bool>(tempCountId, TimeSpan.FromSeconds(5)).Result;
            if (resultWithoutConnection.Success)
            {
                Console.WriteLine($"result: {resultWithoutConnection.Value}");
            }
            else
            {
                Console.WriteLine($"Error while Reading: {resultWithoutConnection.Error}");
            }
        }

    }
}
