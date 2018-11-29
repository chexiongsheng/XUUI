# XUUI

基于xLua的轻量级UI框架。

## 两大核心能力

* 支持MVVM的单/双向绑定
* 应用框架：模块加载，模块刷新（reload）、模块间（数据）隔离、模块间可控交互

## 科普MVVM

* mvvm框架，支持你在UI上设置一些绑定路径，比如：info.name ，select啥的，然后你在逻辑代码那修改info.name ，所有绑定到info.name的UI组件都会**自动**发生变化，这是单向绑定。
* mvvm框架还支持双向绑定：比如输入框绑定到info.name ，那么这个输入框的输入会自动修改info.name ，进而导致所有其它绑定到info.name 的UI组件都会自动发生变化。

## 特点

* 可以和任意UI库配合，ugui，ngui，fairyGUI，你自己倒腾的UI库。。。Whatever you want
* 支持把本框架作为一个mvvm驱动器，纯用C#写逻辑
* 支持“计算属性”：“计算属性”依赖的各属性发生改变会触发“计算属性”的重计算
* 可随时绑定View以及解绑定

## 示例

### 设置绑定信息

怎么操作？添加添加适配器（继承自MonoBehaviour）到GameObject，可以通过Component/XUUI菜单或者手动到XUUI\Scripts\UGUIAdapter目录找脚本拖放到GameObject，然后设置BindTo属性即可。

Helloworld示例UI节点的绑定信息如下：

* InputField: info.name
* Text      : message，这是个“计算属性”，计算时用了info.name，当info.name发生变化会触发message重新计算，并自动更新Text。
* Button    : click，这会绑定到一个click command上

### 代码

~~~csharp
public class Helloworld : MonoBehaviour
{
    Context context = null;

    void Start()
    {
        context = new Context(@"
            return {
                data = {
                    info = {
                        name = 'John',
                    },
                },
                computed = {
                    message = function(data)
                        return 'Hello ' .. data.info.name .. '!'
                    end
                },
                commands = {
                    click = function(data)
                        print(data.info.name)
                    end,
                },
            }
        ");

        context.Attach(gameObject);
    }

    void OnDestroy()
    {
        context.Dispose();
    }
}
~~~

根据一个lua脚本去new一个Context，该脚本仅简单的返回一个table，该table各字段含义如下：

* data就是ViewModle（VM）
* computed中引用到的VM元素，在其依赖的VM元素发生改变会自动重新计算并同步到各个绑定了它（比如上例的message）的节点
* commands是类似按钮点击事件绑定的响应方法

然后就可以愉快的Attach到某个UI根节点了（可以Attach多个），这个UI跟节点设置了绑定信息的UI元素都会自动同步。

## 应用框架

Helloworld例子展现的是类似vue.js的能力，实际项目中，更建议以模块的方式来组织程序。XUUI提供的应用框架，能很好的实现模块间隔离，也能提供模块间的可控交互能力。

详细请看[《应用框架》](Docs/App.md)以及配套的实例程序。

## 例子说明

* Helloworld.unity: 快速入门的例子。
* MoreComplex.unity: 演示混合使用lua，C#静态函数，C#成员函数作为事件响应，演示怎么监听一个数组的变化并应用到UI元素上。
* NoLua.unity(Assets/XLua/Examples/03_UIEvent/): 演示不使用lua，把本框架作为一个mvvm驱动器，纯用C#写逻辑。
* TestDetach.unity: 演示随意地挂载/卸载UI到一个ViewModel上。
* App.unity: 演示应用框架怎么使用。

## 怎么支持各种UI

详细请看[《如何和任意UI库适配》](Docs/GuiAdapter.md)

