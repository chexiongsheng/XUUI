using UnityEngine;

public interface Interface1
{
    string name { get; set; }
}

public interface Interface2
{
    int select { get; set; }
}

public class SomeClass
{
    int i;

    public SomeClass(int i)
    {
        this.i = i;
    }

    public static void Foo(Interface1 data)
    {
        Debug.Log(string.Format("SomeClass.Foo, got name: {0}", data.name));
        data.name = "Foo";
    }

    public void Bar(Interface2 data)
    {
        Debug.Log(string.Format("SomeClass.Foo i = {0}, got select : {1}", i,  data.select));

        data.select = data.select == 0 ? 1 : 0;
    }
}

