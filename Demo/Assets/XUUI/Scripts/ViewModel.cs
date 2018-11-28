using UnityEngine;
using System.Collections.Generic;
using XLua;
using System;

namespace XUUI
{
    public delegate void ContextCreator(LuaTable options, out Func<GameObject, Action> attach, out Action<string, bool> reload);
    public class Context : IDisposable
    {
        LuaEnv luaEnv = null;

        ContextCreator creator = null;

        Action<LuaTable, string, object, string> commandSetter = null;

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

            creator = luaEnv.LoadString<Func<ContextCreator>>(@"
                        return (require 'xuui').new
                    ", "@xuui_init.lua")();

            commandSetter = luaEnv.LoadString<Func<Action<LuaTable, string, object, string>>>(@"
                        return function(options, eventName, obj, methodName)
                            options.commands = options.commands or {}
                            local func = obj[methodName]
                            options.commands[eventName] = function(data)
                                func(obj, data)
                            end
                        end
                    ", "@eventSetter.lua")();
        }


        Func<GameObject, Action> attach;
        Action<string, bool> reload;

        public Func<LuaTable> Compile(string script)
        {
            return luaEnv.LoadString<Func<LuaTable>>(script);
        }

        public Context(LuaEnv env = null)
        {
            initLua(env);
            init(luaEnv.NewTable());
        }

        public Context(string script, LuaEnv env = null)
        {
            initLua(env);
            init(Compile(script)());
        }

        public Context(Func<LuaTable> compiled, LuaEnv env = null)
        {
            initLua(env);
            init(compiled());
        }

        LuaTable options;

        void init(LuaTable options)
        {
            this.options = options;
            creator(options, out attach, out reload);
        }

        Dictionary<GameObject, Action> detachs = new Dictionary<GameObject, Action>();

        public void Attach(GameObject view, bool throwIfFound = false)
        {
            if (detachs.ContainsKey(view))
            {
                if (throwIfFound)
                {
                    throw new InvalidOperationException("attached GameObject");
                }
                return;
            }
            var detach = attach(view);
            detachs.Add(view, detach);
        }

        public void Detach(GameObject view, bool throwIfNotFound = false)
        {
            Action detach;
            if (detachs.TryGetValue(view, out detach))
            {
                detachs.Remove(view);
                detach();
            }
            else if (throwIfNotFound)
            {
                throw new InvalidOperationException("not attached yet!");
            }
        }

        public void ReloadModule(string moduleName, bool reloadData = false)
        {
            reload(moduleName, reloadData);
        }

        public void AddCommand(string commandName, object obj, string methodName)
        {
            commandSetter(options, commandName, obj, methodName);
        }

        void clearLuaRef()
        {
            foreach (var kv in detachs)
            {
                kv.Value();
            }
            detachs.Clear();
            detachs = null;
            options = null;
            attach = null;
            reload = null;

            creator = null;
            commandSetter = null;
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
