using System.Collections.Generic;
using System;
using XLua;

namespace XUUI.UGUIAdapter
{
    public static class XUUIGenConfig
    {
        [CSharpCallLua]
        public static List<Type> CSharpCallLua = new List<Type>()
        {
            typeof(Action),
            typeof(Func<LuaTable>),
            typeof(Func<UnityEngine.GameObject, LuaTable, Action>),
            typeof(Func<Func<UnityEngine.GameObject, LuaTable, Action>>),
            typeof(Action<LuaTable, string, object, string>),
            typeof(Func<Action<LuaTable, string, object, string>>),
        };
    }
}
