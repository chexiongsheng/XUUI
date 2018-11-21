# XUUI

一个采用lua（基于xLua）实现的mvvm框架。

* 可以和任意UI库配合，ugui，ngui，fairyGUI。。。Whatever you want
* 支持把本框架作为一个mvvm驱动器，纯用C#写逻辑
* 支持“计算属性”：“计算属性”依赖的各属性发生改变会触发“计算属性”的重计算

## 示例

### 设置绑定信息

将XXXAdapter拖到GameObject，然后设置BindTo属性即可。

比如InputField，添加一个InputFieldAdapter，设置BindTo信息为"info.name"，当info.name改变了会自动同步到该InputField，该InputField修改后也会同步通知到各依赖info.name的节点

本示例UI节点的绑定信息如下：

* InputField: info.name
* Text      : message
* Dropdown  : select
* Button    : reset

### 代码

~~~csharp
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
                    message = function(data) -- “计算属性”
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
~~~

MVVM构造函数的参数1是要绑定的ui根节点，参数2是一个lua脚本，该脚本仅简单的返回一个table，该table各字段含义如下：

* data就是ViewModle（VM）
* computed中引用到的VM元素，在其依赖的VM元素发生改变会自动重新计算并同步到各个绑定了它（比如上例的message）的节点
* methods是类似按钮点击事件绑定的响应方法


## 扩展

本框架设计上就避免和具体某个UI库耦合，通过实现一套Adapter以及一个AdapterCollector，即可和任意UI库配合。

### Adapter实现

以Unity下的UGUI为例，划重点：

* 一个Adapter，必须得有一个BindTo字段，用于设置/访问绑定信息
* 如果其需要监听VM变化，须实现DataConsumer接口（可以不显式声明实现，只要有DataConsumer声明的接口即可）
* 如果其需要把数据同步回VM，须实现DataProducer接口
* 如果其需要产生一个事件，须实现EventEmitter接口

~~~csharp
using UnityEngine;
using UnityEngine.UI;
using System;

namespace XUUI.UGUIAdapter
{
    public class InputFieldAdapter : MonoBehaviour, DataConsumer<string>, DataProducer<string>
    {
        public InputField Target;

        public string BindTo;

        public Action<string> OnValueChange { get; set; } // InputField发生变化需要调用OnValueChange

        public string Value // VM发生变化，会调用到该Setter，需要同步给InputField
        {
            set
            {
                Target.text = value;
            }
        }

        void Start()
        {
            Target.onValueChange.AddListener((val) =>
            {
                if (OnValueChange != null)
                {
                    OnValueChange(val);
                }
            });
        }
    }
}
~~~

### AdapterCollector

提供一个lua模块，有一个collect方法，接受根UI节点，返回DataConsumer，DataProducer以及EventEmitter

UGUI的实现是采用C#和lua相结合的办法。

lua代码很简单，直接调用C#，然后把数组转成table

~~~lua
local _M = {}


function _M.collect(go)
    local infos = CS.XUUI.UGUIAdapter.Collector.Collect(go)
    local r = {}
    
    for i = 0, infos.Length - 1 do
        local objs = infos[i]
        local t = {}
        for j = 0, objs.Length - 1 do
            table.insert(t, objs[j])
        end
        table.insert(r, t)
    end
    
    return r
end

return _M
~~~

C#测也不复杂

~~~csharp
using UnityEngine;
using System.Linq;

namespace XUUI.UGUIAdapter
{
    public class Collector
    {
        // [0]: DataConsumers
        // [1]: DataProducers
        // [2]: EventEmitters
        public static object[][] Collect(GameObject go)
        {
            var dataProducers = go.GetComponentsInChildren<InputFieldAdapter>(true)
                .Select(o => (object)o)
                .Concat(go.GetComponentsInChildren<DropdownAdapter>(true))
                .ToArray();

            var dataConsumers = go.GetComponentsInChildren<TextAdapter>(true)
                .Select(o => (object)o)
                .Concat(dataProducers)
                .ToArray();

            var eventEmitters = go.GetComponentsInChildren<ButtonAdapter>(true).Select(o => (object)o).ToArray();

            return new object[][] { dataConsumers, dataProducers, eventEmitters };
        }
    }
}
~~~

