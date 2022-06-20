using UnityEngine;

namespace Norsevar.Room_Generation
{
    public class VectorLabelsAttribute : PropertyAttribute
    {

        #region Constructors

        public VectorLabelsAttribute(params string[] labels)
        {
            Labels = labels;
        }

        #endregion

        public readonly string[] Labels;
    }
}
