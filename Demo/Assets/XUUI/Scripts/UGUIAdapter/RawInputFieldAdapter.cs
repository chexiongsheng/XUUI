using UnityEngine;
using UnityEngine.UI;
using System;

namespace XUUI.UGUIAdapter
{
    public class RawInputFieldAdapter : RawAdapterBase, DataConsumer<string>, DataProducer<string>
    {
        private InputField target;

        public Action<string> OnValueChange { get; set; } // InputField发生变化需要调用OnValueChange

        public string Value // VM发生变化，会调用到该Setter，需要同步给InputField
        {
            set
            {
                target.text = value == null ? "" : value;
            }
        }

        public RawInputFieldAdapter(InputField input, string bindTo)
        {
            target = input;
            BindTo = bindTo;

            target.onValueChange.AddListener((val) =>
            {
                if (OnValueChange != null)
                {
                    OnValueChange(val);
                }
            });
        }
    }
}
