using UnityEngine;
using UnityEngine.UI;
using System;

namespace XUUI.UGUIAdapter
{
    public class InputFieldAdapter : MonoBehaviour, DataConsumer<string>, DataProducer<string>
    {
        private InputField target;

        public string BindTo;

        public Action<string> OnValueChange { get; set; } // InputField发生变化需要调用OnValueChange

        public string Value // VM发生变化，会调用到该Setter，需要同步给InputField
        {
            set
            {
                target.text = value;
            }
        }

        void Awake()
        {
            target = gameObject.GetComponent<InputField>();
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
