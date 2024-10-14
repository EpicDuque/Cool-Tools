using UnityEngine;

namespace CoolTools.Actors
{
    public class ActorScanner : Scanner<Actor>
    {
        protected override bool SelfCheck(Actor other)
        {
            return other == Owner;
        }
    }
}