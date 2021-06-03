using System;

namespace ioBroker.net
{
    public class GetStateResult<T>
    {
        public bool Success { get; set; }

        public Exception Error { get; set; }

        public T Value { get; set; }
    }
}
