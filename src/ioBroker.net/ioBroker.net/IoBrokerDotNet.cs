using System;
using System.Collections.Generic;
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

        public IoBrokerDotNet(string url)
        {
            var connectionUri = new Uri(url);
            _socketIoClient = new SocketIO(connectionUri);
            _subscriptions = new Dictionary<string, List<Action<State>>>();
        }

        public async Task ConnectAsync(TimeSpan timeout)
        {
            _connectedWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
            _socketIoClient.OnConnected += _socketIoClient_OnConnected;
            await _socketIoClient.ConnectAsync();

            _connectedWaitHandle.WaitOne(timeout);
        }

        public async Task SetStateAsync<T>(string id, T value)
        {
            await _socketIoClient.EmitAsync("setState", id, new { val = value, ack = false });
        }

        public async Task<T> GetStateAsync<T>(string id, TimeSpan timeout)
        {
            T retVal = default(T);
            var stateReceivedWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
            await _socketIoClient.EmitAsync("getState", (response) => GetStateResponse<T>(response, stateReceivedWaitHandle, out retVal), id);

            stateReceivedWaitHandle.WaitOne(timeout);
            stateReceivedWaitHandle.Dispose();

            return retVal;
        }

        public async Task SubscribeStateAsync<T>(string id, Action<T> callback)
        {
            var cb = new Action<State>((state) => callback(state.Val.ConvertTo<T>()));

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

        private void _socketIoClient_OnConnected(object sender, EventArgs e)
        {
            _socketIoClient.On("stateChange", HandleStateChanged);
            _connectedWaitHandle.Set();
        }

        private void GetStateResponse<T>(SocketIOResponse response, EventWaitHandle stateReceivedWaitHandle, out T value)
        {
            var topic = response.GetValue<string>();
            var obj = response.GetValue<State>(1);
            value = obj.Val.ConvertTo<T>();
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
