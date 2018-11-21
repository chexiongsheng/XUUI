using UnityEngine;
using UnityEngine.UI;
using System;

namespace XUUI.UGUIAdapter
{
    public class ButtonAdapter : MonoBehaviour, EventEmitter
    {
        private Button target;

        public string BindTo;

        public Action OnAction { get; set; }

        void Start()
        {
            target = gameObject.GetComponent<Button>();
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
