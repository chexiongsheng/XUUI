using UnityEngine;
using System.Collections.Generic;
using XLua;
using System;

namespace XUUI
{
    public class MVVM : IDisposable
    {
        static volatile LuaEnv luaEnv = null;

        static Func<LuaTable, Func<GameObject, Action>> creator = null;

        static Action<LuaTable, string, object, string> eventSetter = null;

        public static LuaEnv Env
        {
            set
            {
                if (value != null)
                {
                    luaEnv = value;

                    creator = luaEnv.LoadString<Func<Func<LuaTable, Func<GameObject, Action>>>>(@"
                        return (require 'xuui').new
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
                    eventSetter = null;
                    creator = null;
                    luaEnv = null;
                }
            }
        }

        Func<GameObject, Action> attach;

        public static Func<LuaTable> Compile(string script)
        {
            if (luaEnv == null)
            {
                throw new InvalidOperationException("Please set LuaEnv first!");
            }

            return luaEnv.LoadString<Func<LuaTable>>(script);
        }

        public MVVM()
        {
            if (luaEnv == null)
            {
                Env = new LuaEnv();
            }
            init(luaEnv.NewTable());
        }

        public MVVM(string script) : this(Compile(script))
        {
        }

        public MVVM(Func<LuaTable> compiled)
        {
            init(compiled());
        }

        LuaTable options;

        void init(LuaTable options)
        {
            if (luaEnv == null)
            {
                Env = new LuaEnv();
            }
            this.options = options;
            attach = creator(options);
        }

        Dictionary<GameObject, Action> detachors = new Dictionary<GameObject, Action>();

        public void Attach(GameObject go)
        {
            if (detachors.ContainsKey(go))
            {
                throw new InvalidOperationException("attached GameObject");
            }
            var detach = attach(go);
            detachors.Add(go, detach);
        }

        public void AddEventHandler(string eventName, object obj, string methodName)
        {
            eventSetter(options, eventName, obj, methodName);
        }

        public void Dispose()
        {
            foreach(var kv in detachors)
            {
                kv.Value();
            }

            detachors = null;
            options = null;
            attach = null;
        }
    }
}
