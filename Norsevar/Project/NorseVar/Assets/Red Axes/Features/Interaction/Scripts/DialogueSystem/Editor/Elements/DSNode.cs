using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Norsevar.Interaction.DialogueSystem.Editor
{

    public abstract class DSNode : Node
    {

        #region Private Fields

        private Color _defaultBackgroundColor;

        #endregion

        #region Protected Fields

        protected DSGraphView graphView;

        #endregion

        #region Properties

        public string DialogueName { get; protected set; }

        public string Id { get; set; }

        public List<DSChoiceSaveData> Choices { get; set; }

        public DSDialogueType DialogueType { get; protected set; }

        public DSGroup Group { get; set; }

        #endregion

        #region Private Methods

        private void DisconnectInputPorts()
        {
            DisconnectPorts(inputContainer);
        }

        private void DisconnectOutputPorts()
        {
            DisconnectPorts(outputContainer);
        }

        private void DisconnectPorts(VisualElement element)
        {
            foreach (VisualElement visualElement in element.Children())
            {
                Port child = (Port)visualElement;

                if (!child.connected) continue;

                graphView.DeleteElements(child.connections);
            }
        }

        #endregion

        #region Public Methods

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Disconnect Input Ports", _ => DisconnectInputPorts());
            evt.menu.AppendAction("Disconnect Output Ports", _ => DisconnectOutputPorts());

            base.BuildContextualMenu(evt);
        }

        public void DisconnectAllPorts()
        {
            DisconnectInputPorts();
            DisconnectOutputPorts();
        }

        public abstract void Draw();

        public virtual void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            Id = Guid.NewGuid().ToString();

            graphView = dsGraphView;

            DialogueName = nodeName;
            Choices = new List<DSChoiceSaveData>();

            SetPosition(new Rect(position, Vector2.zero));

            mainContainer.AddToClassList("ds-node_main-container");
            extensionContainer.AddToClassList("ds-node_extension-container");

            _defaultBackgroundColor = mainContainer.style.backgroundColor.value;
        }

        public bool IsStartingNode()
        {
            Port inputPort = (Port)inputContainer.Children().First();

            return !inputPort.connected;
        }

        public void ResetColor()
        {
            mainContainer.style.backgroundColor = _defaultBackgroundColor;
        }

        public void SetErrorStyle(Color errorColor)
        {
            mainContainer.style.backgroundColor = errorColor;
        }

        #endregion

    }

}
