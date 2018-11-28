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
    public static void Foo(Interface1 data)
    {
        Debug.Log(string.Format("SomeClass.Foo, got name: {0}", data.name));
        data.name = "Foo";
    }
}

