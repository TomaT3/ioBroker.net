using System;
using System.Threading.Tasks;

namespace ioBroker.net
{
    public interface IIoBrokerDotNet
    {
        Task ConnectAsync(TimeSpan timeout);
        Task SetStateAsync<T>(string id, T value);
        Task<T> GetStateAsync<T>(string id, TimeSpan timeout);
        Task SubscribeStateAsync<T>(string id, Action<T> callback);
    }
}