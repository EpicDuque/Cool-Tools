using System;
using System.Collections.Generic;

namespace CoolTools.Utilities
{
    public class StateMachine
    {
        public State CurrentState { get; protected set; }
        public State PreviousState { get; protected set; }

        public void Initialize(State state)
        {
            CurrentState = state;
        }

        public virtual void UpdateFSM()
        {
            var newState = CurrentState.EvaluateTransitions();

            if (newState != null)
            {
                TransitionTo(newState);
                return;
            }
            
            CurrentState.Update();
        }

        public virtual void TransitionTo(State newState)
        {
            CurrentState.OnExit();
            
            PreviousState = CurrentState;
            CurrentState = newState;
            
            CurrentState.OnEnter();
        }

        public abstract class State
        {
            public StateMachine Context { get; }
            private readonly Dictionary<State, Func<bool>> transitions = new();

            public State(StateMachine context)
            {
                Context = context;
            }

            public void AddTransition(State to, Func<bool> transition)
            {
                transitions.Add(to, transition);
            }
            
            protected internal virtual void OnEnter(){}

            protected internal virtual void Update() {}
            
            protected internal virtual void OnExit(){}

            public State EvaluateTransitions()
            {
                // var trans = new KeyValuePair<State, Func<bool>>();
                foreach (var kvp in transitions)
                {
                    if (kvp.Value.Invoke())
                    {
                        return kvp.Key;
                    }
                }

                return null;
            }
        }
    }
}