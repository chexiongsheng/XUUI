using UnityEngine;
using UnityEngine.UI;
using System;

namespace XUUI.UGUIAdapter
{
    public class ButtonAdapter : MonoBehaviour, EventEmitter
    {
        public Button Target;

        public string BindTo;

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
