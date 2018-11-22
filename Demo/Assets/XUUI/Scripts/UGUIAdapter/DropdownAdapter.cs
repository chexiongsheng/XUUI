using UnityEngine;
using UnityEngine.UI;
using System;

namespace XUUI.UGUIAdapter
{
    [AddComponentMenu("XUUI/Dropdown", 1)]
    [RequireComponent(typeof(Dropdown))]
    public class DropdownAdapter : AdapterBase<Dropdown>, DataConsumer<int>, DataProducer<int>
    {
        public Action<int> OnValueChange { get; set; }

        public int Value
        {
            set
            {
                Target.value = value;
            }
        }

        void Start()
        {
            Target.onValueChanged.AddListener((val) =>
            {
                if (OnValueChange != null)
                {
                    OnValueChange(val);
                }
            });
        }
    }
}
