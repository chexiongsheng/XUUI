using UnityEngine;
using XUUI;

public class TestDetach : MonoBehaviour
{
    ViewModel vm = null;

    GameObject panelLeft;
    GameObject panelRight;

    void Start()
    {
        vm = new ViewModel(@"
            local observeable = require 'observeable'

            return {
                data = {
                    name = 'john',
                    select = 0,
                    options = {'vegetables', 'meat'},
                },
                computed = {
                    message = function(data)
                        return 'Hello ' .. data.name .. ', your choice is ' .. tostring(data.options[data.select + 1])
                    end
                },
                methods = {
                    add_option = function(data)
                        local tmp = observeable.raw(data.options) -- 只有获取raw后，table.insert之类的函数才能正常操作
                        table.insert(tmp,'Option #' .. (#tmp + 1))
                        data.options = tmp
                    end,
                    static_csharp_callback = CS.SomeClass.Foo,
                },
            }
        ");

        vm.AddEventHandler("instance_csharp_callback", new SomeClass(1024), "Bar");
        vm.AddEventHandler("attachleft", this, "AttachLeft");
        vm.AddEventHandler("detachleft", this, "DetachLeft");
        vm.AddEventHandler("attachright", this, "AttachRight");
        vm.AddEventHandler("detachright", this, "DetachRight");


        var control = GameObject.Find("PanelControl");
        panelLeft = GameObject.Find("PanelLeft");
        panelRight = GameObject.Find("PanelRight");

        vm.Attach(control);
    }

    public void AttachLeft()
    {
        vm.Attach(panelLeft);
    }

    public void DetachLeft()
    {
        vm.Detach(panelLeft);
    }

    public void AttachRight()
    {
        vm.Attach(panelRight);
    }

    public void DetachRight()
    {
        vm.Detach(panelRight);
    }

    void OnDestroy()
    {
        vm.Dispose();
    }
}
