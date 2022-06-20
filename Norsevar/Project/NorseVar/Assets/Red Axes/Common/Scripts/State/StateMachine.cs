using System;
using System.Collections.Generic;
using System.Linq;

namespace Norsevar
{

    public interface IState<out T>
    {

        #region Public Methods

        T GetState();

        void OnEnter();

        void OnExit();

        void Update(); //Tick()

        #endregion

    }

    public class StateMachine<T>
    {

        #region Constants and Statics

        private static readonly List<Transition> EmptyTransitions = new(0);

        #endregion

        #region Private Fields

        private readonly Dictionary<Type, List<Transition>> _transitions = new();
        private readonly List<Transition> _anyTransitions = new();
        private List<Transition> _currentTransitions = new();

        private IState<T> _currentState;

        #endregion

        #region Properties

        public T State => _currentState.GetState();

        #endregion

        #region Private Methods

        private Transition GetTransition()
        {
            foreach (Transition transition in _anyTransitions.Where(pTransition => pTransition.Condition()))
                return transition;

            return _currentTransitions.FirstOrDefault(pTransition => pTransition.Condition());

        }

        #endregion

        #region Public Methods

        public void AddAnyTransition(IState<T> pState, Func<bool> pCondition)
        {
            _anyTransitions.Add(new Transition(pState, pCondition));
        }

        public void AddTransition(IState<T> pFrom, IState<T> pTo, Func<bool> pCondition)
        {
            if (_transitions.TryGetValue(pFrom.GetType(), out List<Transition> transitionsList) == false)
            {
                transitionsList = new List<Transition>();
                _transitions[pFrom.GetType()] = transitionsList; //Dictionary => [{StandingClosed, List[]}, {Opening, List[]}]
            }
            transitionsList.Add(
                new Transition(pTo, pCondition)); //Dictionary => [{StandingClosed, List[Transition(Opening, HasReceivedOpenRequest)]}}]
        }

        public void SetState(IState<T> pState)
        {
            if (pState == _currentState)
                return;

            _currentState?.OnExit();
            _currentState = pState;

            _transitions.TryGetValue(_currentState.GetType(), out _currentTransitions);
            _currentTransitions ??= EmptyTransitions;

            _currentState.OnEnter();
        }

        public void Update()
        {
            Transition transition = GetTransition();
            if (transition != null)
                SetState(transition.To);

            _currentState?.Update();
        }

        #endregion

        private class Transition
        {

            #region Constructors

            public Transition(IState<T> pTo, Func<bool> pCondition)
            {
                To = pTo;
                Condition = pCondition;
            }

            #endregion

            #region Properties

            public Func<bool> Condition { get; }

            public IState<T> To { get; }

            #endregion

        }
    }

}
