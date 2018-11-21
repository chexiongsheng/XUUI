using XLua;
using System;
using System.Collections.Generic;

public static class DemoGenConfig
{
    [LuaCallCSharp]
    public static List<Type> LuaCallCSharp = new List<Type>()
    {
        typeof(SomeClass),
    };

    [CSharpCallLua]
    public static List<Type> CSharpCallLua = new List<Type>()
    {
        typeof(Interface1),
        typeof(Interface2),
    };
}
