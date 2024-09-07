using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class YcTagSelectorAttribute : PropertyAttribute {
    public bool UseDefaultTagFieldDrawer = false;
}