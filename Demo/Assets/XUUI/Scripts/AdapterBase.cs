using UnityEngine;

namespace XUUI
{
    public class AdapterBase : MonoBehaviour
    {
        [TextArea]
        public string BindTo;
    }

    public class AdapterBase<T> : AdapterBase
    {
        public T Target;

        void Awake()
        {
            if (Target == null)
            {
                Target = gameObject.GetComponent<T>();
            }
        }
    }
}
