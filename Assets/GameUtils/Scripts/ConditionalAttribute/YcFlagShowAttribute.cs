using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class YcFlagShowAttribute : PropertyAttribute {

    public string ConditionalSourceField = "";
    public int[] HideWhenEqualToValues;

    public YcFlagShowAttribute(string conditionalSourceField, params object[] enumValues) {
        this.Setup(conditionalSourceField, Array.ConvertAll(enumValues, item => (int)item));
    }

    private void Setup(string conditionalSourceField, int[] hideWhenEqualToValues) {
        this.ConditionalSourceField = conditionalSourceField;
        this.HideWhenEqualToValues = hideWhenEqualToValues;
    }
}
