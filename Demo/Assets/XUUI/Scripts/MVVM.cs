using UnityEngine;
using System.Collections;
using XLua;
using System;

namespace XUUI
{
    public class MVVM : IDisposable
    {
        static LuaEnv luaEnv = null;

        static Func<GameObject, LuaTable, Action> creator = null;

        public static LuaEnv Env
        {
            set
            {
                if (value != null)
                {
                    luaEnv = value;

                    creator = luaEnv.LoadString<Func<Func<GameObject, LuaTable, Action>>>(@"
                        local xuui = require 'xuui'
                        return function(el, options)
                            options = options or {}
                            options.el = el
                            options.data = options.data or {}
                            options.computed = options.computed or {}
                            options.methods = options.methods or {}
                            return xuui.new(options).detach
                        end
                    ", "@xuui_init.lua")();
                }
                else
                {
                    creator = null;
                    luaEnv = null;
                }
            }
        }

        Action detach;

        public MVVM(GameObject root, string script)
        {
            if (luaEnv == null)
            {
                throw new InvalidOperationException("Please set LuaEnv first!");
            }

            LuaTable tbl = luaEnv.LoadString<Func<LuaTable>>(script)();
            detach = creator(root, tbl);
        }

        public void Dispose()
        {
            detach();
            detach = null;
        }
    }
}
