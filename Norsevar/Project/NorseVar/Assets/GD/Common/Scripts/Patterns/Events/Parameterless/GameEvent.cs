using System.Collections.Generic;
using UnityEngine;

namespace GD.Events
{

    [CreateAssetMenu( fileName = "GameEvent", menuName = "Scriptable Objects/Events/GameEvent" )]
    public class GameEvent : ScriptableObject
    {
        #region Private Fields

        private readonly List<GameEventListener> listeners = new List<GameEventListener>();

        #endregion

        #region Serialized Fields

        [SerializeField]
        [Header( "Descriptive Information (optional)" )]
        [ContextMenuItem( "Reset Name", "ResetName" )]
        private string Name;

        #endregion

        #region Public Methods

        [ContextMenu( "Raise Event" )]
        public virtual void Raise()
        {
            for ( int i = listeners.Count - 1; i >= 0; i-- ) listeners[ i ].OnEventRaised();
        }

        public void RegisterListener( GameEventListener listener )
        {
            if ( !listeners.Contains( listener ) )
                listeners.Add( listener );
        }

        public void UnregisterListener( GameEventListener listener )
        {
            if ( listeners.Contains( listener ) )
                listeners.Remove( listener );
        }

        #endregion
    }

}