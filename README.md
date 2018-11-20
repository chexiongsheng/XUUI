# XUUI

一个采用lua实现，和具体引擎，具体UI无关，可拓展的mvvm框架。

## 示例

### lua侧

接口和vue非常类似：

~~~lua
local xuui = require 'xuui'
local select_info = {'vegetables', 'meat'}

local mvvm = xuui.new {
   el = select(1, ...), -- 从参数传过来，或者类似CS.UnityEngine.GameObject.Find('Canvas')主动获取也可以
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
~~~

xuui.new(options)，options字段基本和vue一样

* el传要绑定的UI元素根节点
* data就是ViewModle（VM）
* computed中引用到的VM元素，在其依赖的VM元素发生改变会自动重新计算并同步到各个绑定了它（比如上例的message）的节点
* methods是类似按钮点击事件绑定的响应方法

返回是一个handler，目前有个detach方法用于去绑定。

### C#侧

将XXXAdapter拖到GameObject，然后设置Target和BindTo属性即可。

比如InputFieldAdapter，拖动到一个InputField节点，然后把该节点拖动到Target，设置BindTo（本实例为name），然后当lua侧VM中的data.name发生变化时，InputField会自动改变，更改该InputField时，也会自动修改data.name，进而同步到其它也绑定到name的UI节点。


## 扩展

本框架设计上就避免和具体某个UI库耦合，通过实现一套Adapter以及一个AdapterCollector，即可和任意UI库配合。

注意：这部分和具体UI适配一次后就可以不管了，后续本项目会逐渐完善对各种UI的适配器

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

