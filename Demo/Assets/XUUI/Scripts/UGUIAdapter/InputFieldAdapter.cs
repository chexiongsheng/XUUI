using UnityEngine;
using UnityEngine.UI;
using System;

namespace XUUI.UGUIAdapter
{
    public class InputFieldAdapter : MonoBehaviour, DataConsumer<string>, DataProducer<string>
    {
        public InputField Target;

        public string BindTo;

        public Action<string> OnValueChange { get; set; } // InputField发生变化需要调用OnValueChange

        public string Value // VM发生变化，会调用到该Setter，需要同步给InputField
        {
            set
            {
                Target.text = value;
            }
        }

        void Start()
        {
            Target.onValueChange.AddListener((val) =>
            {
                if (OnValueChange != null)
                {
                    OnValueChange(val);
                }
            });
        }
    }
}
