using UnityEngine;

public class AdapterBase<T> : MonoBehaviour
{
    public T Target;

    public string BindTo;

    void Awake()
    {
        if (Target == null)
        {
            Target = gameObject.GetComponent<T>();
        }
    }
}
