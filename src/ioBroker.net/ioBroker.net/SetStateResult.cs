using System;

namespace ioBroker.net
{
    public class SetStateResult<T>
    {
        public bool Success { get; set; }

        public Exception Error { get; set; }

        public T ValueToWrite { get; set; }
    }
}
