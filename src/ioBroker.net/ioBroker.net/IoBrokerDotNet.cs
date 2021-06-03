using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ioBroker.net.Extensions;
using ioBroker.net.Model;
using SocketIOClient;

namespace ioBroker.net
{
    public class IoBrokerDotNet : IIoBrokerDotNet
    {
        private readonly SocketIO _socketIoClient;
        private EventWaitHandle _connectedWaitHandle;
        private readonly Dictionary<string, List<Action<State>>> _subscriptions;

        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);


        public IoBrokerDotNet()
        {
            _socketIoClient = new SocketIO();
            _socketIoClient.OnConnected += async (sender, eventArgs) => await SocketIoOnConnectedHandler(sender, eventArgs);
            _socketIoClient.OnDisconnected += (sender, s) => { Console.WriteLine($"Disonnected from socket.io: {s}"); };
            _socketIoClient.OnError += (sender, s) => { Console.WriteLine($"Error from socket.io: {s}"); };
            _socketIoClient.OnReconnecting += (sender, i) => { Console.WriteLine($"Reconnecting: {i}"); };
            _socketIoClient.OnReconnectFailed += (sender, exception) => { Console.WriteLine($"Reconnect failed: {exception}"); };

            _subscriptions = new Dictionary<string, List<Action<State>>>();
        }

        public IoBrokerDotNet(string connectionString) : base()
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; set; }

        public async Task ConnectAsync(TimeSpan timeout)
        {
            var connectionUri = new Uri(ConnectionString);
            _socketIoClient.ServerUri = connectionUri;

            _connectedWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
            await _socketIoClient.ConnectAsync();

            _connectedWaitHandle.WaitOne(timeout);
        }

        public async Task SetStateAsync<T>(string id, T value)
        {
            await _socketIoClient.EmitAsync("setState", id, new { val = value, ack = false });
        }

        public async Task<GetStateResult<T>> GetStateAsync<T>(string id, TimeSpan timeout)
        {
            var retVal = new GetStateResult<T>();
            var stateReceivedWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
            await _socketIoClient.EmitAsync("getState", (response) => GetStateResponse<T>(response, stateReceivedWaitHandle, retVal, id), id);

            if (!stateReceivedWaitHandle.WaitOne(timeout))
            {
                retVal.Success = false;
                retVal.Error = new TimeoutException($"Timeout for reading state of id: \"{id}\"");
            }
            stateReceivedWaitHandle.Dispose();

            return retVal;
        }

        public async Task SubscribeStateAsync<T>(string id, Action<T> callback)
        {
            var cb = new Action<State>((state) => callback(JsonSerializer.Deserialize<T>(state.Val.ToString())));

            await semaphoreSlim.WaitAsync();
            try
            {
                if (_subscriptions.TryGetValue(id, out var callbacks))
                {
                    callbacks.Add(cb);
                }
                else
                {
                    callbacks = new List<Action<State>>();
                    callbacks.Add(cb);
                    _subscriptions.Add(id, callbacks);
                    await _socketIoClient.EmitAsync("subscribe", id);
                }
            }
            finally
            {
                semaphoreSlim.Release();
            }
            
        }

        private async Task SocketIoOnConnectedHandler(object sender, EventArgs e)
        {
            Console.WriteLine($"Connected succesully to socket.io");
            Console.WriteLine($"Register for state changes");
            _socketIoClient.On("stateChange", HandleStateChanged);
            await SubscribeAllIds();
            _connectedWaitHandle.Set();
        }

        private async Task SubscribeAllIds()
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                foreach (var subscription in _subscriptions)
                {
                    Console.WriteLine($"Subscribe to {subscription.Key}");
                    await _socketIoClient.EmitAsync("subscribe", subscription.Key);
                }
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        private void GetStateResponse<T>(SocketIOResponse response, EventWaitHandle stateReceivedWaitHandle, GetStateResult<T> stateResult, string id)
        {
            var topic = response.GetValue<string>();
            var obj = response.GetValue<State>(1);
            if (obj != null)
            {
                try
                {
                    stateResult.Value= JsonSerializer.Deserialize<T>(obj.Val.ToString());
                    stateResult.Success = true;
                }
                catch (Exception e)
                {
                    stateResult.Success = false;
                    stateResult.Error = e;
                }
            }
            else
            {
                stateResult.Success = false;
                stateResult.Error = new Exception($"Id: \"{id}\" not found");
            }

            stateReceivedWaitHandle.Set();
        }

        private void HandleStateChanged(SocketIOResponse response)
        {
            var topic = response.GetValue<string>();
            var obj = response.GetValue<State>(1);

            if (_subscriptions.TryGetValue(topic, out List<Action<State>> callbacks))
            {
                Parallel.ForEach(callbacks, (callbackMethod) => callbackMethod(obj));
            }
        }
    }
}
