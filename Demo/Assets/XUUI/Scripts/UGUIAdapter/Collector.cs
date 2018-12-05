using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace XUUI.UGUIAdapter
{
    public class Collector
    {
        // [0]: DataConsumers
        // [1]: DataProducers
        // [2]: EventEmitters
        public static object[][] Collect(GameObject go)
        {
            var viewBinding = go.GetComponent<ViewBinding>();
            if (viewBinding != null)
            {
                return viewBinding.GetAdapters();
            }

            var adapters = go.GetComponentsInChildren<AdapterBase>(true);

            var dataProducers = adapters
                .Where(adapter => adapter is DataProducer)
                .Select(o => (object)o)
                .ToArray();

            var dataConsumers = adapters
                .Where(adapter => adapter is DataConsumer)
                .Select(o => (object)o)
                .ToArray();

            var eventEmitters = adapters
                .Where(adapter => adapter is EventEmitter)
                .Select(o => (object)o)
                .ToArray();

            return new object[][] { dataConsumers, dataProducers, eventEmitters };
        }
    }
}
