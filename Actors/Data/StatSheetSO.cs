using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoolTools.Actors
{
    [CreateAssetMenu(menuName = "Actor/Stat Sheet", fileName = "New Actor Stat Sheet")]
    public class StatSheetSO : ScriptableObject
    {
        [Serializable]
        public struct AttributeValue
        {
            public AttributeSO Attribute;
            public int Value;

            public bool Equals(AttributeValue other)
            {
                return Attribute == other.Attribute && Value == other.Value;
            }
        }
        
        [SerializeField] private List<AttributeValue> baseStats;
        
        public AttributeValue[] BaseStats => baseStats.ToArray();

        public int GetBaseAttributeValue(AttributeSO definition) =>
            baseStats.FirstOrDefault(s => s.Attribute == definition).Value;
    }
}