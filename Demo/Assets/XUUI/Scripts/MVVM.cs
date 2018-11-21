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

        static Action<LuaTable, string, object, string> eventSetter = null;

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

                    eventSetter = luaEnv.LoadString<Func<Action<LuaTable, string, object, string>>>(@"
                        return function(options, eventName, obj, methodName)
                            options = options or {}
                            options.methods = options.methods or {}
                            local func = obj[methodName]
                            options.methods[eventName] = function(data)
                                func(obj, data)
                            end
                        end
                    ", "@eventSetter.lua")();
                }
                else
                {
                    creator = null;
                    luaEnv = null;
                }
            }
        }

        Action detach;

        LuaTable options;

        GameObject root;

        bool attached = false;

        public static Func<LuaTable> Compile(string script)
        {
            if (luaEnv == null)
            {
                throw new InvalidOperationException("Please set LuaEnv first!");
            }

            return luaEnv.LoadString<Func<LuaTable>>(script);
        }

        public MVVM(GameObject root, string script, bool doAttach = true) : this(root, Compile(script), doAttach)
        {
        }

        public MVVM(GameObject root, Func<LuaTable> compiled, bool doAttach = true)
        {
            if (luaEnv == null)
            {
                throw new InvalidOperationException("Please set LuaEnv first!");
            }

            this.root = root;
            options = compiled();
            if (doAttach)
            {
                Attach();
            }
        }

        public void Attach()
        {
            if (attached)
            {
                throw new Exception("attached!");
            }

            detach = creator(root, options);
            attached = true;

            root = null;
            options = null;
        }

        public void AddEventHandler(string eventName, object obj, string methodName)
        {
            if (attached)
            {
                throw new Exception("attached!");
            }

            eventSetter(options, eventName, obj, methodName);
        }

        public void Dispose()
        {
            attached = false;
            detach();
            root = null;
            options = null;
            detach = null;
            eventSetter = null;
        }
    }
}
