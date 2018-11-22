using UnityEngine;
using XUUI;

public class NoLua : MonoBehaviour
{
    ViewModel vm = null;

    void Start()
    {
        vm = new ViewModel();
        vm.AddEventHandler("Foo", this, "Foo");
        vm.AddEventHandler("Bar", this, "Bar");
        vm.Attach(gameObject);
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
        vm.Dispose();
    }
}
