using System;

namespace XUUI
{
    public interface EventEmitter
    {
        Action OnAction { get; set; }
    }
}
