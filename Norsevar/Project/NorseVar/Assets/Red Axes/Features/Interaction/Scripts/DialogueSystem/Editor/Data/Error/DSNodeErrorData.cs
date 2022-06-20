using System.Collections.Generic;

namespace Norsevar.Interaction.DialogueSystem.Editor
{

    public class DSNodeErrorData
    {

        #region Constructors

        public DSNodeErrorData()
        {
            ErrorData = new DSErrorData();
            Nodes = new List<DSNode>();
        }

        #endregion

        #region Properties

        public DSErrorData ErrorData { get; }

        public List<DSNode> Nodes { get; }

        #endregion

    }

}
