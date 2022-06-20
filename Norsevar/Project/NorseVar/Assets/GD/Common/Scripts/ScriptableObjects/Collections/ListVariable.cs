using System.Collections.Generic;
using UnityEngine;

namespace GD.ScriptableTypes
{

    public abstract class ListVariable<T> : NumberVariable<T>
    {
        #region Serialized Fields

        [SerializeField]
        private List<T> list;

        #endregion

        #region Public Methods

        public void Add( T obj ) { list.Add( obj ); }

        public void Clear() { list.Clear(); }

        public int Count() => list.Count;

        public void Remove( T obj ) { list.Remove( obj ); }

        #endregion

        //Sort(comparable), Remove(predicate)
    }

}