# XUUI

一个采用lua（基于xLua）实现的mvvm框架。

* 可以和任意UI库配合，ugui，ngui，fairyGUI，你自己倒腾的UI库。。。Whatever you want
* 支持把本框架作为一个mvvm驱动器，纯用C#写逻辑
* 支持“计算属性”：“计算属性”依赖的各属性发生改变会触发“计算属性”的重计算

## 示例

### 设置绑定信息

比如把一个输入框绑定到"info.name"，那么当info.name改变了会自动同步到该输入框，该输入修改后也会通知到各依赖info.name的其它节点。

Helloworld示例UI节点的绑定信息如下：

* InputField: info.name
* Text      : message，这是个“计算属性”，其依赖于info.name以及select，当且仅当info.name以及select其中一个发生变化，会触发message重新计算（XUUI会自动计算依赖关系），并自动更新Text。
* Dropdown  : select
* Button    : reset，这会绑定到一个reset函数上

绑定怎么操作？

添加添加适配器（继承自MonoBehaviour）到GameObject，可以通过Component/XUUI菜单或者手动到XUUI\Scripts\UGUIAdapter目录找脚本拖放到GameObject，然后设置BindTo属性即可。

### 代码

~~~csharp
public class Helloworld : MonoBehaviour
{
    ViewModel vm = null;

    void Start()
    {
        vm = new ViewModel(@"
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

        vm.Attach(gameObject);
    }

    void OnDestroy()
    {
        vm.Dispose();
    }
}
~~~

根据一个lua脚本去new一个ViewModel，该脚本仅简单的返回一个table，该table各字段含义如下：

* data就是ViewModle（VM）
* computed中引用到的VM元素，在其依赖的VM元素发生改变会自动重新计算并同步到各个绑定了它（比如上例的message）的节点
* methods是类似按钮点击事件绑定的响应方法

然后就可以愉快的Attach到某个UI根节点了（可以Attach多个），这个UI跟节点设置了绑定信息的UI元素都会自动同步。

## 例子说明

* Helloworld.unity: 快速入门的例子。
* MoreComplex.unity: 演示混合使用lua，C#静态函数，C#成员函数作为事件响应，演示怎么监听一个数组的变化并应用到UI元素上。
* NoLua.unity(Assets/XLua/Examples/03_UIEvent/): 演示不使用lua，把本框架作为一个mvvm驱动器，纯用C#写逻辑。
* TestDetach.unity: 演示随意地挂载/卸载UI到一个ViewModel上。

## 扩展

本框架设计上就避免和具体某个UI库耦合，通过实现一套Adapter以及一个AdapterCollector，即可和任意UI库配合。

### Adapter实现

以Unity下的UGUI为例，划重点：

* 继承AdapterBase<要适配的UI类>
* 如果其需要监听VM变化，须实现DataConsumer接口（可以不显式声明实现，只要有DataConsumer声明的接口即可）
* 如果其需要把数据同步回VM，须实现DataProducer接口
* 如果其需要产生一个事件，须实现EventEmitter接口

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

