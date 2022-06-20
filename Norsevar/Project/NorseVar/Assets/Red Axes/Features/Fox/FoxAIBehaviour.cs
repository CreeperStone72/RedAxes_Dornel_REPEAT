using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Norsevar.Fox
{

    public class FoxAIBehaviour : MonoBehaviour
    {
        #region Constants and Statics

        private static readonly int Speed = Animator.StringToHash( "Speed" );
        private static readonly int TurnRate = Animator.StringToHash( "TurnRate" );

        #endregion

        #region Private Fields

        private Animator _animator;
        private NavMeshAgent _agent;
        private bool _waiting;
        private Transform _currentDest;
        private int _foxDataIndex;
        private int _foxDataPathIndex;
        private bool _turning;

        #endregion

        #region Serialized Fields

        public float maxSpeed;

        [SerializeField] private List<FoxPathData> data;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            //Waiting, so done following its path and expecting a player interaction
            if ( _waiting || _agent.pathPending || _turning ) return;

            SetAnimatorValues();

            //Not waiting, so following its path
            //Do we have a dest?
            if ( _currentDest is null )
            {
                _currentDest = data[ _foxDataIndex ].pathPoints[ _foxDataPathIndex ];
                _agent.SetDestination( _currentDest.position );
                return;
            }

            //Did we arrive?
            if ( !( _agent.remainingDistance < 0.2f ) )
                return;
            
            _foxDataPathIndex++;

            if ( data[ _foxDataIndex ].pathPoints.Count == _foxDataPathIndex )
                TurnTo( data[ _foxDataIndex ].forwardTurn.rotation.eulerAngles );

            _currentDest = null;
        }

        #endregion

        #region Private Methods

        private void Arrived()
        {
            _waiting = true;
            _turning = false;
            data[ _foxDataIndex ].onArrive.Invoke();
            _animator.SetFloat( TurnRate, 0 );
            _animator.SetFloat( Speed, -1 );
        }

        private void SetAnimatorValues()
        {
            //"Inspired" by this thread: https://forum.unity.com/threads/detecting-if-a-navmeshagent-is-rotating.410197/
            Vector3 s = _agent.transform.InverseTransformDirection( _agent.velocity ).normalized;
            float speed = s.z;
            float turn = s.x;
            _animator.SetFloat( Speed, speed * maxSpeed );
            _animator.SetFloat( TurnRate, turn );
        }

        private void TurnTo( Vector3 rotation )
        {
            _turning = true;
            Transform localTransform;
            ( localTransform = transform ).DORotate( rotation, 2 ).OnComplete( Arrived );
            Quaternion toRot = Quaternion.FromToRotation( localTransform.rotation.eulerAngles, rotation );
            _animator.SetFloat( TurnRate, toRot.eulerAngles.y < 0 ? -1 : 1 );
            _animator.SetFloat( Speed, 0 );
        }

        #endregion

        #region Public Methods

        [Button]
        public void Resume()
        {
            data[ _foxDataIndex ].onResume.Invoke();
            _foxDataIndex++;
            _foxDataPathIndex = 0;
            _waiting = false;
        }

        #endregion

        [Serializable]
        private struct FoxPathData
        {
            #region Serialized Fields

            public List<Transform> pathPoints;
            public Transform forwardTurn;
            public UnityEvent onArrive;
            public UnityEvent onResume;

            #endregion
        }
    }

}