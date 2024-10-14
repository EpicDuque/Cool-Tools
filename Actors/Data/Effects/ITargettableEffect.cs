using System.Collections.Generic;

namespace CoolTools.Actors
{
    public interface ITargettableEffect
    {
        public IEnumerable<Actor> GetTargets(Actor source);
        
    }
}