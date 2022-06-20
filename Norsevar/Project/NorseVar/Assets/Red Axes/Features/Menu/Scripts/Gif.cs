using UnityEngine;

namespace Norsevar.Menu
{
    [CreateAssetMenu(fileName = "Gif", menuName = "Norsevar/Gif")]
    public class Gif : ScriptableObject
    {

        #region Serialized Fields

        [SerializeField] private Texture2D[] texture2D;
        [SerializeField] private int fps = 24;

        #endregion

        #region Public Methods

        public float GetFPS()
        {
            return 1.0f / fps;
        }

        public int Size()
        {
            return texture2D.Length;
        }

        #endregion

        public Texture2D this[int i] => texture2D[i];
    }
}
