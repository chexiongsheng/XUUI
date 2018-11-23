using UnityEngine;
using XUUI;

public class App : MonoBehaviour
{
    Context context = null;

    void Start()
    {
        context = new Context(@"
            return {
                __type = 'app',
                name  = 'myapp', 
                modules = {'module1', 'module2'},
           }
        ");

        context.AddCommand("module2.cscmd", this, "CSCmd");
        context.Attach(gameObject);
    }

    void OnDestroy()
    {
        context.Dispose();
    }

    public void CSCmd(Interface2 data)
    {
        Debug.Log("data.select=" + data.select);
    }
}
