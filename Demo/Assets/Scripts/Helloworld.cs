using UnityEngine;
using XLua;
using XUUI;

public class Helloworld : MonoBehaviour
{
    LuaEnv luaenv = new LuaEnv();

    MVVM mvvm = null;

    void Start()
    {
        MVVM.Env = luaenv;

        mvvm = new MVVM(gameObject, @"
            local select_info = {'vegetables', 'meat'}

            return {
               data = {
	               info = {
                       name = 'john',
                   },
	               select = 0,
               },
               computed = {
	               message = function(data)
		               return 'Hello ' .. data.info.name .. ', your choice is ' .. tostring(select_info[data.select + 1])
	               end
               },
               methods = {
	               reset = function(data)
		               data.info.name = 'john'
		               data.select = 0
	               end,
               },
            }
        ");
        
    }

    void OnDestroy()
    {
        mvvm.Dispose();
        MVVM.Env = null;
        luaenv.Dispose();
    }
}
