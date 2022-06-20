using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Norsevar.Interaction.DialogueSystem.Editor
{

    public static class DSElementUtility
    {

        #region Public Methods

        public static Button CreateButton(string title, Action onClick = null)
        {
            Button button = new(onClick)
            {
                text = title
            };

            return button;
        }

        public static EnumField CreateEnumField(Enum @enum, EventCallback<ChangeEvent<Enum>> onValueChanged = null)
        {
            EnumField enumField = new(@enum);

            enumField.RegisterCallback(onValueChanged);
            if (onValueChanged is not null) enumField.RegisterCallback(onValueChanged);

            return enumField;
        }

        public static Foldout CreateFoldout(string title, bool collapsed = false)
        {
            Foldout foldout = new()
            {
                text = title,
                value = !collapsed
            };

            return foldout;
        }

        public static Port CreatePort(
            this DSNode   textNode,
            string        portName    = "",
            Orientation   orientation = Orientation.Horizontal,
            Direction     direction   = Direction.Output,
            Port.Capacity capacity    = Port.Capacity.Single)
        {
            Port port = textNode.InstantiatePort(orientation, direction, capacity, typeof(bool));

            port.portName = portName;

            return port;
        }

        public static TextField CreateTextArea(
            string                             value          = null,
            string                             label          = null,
            EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextField textArea = CreateTextField(value, label, onValueChanged);
            textArea.multiline = true;
            return textArea;
        }

        public static TextField CreateTextField(
            string                             value          = null,
            string                             label          = null,
            EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextField textField = new()
            {
                value = value,
                label = label
            };

            if (onValueChanged is not null) textField.RegisterCallback(onValueChanged);

            return textField;
        }

        #endregion

    }

}
