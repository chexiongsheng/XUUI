using UnityEngine;
using UnityEngine.UI;

namespace XUUI.UGUIAdapter
{
    public class TextAdapter : MonoBehaviour, DataConsumer<string>
    {
        public Text Target;

        public string BindTo;

        public string Value
        {
            set
            {
                Target.text = value;
            }
        }

        void Awake()
        {
            if (Target == null)
            {
                Target = gameObject.GetComponent<Text>();
            }
        }
    }
}
