using UnityEngine;
using System.Linq;

namespace XUUI.UGUIAdapter
{
    public class Collector
    {
        // [0]: DataConsumers
        // [1]: DataProducers
        // [2]: EventEmitters
        public static object[][] Collect(GameObject go)
        {
            var dataProducers = go.GetComponentsInChildren<InputFieldAdapter>(true)
                .Select(o => (object)o)
                .Concat(go.GetComponentsInChildren<DropdownAdapter>(true))
                .ToArray();

            var dataConsumers = go.GetComponentsInChildren<TextAdapter>(true)
                .Select(o => (object)o)
                .Concat(dataProducers)
                .ToArray();

            var eventEmitters = go.GetComponentsInChildren<ButtonAdapter>(true).Select(o => (object)o).ToArray();

            return new object[][] { dataConsumers, dataProducers, eventEmitters };
        }
    }
}
