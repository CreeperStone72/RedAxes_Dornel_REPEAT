using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Norsevar.Interaction.DialogueSystem.Editor
{

    public sealed class DSGroup : Group
    {

        #region Private Fields

        private readonly Color _defaultBorderColor;
        private readonly float _defaultBorderWidth;

        #endregion

        #region Constructors

        public DSGroup(string groupTitle, Vector2 position)
        {

            Id = Guid.NewGuid().ToString();

            title = groupTitle;
            oldTitle = groupTitle;

            SetPosition(new Rect(position, Vector2.zero));

            _defaultBorderColor = contentContainer.style.borderBottomColor.value;
            _defaultBorderWidth = contentContainer.style.borderBottomWidth.value;
        }

        #endregion

        #region Properties

        public string Id { get; set; }

        #endregion

        #region Public Methods

        public void ResetStyle()
        {
            contentContainer.style.borderBottomColor = _defaultBorderColor;
            contentContainer.style.borderBottomWidth = _defaultBorderWidth;
        }

        public void SetErrorStyle(Color errorColor)
        {
            contentContainer.style.borderBottomColor = errorColor;
            contentContainer.style.borderBottomWidth = 2f;
        }

        #endregion

        public string oldTitle;
    }

}
