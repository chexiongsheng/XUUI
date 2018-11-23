using UnityEngine;
using XUUI;

public class TestDetach : MonoBehaviour
{
    Context context = null;

    GameObject panelLeft;
    GameObject panelRight;

    void Start()
    {
        context = new Context(@"
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
                commands = {
                    add_option = function(data)
                        local tmp = observeable.raw(data.options) -- 只有获取raw后，table.insert之类的函数才能正常操作
                        table.insert(tmp,'Option #' .. (#tmp + 1))
                        data.options = tmp
                    end,
                    static_csharp_callback = CS.SomeClass.Foo,
                },
            }
        ");

        context.AddCommand("instance_csharp_callback", new SomeClass(1024), "Bar");
        context.AddCommand("attachleft", this, "AttachLeft");
        context.AddCommand("detachleft", this, "DetachLeft");
        context.AddCommand("attachright", this, "AttachRight");
        context.AddCommand("detachright", this, "DetachRight");


        var control = GameObject.Find("PanelControl");
        panelLeft = GameObject.Find("PanelLeft");
        panelRight = GameObject.Find("PanelRight");

        context.Attach(control);
    }

    public void AttachLeft()
    {
        context.Attach(panelLeft);
    }

    public void DetachLeft()
    {
        context.Detach(panelLeft);
    }

    public void AttachRight()
    {
        context.Attach(panelRight);
    }

    public void DetachRight()
    {
        context.Detach(panelRight);
    }

    void OnDestroy()
    {
        context.Dispose();
    }
}
