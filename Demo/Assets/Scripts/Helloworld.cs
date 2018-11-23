using UnityEngine;
using XUUI;

public class Helloworld : MonoBehaviour
{
    Context context = null;

    void Start()
    {
        context = new Context(@"
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
                commands = {
                    reset = function(data)
                        data.info.name = 'john'
                        data.select = 0
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
