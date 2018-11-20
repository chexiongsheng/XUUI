using System;

namespace XUUI
{
    public interface DataProducer<T>
    {
        Action<T> OnValueChange { get; set; }
    }
}
