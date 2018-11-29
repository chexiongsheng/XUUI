# 如何和任意UI库适配

本框架设计上就避免和具体某个UI库耦合，通过实现一套Adapter以及一个AdapterCollector，即可和任意UI库配合。

## Adapter实现

Adapter只要满足如下需求即可

* 有个public string BindTo字段
* 如果其需要监听VM变化，须实现DataConsumer接口（可以不显式声明实现，只要有DataConsumer声明的接口即可）
* 如果其需要把数据同步回VM，须实现DataProducer接口
* 如果其需要产生一个事件，须实现EventEmitter接口

有两个思路：
* 每个Adapter是MonoBehaviour，直接绑定到对应控件上进行绑定信息的设置
* 每个Adapter只是个普通对象，然后用一个MonoBehaviour设置整个UI树（View）的绑定信息

以ugui的InputField为例，介绍两个思路的实现

### 思路1

继承AdapterBase<InputField>，AdapterBase<InputField>继承自MonoBehaviour，有BindTo字段，并实现了自动查找InputField的功能，代码如下：

~~~csharp
using UnityEngine;
using UnityEngine.UI;
using System;

namespace XUUI.UGUIAdapter
{
    public class InputFieldAdapter : AdapterBase<InputField>, DataConsumer<string>, DataProducer<string>
    {

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

### 思路2

继承RawAdapterBase，RawAdapterBase只是个普通对象，声明了个BindTo字段，仅此而已，代码如下：

~~~csharp
using UnityEngine.UI;
using System;

namespace XUUI.UGUIAdapter
{
    public class RawInputFieldAdapter : RawAdapterBase, DataConsumer<string>, DataProducer<string>
    {
        private InputField target;

        public Action<string> OnValueChange { get; set; } // InputField发生变化需要调用OnValueChange

        public string Value // VM发生变化，会调用到该Setter，需要同步给InputField
        {
            set
            {
                target.text = value == null ? "" : value;
            }
        }

        public RawInputFieldAdapter(InputField input, string bindTo)
        {
            target = input;
            BindTo = bindTo;

            target.onValueChange.AddListener((val) =>
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

一个统一的设置类，用于设置绑定信息

~~~csharp
namespace XUUI.UGUIAdapter
{
    [Serializable]
    public class ButtonBindTo
    {
        public Button Target;
        public string BindTo;
    }

    [Serializable]
    public class TextBindTo
    {
        public Text Target;
        public string BindTo;
    }

    [Serializable]
    public class DropdownBindTo
    {
        public Dropdown Target;
        public string BindTo;
    }

    [Serializable]
    public class InputFieldBindTo
    {
        public InputField Target;
        public string BindTo;
    }

    public class BindToSetting : MonoBehaviour
    {
        public ButtonBindTo[] Buttons;

        public TextBindTo[] Texts;

        public DropdownBindTo[] Dropdowns;

        public InputFieldBindTo[] InputFields;
    }
}
~~~

## AdapterCollector

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

C#测也不复杂，对应思路1的Collector如下：

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
            var adapters = go.GetComponentsInChildren<AdapterBase>(true);

            var dataProducers = adapters
                .Where(adapter => adapter is DataProducer)
                .Select(o => (object)o)
                .ToArray();

            var dataConsumers = adapters
                .Where(adapter => adapter is DataConsumer)
                .Select(o => (object)o)
                .ToArray();

            var eventEmitters = adapters
                .Where(adapter => adapter is EventEmitter)
                .Select(o => (object)o)
                .ToArray();

            return new object[][] { dataConsumers, dataProducers, eventEmitters };
        }
    }
}
~~~

如果是思路2的Collect逻辑如下：
~~~csharp
static object[][] collect(BindToSetting setting)
{
	var dataProducers = new List<object>();
	var dataConsumers = new List<object>();
	var eventEmitters = new List<object>();

	if (setting.InputFields != null)
	{
		var adpaters = setting.InputFields.Select(item => (object)new RawInputFieldAdapter(item.Target, item.BindTo));
		dataConsumers.AddRange(adpaters);
		dataProducers.AddRange(adpaters);

	}

	if (setting.Dropdowns != null)
	{
		var adpaters = setting.Dropdowns.Select(item => (object)new RawDropdownAdapter(item.Target, item.BindTo));
		dataConsumers.AddRange(adpaters);
		dataProducers.AddRange(adpaters);
	}

	if (setting.Texts != null)
	{
		dataConsumers.AddRange(setting.Texts.Select(item => (object)new RawTextAdapter(item.Target, item.BindTo)));
	}

	if (setting.Buttons != null)
	{
		eventEmitters.AddRange(setting.Buttons.Select(item => (object)new RawButtonAdapter(item.Target, item.BindTo)));
	}

	return new object[][] { dataConsumers.ToArray(), dataProducers.ToArray(), eventEmitters.ToArray() };
}
~~~
