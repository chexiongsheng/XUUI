using UnityEngine;
using UnityEngine.UI;
using System;
using XLua;

namespace XUUI.UGUIAdapter
{
    public class DropdownOptionsAdapter : AdapterBase<Dropdown>, DataConsumer<LuaTable>
    {
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

    }
}

