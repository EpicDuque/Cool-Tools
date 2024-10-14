using UnityEngine;

namespace CoolTools.Actors
{
    /// <summary>
    /// Faction represents an abstraction to help determine if damage can be transmitted safely.
    /// Not limited to damage system and can be used for other purposes.
    /// </summary>
    [CreateAssetMenu(fileName = "New Faction", menuName = "Actor/Faction")]
    public class ActorFaction : ScriptableObject
    {
        [SerializeField] protected string factionName;

        public string FactionName => factionName;
    }
}