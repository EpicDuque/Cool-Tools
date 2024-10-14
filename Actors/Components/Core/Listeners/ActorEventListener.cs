using System.Collections.Generic;
using CoolTools.Data;
using UnityEngine;
using UnityEngine.Events;

namespace CoolTools.Actors
{
    public class ActorEventListener : GameEventListener<ActorEvent, Actor>
    {
        [Space(10f)] 
        [SerializeField] private bool _listenForSpecificActors;
        [SerializeField] private List<Actor> _actorsToListen;
        
        [Space(10f)]
        public UnityEvent<Actor> OnSpecificActorEvent;

        public List<Actor> ActorsToListen => _actorsToListen;

        public bool ListenForSpecificActors
        {
            get => _listenForSpecificActors;
            set => _listenForSpecificActors = value;
        }

        public override void OnEventRaised(Actor data)
        {
            base.OnEventRaised(data);
            
            if (!_listenForSpecificActors) return;
            
            if (_actorsToListen.Contains(data))
                OnSpecificActorEvent?.Invoke(data);
        }
    }
}