using System.Collections.Generic;

namespace Norsevar.Interaction.DialogueSystem.Editor
{

    public class DSGroupErrorData
    {

        #region Constructors

        public DSGroupErrorData()
        {
            ErrorData = new DSErrorData();
            Groups = new List<DSGroup>();
        }

        #endregion

        #region Properties

        public DSErrorData ErrorData { get; }

        public List<DSGroup> Groups { get; }

        #endregion

    }

}
