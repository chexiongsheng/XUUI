using UnityEngine;
using UnityEngine.UI;
using System;

namespace XUUI.UGUIAdapter
{
    public class RawButtonAdapter : RawAdapterBase, EventEmitter
    {
        private Button target;

        public Action OnAction { get; set; }

        public RawButtonAdapter(Button button, string bindTo)
        {
            target = button;
            BindTo = bindTo;

            target.onClick.AddListener(() =>
            {
                if (OnAction != null)
                {
                    OnAction();
                }
            });
        }
    }
}
