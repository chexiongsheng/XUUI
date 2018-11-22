using UnityEngine;
using System.Collections.Generic;
using XLua;
using System;

namespace XUUI
{
    public class ViewModel : IDisposable
    {
        LuaEnv luaEnv = null;

        Func<LuaTable, Func<GameObject, Action>> creator = null;

        Action<LuaTable, string, object, string> eventSetter = null;

        bool disposeLuaEnv = false;

        void initLua(LuaEnv env)
        {
            if (env == null)
            {
                luaEnv = new LuaEnv();
                disposeLuaEnv = true;
            }
            else
            {
                luaEnv = env;
            }

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


        Func<GameObject, Action> attach;

        public Func<LuaTable> Compile(string script)
        {
            return luaEnv.LoadString<Func<LuaTable>>(script);
        }

        public ViewModel(LuaEnv env = null)
        {
            initLua(env);
            init(luaEnv.NewTable());
        }

        public ViewModel(string script, LuaEnv env = null)
        {
            initLua(env);
            init(Compile(script)());
        }

        public ViewModel(Func<LuaTable> compiled, LuaEnv env = null)
        {
            initLua(env);
            init(compiled());
        }

        LuaTable options;

        void init(LuaTable options)
        {
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

        void clearLuaRef()
        {
            foreach (var kv in detachors)
            {
                kv.Value();
            }
            detachors.Clear();
            detachors = null;
            options = null;
            attach = null;

            creator = null;
            eventSetter = null;
        }

        public void Dispose()
        {
            clearLuaRef();

            if (disposeLuaEnv)
            {
                luaEnv.Dispose();
            }

            luaEnv = null;
        }
    }
}
