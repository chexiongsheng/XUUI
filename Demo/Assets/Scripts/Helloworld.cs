using UnityEngine;
using XLua;
using System;

public class Helloworld : MonoBehaviour
{
    LuaEnv luaenv = new LuaEnv();

    Action detach = null;

    void Start()
    {
        detach = luaenv.LoadString<Func<object, Action>>(@"
            local xuui = require 'xuui'
            local select_info = {'vegetables', 'meat'}

            local mvvm = xuui.new {
               el = select(1, ...),
               data = {
	               name = 'john',
	               select = 0,
               },
               computed = {
	               message = function(data)
		               return 'Hello ' .. data.name .. ', your choice is ' .. tostring(select_info[data.select + 1])
	               end
               },
               methods = {
	               reset = function(data)
		               data.name = 'john'
		               data.select = 0
	               end,
               },
            }
            return mvvm.detach
        ", "@main.lua")(gameObject);
        
    }

    void OnDestroy()
    {
        detach();
        detach = null;
        luaenv.Dispose();
    }
}
