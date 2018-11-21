using UnityEngine;
using XUUI;

public class NoLua : MonoBehaviour
{
    MVVM mvvm = null;

    void Start()
    {
        mvvm = new MVVM(gameObject);
        mvvm.AddEventHandler("Foo", this, "Foo");
        mvvm.AddEventHandler("Bar", this, "Bar");
        mvvm.Attach();
    }

    public void Foo(Interface1 data)
    {
        Debug.Log(string.Format("NoLua.Foo, got name: {0}", data.name));
        data.name = "Foo";
    }

    public void Bar(Interface2 data)
    {
        Debug.Log(string.Format("NoLua.Foo, got select : {0}", data.select));

        data.select = data.select == 0 ? 1 : 0;
    }

    void OnDestroy()
    {
        mvvm.Dispose();
    }
}
