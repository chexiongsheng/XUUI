using UnityEngine;
using UnityEngine.UI;

namespace XUUI.UGUIAdapter
{
    public class TextAdapter : MonoBehaviour, DataConsumer<string>
    {
        private Text target;

        public string BindTo;

        public string Value
        {
            set
            {
                target.text = value;
            }
        }

        void Start()
        {
            target = gameObject.GetComponent<Text>();
        }
    }
}
