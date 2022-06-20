using System.Collections.Generic;
using UnityEngine.Rendering;

namespace Norsevar.Interaction.DialogueSystem.Utilities
{

    public static class CollectionUtility
    {

        #region Public Methods

        public static void AddItem<K, V>(this SerializedDictionary<K, List<V>> serializedDictionary, K key, V value)
        {
            if (serializedDictionary.ContainsKey(key))
            {
                serializedDictionary[key].Add(value);
                return;
            }

            serializedDictionary.Add(
                key,
                new List<V>
                {
                    value
                });

        }

        #endregion

    }

}
