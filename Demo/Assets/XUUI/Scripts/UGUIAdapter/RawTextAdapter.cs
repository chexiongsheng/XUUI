using UnityEngine;
using UnityEngine.UI;

namespace XUUI.UGUIAdapter
{
    public class RawTextAdapter : RawAdapterBase, DataConsumer<string>
    {
        private Text target;

        public string Value
        {
            set
            {
                target.text = value;
            }
        }

        public RawTextAdapter(Text text, string bindTo)
        {
            target = text;
            BindTo = bindTo;
        }
    }
}
