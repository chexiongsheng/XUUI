using UnityEngine;
using UnityEngine.UI;
using System;
using XLua;

namespace XUUI.UGUIAdapter
{
    public class DropdownOptionsAdapter : MonoBehaviour, DataConsumer<LuaTable>
    {
        public Dropdown Target;

        public string BindTo;

        public LuaTable Value
        {
            set
            {
                Target.options.Clear();
                value.ForEach<int, string>((k, v) =>
                {
                    Target.options.Add(new Dropdown.OptionData(v));
                });
            }
        }

        void Awake()
        {
            if (Target == null)
            {
                Target = gameObject.GetComponent<Dropdown>();
            }
        }
    }
}

