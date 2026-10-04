using System;
using UnityEngine;

namespace DuzeraTools.GizmosDrawer
{
[AttributeUsage(AttributeTargets.Field)]
public sealed class MinMaxSliderAttribute : PropertyAttribute
{
    public float Min { get; }
    public float Max { get; }

    public MinMaxSliderAttribute(float min, float max)
    {
        Min = min;
        Max = max;
    }
}
}
