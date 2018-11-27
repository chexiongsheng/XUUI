## 应用框架的核心目标

* 模块加载
* 模块间（数据）隔离
* 模块间可控交互


## 定义一个App

Context的构造函数传入的是一个含modules字段以及name字段的table即可。

~~~csharp
context = new Context(@"
    return {
        name  = 'myapp', 
        modules = {'module1', 'module2'}, 
   }
");
~~~

定义了一个名字为myapp的app以及两个模块：module1，module2

框架会做这些事情：

* 加载myapp.module1，myapp.module2，加载的规则和require是一致的，为了演示方便，本实例两个模块放在Resources.myapp目录，实际上通过CustomLoader可以实现以任意后缀放在任意目录
* 为这两个设置独立的沙盒，各模块即使定义了全局变量也互不影响，一定程度上减轻不同模块开发者由于沟通不足或者笔误引发的模块间冲突
* 模块间数据隔离：模块也可以定义data、commands、computed，在模块定义的commands和computed只能看到本模块的data
* 模块间调用：通过exports字段可以导出一些函数供其它模块调用，其它模块可以通过“模块名.函数名”调用
* 支持模块刷新（reload），reload后data变动会更新UI，监听原先commands也会自动更新到新的commands，computed会自动重新计算并更新UI

module1代码

~~~lua
return {
    data = {
	    name = "haha", 
		select = 0, -- ui通过 module1.select来绑定
	},
	
	commands = {
		click = function(data)
		    module2.set_select(data.select) -- 可以调用别的模块exports的接口
			data.select = data.select == 0 and 1 or 0 -- command只能看到/修改自己的数据
		end,
	},
    
    computed = {
        info = function(data)
            return string.format('i am %s, my select is %d', data.name, data.select)
        end,
    },
	
	exports = {
	    hello = function(p) -- 可以被其它module调用
		    print('hello, p = '.. p)
		end,
	},
}
~~~

module2代码

~~~lua
local data = {
    message = "hehe",
    select = 1,
}
	
return {
    data = data,
	
	commands = {
		click = function(data)
		    module1.hello(1)
			data.select = data.select == 0 and 1 or 0
		end,
	},
    
    computed = {
        info = function(data)
            return string.format('message is %s, select is %d', data.message, data.select)
        end,
    },
	
	exports = {
	    set_select = function(p)
		    data.select = p
		end,
	},
}
~~~

## UI绑定

UI不像逻辑那样划分模块，通过“模块名.模块内路径”去进行数据/响应的绑定，比如moudle1.select，module2.click等等。


