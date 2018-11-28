using UnityEngine;
using XUUI;

public class MoreComplex : MonoBehaviour
{
    Context context = null;

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

        context.Attach(gameObject);
    }

    void OnDestroy()
    {
        context.Dispose();
    }
}
