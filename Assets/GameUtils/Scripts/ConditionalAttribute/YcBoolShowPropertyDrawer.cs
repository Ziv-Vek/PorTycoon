using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(YcBoolShowAttribute))]
public class YcBoolShowPropertyDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        YcBoolShowAttribute condHAtt = (YcBoolShowAttribute)this.attribute;
        bool enabled = this.GetResult(condHAtt, property);
        bool wasEnabled = GUI.enabled;
        GUI.enabled = enabled;
        if (enabled) {
            EditorGUI.PropertyField(position, property, label, true);
        }
        GUI.enabled = wasEnabled;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        YcBoolShowAttribute condHAtt = (YcBoolShowAttribute)this.attribute;
        if (this.GetResult(condHAtt, property)) {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        return -EditorGUIUtility.standardVerticalSpacing;
    }

    private bool GetResult(YcBoolShowAttribute condHAtt, SerializedProperty property) {
        string propertyPath = property.propertyPath; //returns the property path of the property we want to apply the attribute to
        string conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionalSourceField); //changes the path to the conditionalsource property path
        SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);
        if (sourcePropertyValue != null) {
            return sourcePropertyValue.boolValue == condHAtt.ShowWhenEqualToValue;
        }
        return true;
    }
}

#endif