using UnityEngine;
using XUUI;

public class App : MonoBehaviour
{
    Context context = null;

    void Start()
    {
        context = new Context(@"
            return {
                name  = 'myapp', 
                modules = {'module1', 'module2'}, -- 定义了modules就是使用app模式，module1模块将会以类似require 'myapp.module1'的方式加载
           }
        ");

        context.AddCSharpModule("ModuleManager", this);
        context.Attach(gameObject);
    }

    void OnDestroy()
    {
        context.Dispose();
    }

    [Export]
    public void HelloCSharp(Interface2 data, int p)
    {
        Debug.Log("data.select=" + data.select + ", p=" + p);
    }

    [Command]
    public void ReloadModule1()
    {
        context.ReloadModule("module1");
    }

    [Command]
    public void ReloadModule2()
    {
        context.ReloadModule("module2");
    }
}
