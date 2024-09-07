using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class YcEnumShowAttribute : PropertyAttribute {

    public string ConditionalSourceField = "";
    public string[] HideWhenEqualToValues;

    public YcEnumShowAttribute(string conditionalSourceField, params object[] enumValues) {
        this.Setup(conditionalSourceField, Array.ConvertAll(enumValues, item => item.ToString()));
    }

    private void Setup(string conditionalSourceField, string[] hideWhenEqualToValues) {
        this.ConditionalSourceField = conditionalSourceField;
        this.HideWhenEqualToValues = hideWhenEqualToValues;
    }
}
