using System;
using UnityEngine;

namespace Norsevar.Menu
{
    [Serializable]
    public class Sprites
    {

        #region Serialized Fields

        [SerializeField] private Sprite[] sprites;

        #endregion

        #region Properties

        public int Length => sprites.Length;

        #endregion

        public Sprite this[int index] => sprites[index];
    }
}
