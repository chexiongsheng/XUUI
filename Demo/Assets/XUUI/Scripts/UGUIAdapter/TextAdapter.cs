using UnityEngine;
using UnityEngine.UI;

namespace XUUI.UGUIAdapter
{
    public class TextAdapter : AdapterBase<Text>, DataConsumer<string>
    {
        public string Value
        {
            set
            {
                Target.text = value;
            }
        }
    }
}
