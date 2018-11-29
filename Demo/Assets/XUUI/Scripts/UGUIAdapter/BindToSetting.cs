using UnityEngine;
using UnityEngine.UI;
using System;

namespace XUUI.UGUIAdapter
{
    [Serializable]
    public class ButtonBindTo
    {
        public Button Target;
        public string BindTo;
    }

    [Serializable]
    public class TextBindTo
    {
        public Text Target;
        public string BindTo;
    }

    [Serializable]
    public class DropdownBindTo
    {
        public Dropdown Target;
        public string BindTo;
    }

    [Serializable]
    public class InputFieldBindTo
    {
        public InputField Target;
        public string BindTo;
    }

    public class BindToSetting : MonoBehaviour
    {
        public ButtonBindTo[] Buttons;

        public TextBindTo[] Texts;

        public DropdownBindTo[] Dropdowns;

        public InputFieldBindTo[] InputFields;
    }
}
