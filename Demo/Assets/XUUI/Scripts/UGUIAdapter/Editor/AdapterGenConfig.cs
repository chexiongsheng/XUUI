using System.Collections.Generic;
using System;
using XLua;

namespace XUUI.UGUIAdapter
{

    public static class AdapterGenConfig
    {
        [LuaCallCSharp]
        public static List<Type> LuaCallCSharp = new List<Type>()
        {
            typeof(TextAdapter),
            typeof(InputFieldAdapter),
            typeof(DropdownAdapter),
            typeof(ButtonAdapter),
        };

        [CSharpCallLua]
        public static List<Type> CSharpCallLua = new List<Type>()
        {
            typeof(Action),
            typeof(Action<string>),
            typeof(Action<int>),
        };
    }
}