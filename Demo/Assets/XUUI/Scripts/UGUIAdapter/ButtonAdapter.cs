using UnityEngine;
using UnityEngine.UI;
using System;

namespace XUUI.UGUIAdapter
{
    public class ButtonAdapter : AdapterBase<Button>, EventEmitter
    {
        public Action OnAction { get; set; }

        void Start()
        {
            Target.onClick.AddListener(() =>
            {
                if (OnAction != null)
                {
                    OnAction();
                }
            });
        }
    }
}
