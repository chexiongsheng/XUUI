using UnityEngine;
using XUUI;

public class NoLua : MonoBehaviour
{
    Context context = null;

    void Start()
    {
        context = new Context();
        context.AddCSharpModule("NoLua", this);
        context.Attach(gameObject);
    }

    [Command]
    public void Foo(Interface1 data)
    {
        Debug.Log(string.Format("NoLua.Foo, got name: {0}", data.name));
        data.name = "Foo";
    }

    [Command]
    public void Bar(Interface2 data)
    {
        Debug.Log(string.Format("NoLua.Foo, got select : {0}", data.select));

        data.select = data.select == 0 ? 1 : 0;
    }

    void OnDestroy()
    {
        context.Dispose();
    }
}
