using UnityEngine;
using XLua;
using XUUI;

public class Helloworld : MonoBehaviour
{
    ViewModel mvvm = null;

    void Start()
    {
        mvvm = new ViewModel(@"
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

        mvvm.Attach(gameObject);
    }

    void OnDestroy()
    {
        mvvm.Dispose();
    }
}
