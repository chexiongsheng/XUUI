using UnityEngine.UI;
using System;

namespace XUUI.UGUIAdapter
{
    public class RawDropdownAdapter : RawAdapterBase, DataConsumer<int>, DataProducer<int>
    {
        public Dropdown target;

        public Action<int> OnValueChange { get; set; }

        public int Value
        {
            set
            {
                target.value = value;
            }
        }

        public RawDropdownAdapter(Dropdown dropdown, string bindTo)
        {
            target = dropdown;
            BindTo = bindTo;

            target.onValueChanged.AddListener((val) =>
            {
                if (OnValueChange != null)
                {
                    OnValueChange(val);
                }
            });
        }
    }
}
