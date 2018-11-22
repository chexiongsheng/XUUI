using UnityEngine;
using UnityEngine.UI;
using System;

namespace XUUI.UGUIAdapter
{
    [AddComponentMenu("XUUI/Button", 1)]
    [RequireComponent(typeof(Button))]
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
