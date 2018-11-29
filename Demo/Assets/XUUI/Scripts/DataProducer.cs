using System;

namespace XUUI
{
    public interface DataProducer
    {
    }

    public interface DataProducer<T> : DataProducer
    {
        Action<T> OnValueChange { get; set; }
    }
}
