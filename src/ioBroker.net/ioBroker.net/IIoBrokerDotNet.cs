using System;
using System.Threading.Tasks;

namespace ioBroker.net
{
    public interface IIoBrokerDotNet
    {
        string ConnectionString { get; set; }
        Task ConnectAsync(TimeSpan timeout);
        Task SetStateAsync<T>(string id, T value);
        Task<GetStateResult<T>> GetStateAsync<T>(string id, TimeSpan timeout);
        Task SubscribeStateAsync<T>(string id, Action<T> callback);
    }
}