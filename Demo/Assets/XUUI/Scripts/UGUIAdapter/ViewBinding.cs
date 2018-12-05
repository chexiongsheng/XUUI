using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace XUUI.UGUIAdapter
{
    public enum ComponentType
    {
        Text,
        InputField,
        Button,
        Dropdown,
    }

    [Serializable]
    public struct Binding
    {
        [SerializeField]
        public ComponentType Type;

        [SerializeField]
        public Component Component;

        [SerializeField]
        public string BindTo;

        [SerializeField]
        public bool MultiFields;
    }

    public class ViewBinding : MonoBehaviour
    {
        [SerializeField]
        public List<Binding> Bindings;

        [NonSerialized]
        private object[][] cacheAdapters = null;

        private RawAdapterBase createAdapter(Component component, string bindTo)
        {
            Text text = component as Text;
            if (text != null)
            {
                return new RawTextAdapter(text, bindTo);
            }

            InputField inputField = component as InputField;
            if (inputField != null)
            {
                return new RawInputFieldAdapter(inputField, bindTo);
            }

            Dropdown dropdown = component as Dropdown;
            if (dropdown != null)
            {
                return new RawDropdownAdapter(dropdown, bindTo);
            }

            Button button = component as Button;
            if (button != null)
            {
                return new RawButtonAdapter(button, bindTo);
            }

            return null;
        }

        public object[][] GetAdapters()
        {
            if (cacheAdapters == null)
            {
                if (Bindings == null || Bindings.Count == 0)
                {
                    cacheAdapters = new object[][] { };
                }
                else
                {
                    var dataConsumers = new List<object>();
                    var dataProducers = new List<object>();
                    var eventEmitters = new List<object>();

                    for (int i = 0; i < Bindings.Count; i++)
                    {
                        var binding = Bindings[i];
                        RawAdapterBase adapter = createAdapter(binding.Component, binding.BindTo);
                        if (adapter == null)
                        {
                            throw new InvalidOperationException("no adatper for " + binding.Component);
                        }
                        if (adapter is DataConsumer)
                        {
                            dataConsumers.Add(adapter);
                        }
                        if (adapter is DataProducer)
                        {
                            dataProducers.Add(adapter);
                        }
                        if (adapter is EventEmitter)
                        {
                            eventEmitters.Add(adapter);
                        }
                    }

                    cacheAdapters = new object[][] { dataConsumers.ToArray(), dataProducers.ToArray(), eventEmitters.ToArray() };
                }
            }
            return cacheAdapters;
        }

    }
}
