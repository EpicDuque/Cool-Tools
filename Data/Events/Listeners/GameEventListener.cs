using System;
using CoolTools.Attributes;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

namespace CoolTools.Data
{
    public abstract class GameEventListener<T, ET> : MonoBehaviour, IGameEventListener<ET> 
        where T : GameEventSO<ET>
    {
        public T Event;
        [SerializeField] protected bool unsubscribeOnDisable = true;
        
        [Tooltip("This will allow the Event ScriptableObject's inspector to have a proper serialized field of this subscriber. " +
                 "Might have a bit of performance impact, but it's not noticeable.")]
        [SerializeField] protected bool subscribeWithContext = false;
        [SerializeField] protected bool _logEventListen;
        [SerializeField] protected bool _logResponse;
        
        [Space(10f)] 
        [SerializeField] private float throttleTime;
        [SerializeField] private float invokeDelay;

        public UnityEvent<ET> Response;

        [Space(10f)]
        [InspectorDisabled] public ET LastData;

        private bool subscribed;
        private float throttleCount;

        private void OnEnable()
        {
            if (subscribed) return;
            
            Event.Subscribe(this, subscribeWithContext ? gameObject : null);
            subscribed = true;
        }

        private void OnDisable()
        {
            if(!unsubscribeOnDisable) return;
            
            Event.Unsubscribe(this);
            subscribed = false;
        }

        public virtual void OnEventRaised(ET data)
        {
            if(_logEventListen)
                Debug.Log($"[{nameof(GameEventListener<T, ET>)} {nameof(T)}] Recieved event {Event.name} with data {data}", this);
            
            if(throttleCount > 0f) return;
            
            LastData = data;
            Observable.Timer(TimeSpan.FromSeconds(invokeDelay)).Subscribe(_ =>
            {
                if(_logResponse)
                    Debug.Log($"[{nameof(GameEventListener<T, ET>)} {nameof(T)}] Invoking response for event {Event.name} with data {data}", this);
                
                Response?.Invoke(data);
            }).AddTo(this);
            
            throttleCount = throttleTime;
        }

        private void Update()
        {
            if (throttleCount > 0f)
                throttleCount -= Time.deltaTime;
            else
                throttleCount = 0f;
        }
    }
}
