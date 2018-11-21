using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace XUUI.UGUIAdapter
{
    public class DropdownAdapter : MonoBehaviour, DataConsumer<int>, DataProducer<int>
    {
        private Dropdown target;

        public string BindTo;

        public Action<int> OnValueChange { get; set; }

        public int Value
        {
            set
            {
                target.value = value;
            }
        }

        void Awake()
        {
            target = gameObject.GetComponent<Dropdown>();
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
