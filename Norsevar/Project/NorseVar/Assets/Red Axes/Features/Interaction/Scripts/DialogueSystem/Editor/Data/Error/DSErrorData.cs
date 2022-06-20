using UnityEngine;

namespace Norsevar.Interaction.DialogueSystem.Editor
{

    public class DSErrorData
    {

        #region Constructors

        public DSErrorData()
        {
            GenerateRandomColor();
        }

        #endregion

        #region Properties

        public Color Color { get; private set; }

        #endregion

        #region Private Methods

        private void GenerateRandomColor()
        {
            Color = new Color32((byte)Random.Range(65, 256), (byte)Random.Range(50, 176), (byte)Random.Range(50, 176), 255);
        }

        #endregion

    }

}
