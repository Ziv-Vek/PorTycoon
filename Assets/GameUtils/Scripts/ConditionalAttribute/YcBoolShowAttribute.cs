using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class YcBoolShowAttribute : PropertyAttribute {

    public string ConditionalSourceField = "";
    public bool ShowWhenEqualToValue;

    public YcBoolShowAttribute(string conditionalSourceField, bool showWhenEqualToValue) {
        this.ConditionalSourceField = conditionalSourceField;
        this.ShowWhenEqualToValue = showWhenEqualToValue;
    }
}