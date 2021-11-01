using System;
using System.Threading.Tasks;

namespace ioBroker.net
{
    public interface IIoBrokerDotNet
    {
        string ConnectionString { get; set; }
        Task ConnectAsync(TimeSpan timeout);
        Task<SetStateResult<T>> TrySetStateAsync<T>(string id, T value);
        Task<GetStateResult<T>> TryGetStateAsync<T>(string id, TimeSpan timeout);
        Task SubscribeStateAsync<T>(string id, Action<T> callback);
    }
}