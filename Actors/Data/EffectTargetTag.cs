using UnityEngine;

namespace CoolTools.Actors
{
    [CreateAssetMenu(fileName = "New Effect Target Tag", menuName = "Actor/Effect Target Tag", order = 0)]
    public class EffectTargetTag : ScriptableObject
    {
        [SerializeField] private string tagName;

        public string TagName => tagName;
    }
}