using System;
using UnityEngine;

public class IntervalVector2Attribute : PropertyAttribute
{
    public float min, max;

    public IntervalVector2Attribute(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}
