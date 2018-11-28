using UnityEngine;
using XUUI;

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
