using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Damage Type", menuName = "Damage Type", order = 1)]
public class DamageType : ScriptableObject
{
    [SerializeField] private string typeName;

    public string TypeName => typeName;
}
