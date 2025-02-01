using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoolTools.Utilities
{
    public class StateMachine
    {
        public State CurrentState { get; protected set; }
        public State PreviousState { get; protected set; }

        public void Initialize(State state)
        {
            CurrentState = state;
            CurrentState.OnEnter();
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

        // ReSharper disable Unity.PerformanceAnalysis
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
            
            private readonly Dictionary<State, Func<bool>> _transitions = new();
            
            private readonly Dictionary<State, float> _originalTransitionTimers = new();

            private readonly List<State> _timerStates = new();
            private readonly List<float> _timers = new(); 

            public State(StateMachine context)
            {
                Context = context;
            }

            public void AddTransition(State to, Func<bool> condition)
            {
                _transitions.Add(to, condition);
            }

            public void AddTransition(State to, float timer)
            {
                _timerStates.Add(to);
                _timers.Add(timer);
                _originalTransitionTimers.Add(to, timer);
            }
            
            protected internal virtual void OnEnter(){}

            protected internal virtual void Update() {}
            
            protected internal virtual void OnExit(){}

            public State EvaluateTransitions()
            {
                foreach (var kvp in _transitions)
                {
                    if (kvp.Value.Invoke())
                    {
                        return kvp.Key;
                    }
                }

                foreach (var state in _timerStates)
                {
                    var timerIndex = _timerStates.IndexOf(state);
                    var time = _timers[timerIndex];
                    
                    if (time <= 0)
                    {
                        _timers[timerIndex] = _originalTransitionTimers[state];
                        return state;
                    }

                    _timers[timerIndex] -= Time.deltaTime;
                }
                
                return null;
            }
        }
    }
}