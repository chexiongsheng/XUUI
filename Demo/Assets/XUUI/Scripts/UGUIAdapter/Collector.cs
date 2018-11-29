using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace XUUI.UGUIAdapter
{
    public class Collector
    {
        static object[][] collect(BindToSetting setting)
        {
            var dataProducers = new List<object>();
            var dataConsumers = new List<object>();
            var eventEmitters = new List<object>();

            if (setting.InputFields != null)
            {
                var adpaters = setting.InputFields.Select(item => (object)new RawInputFieldAdapter(item.Target, item.BindTo));
                dataConsumers.AddRange(adpaters);
                dataProducers.AddRange(adpaters);

            }

            if (setting.Dropdowns != null)
            {
                var adpaters = setting.Dropdowns.Select(item => (object)new RawDropdownAdapter(item.Target, item.BindTo));
                dataConsumers.AddRange(adpaters);
                dataProducers.AddRange(adpaters);
            }

            if (setting.Texts != null)
            {
                dataConsumers.AddRange(setting.Texts.Select(item => (object)new RawTextAdapter(item.Target, item.BindTo)));
            }

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
                return collect(setting);
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
