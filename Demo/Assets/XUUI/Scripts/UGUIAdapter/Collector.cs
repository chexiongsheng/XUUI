using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace XUUI.UGUIAdapter
{
    public class Collector
    {
        static object[][] Collect(BindToSetting setting)
        {
            var dataProducers = new List<object>();

            if (setting.InputFields != null)
            {
                dataProducers.AddRange(setting.InputFields.Select(item => (object)new RawInputFieldAdapter(item.Target, item.BindTo)));
            }

            if (setting.Dropdowns != null)
            {
                dataProducers.AddRange(setting.Dropdowns.Select(item => (object)new RawDropdownAdapter(item.Target, item.BindTo)));
            }

            var dataConsumers = new List<object>();

            if (setting.Texts != null)
            {
                dataConsumers.AddRange(setting.Texts.Select(item => (object)new RawTextAdapter(item.Target, item.BindTo)));
            }

            dataConsumers.AddRange(dataProducers);

            var eventEmitters = new List<object>();

            if (setting.Buttons != null)
            {
                eventEmitters.AddRange(setting.Buttons.Select(item => (object)new RawButtonAdapter(item.Target, item.BindTo)));
            }

            return new object[][] { dataConsumers.ToArray(), dataProducers.ToArray(), eventEmitters.ToArray() };
        }


        // [0]: DataConsumers
        // [1]: DataProducers
        // [2]: EventEmitters
        public static object[][] Collect(GameObject go)
        {
            var setting = go.GetComponent<BindToSetting>();
            if (setting != null)
            {
                return Collect(setting);
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
