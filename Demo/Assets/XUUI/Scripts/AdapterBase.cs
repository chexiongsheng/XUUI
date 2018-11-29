using UnityEngine;

namespace XUUI
{
    public class AdapterBase<T> : MonoBehaviour
    {
        public T Target;

        [TextArea]
        public string BindTo;

        void Awake()
        {
            if (Target == null)
            {
                Target = gameObject.GetComponent<T>();
            }
        }
    }
}
