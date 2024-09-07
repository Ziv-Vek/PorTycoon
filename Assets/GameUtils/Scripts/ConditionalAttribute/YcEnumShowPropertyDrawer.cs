using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(YcEnumShowAttribute))]
public class YcEnumShowPropertyDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        YcEnumShowAttribute condHAtt = (YcEnumShowAttribute)this.attribute;
        bool enabled = this.GetResult(condHAtt, property);
        bool wasEnabled = GUI.enabled;
        GUI.enabled = enabled;
        if (enabled) {
            EditorGUI.PropertyField(position, property, label, true);
        }
        GUI.enabled = wasEnabled;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        YcEnumShowAttribute condHAtt = (YcEnumShowAttribute)this.attribute;
        if (this.GetResult(condHAtt, property)) {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        return -EditorGUIUtility.standardVerticalSpacing;
    }

    private bool GetResult(YcEnumShowAttribute condHAtt, SerializedProperty property) {
        string propertyPath = property.propertyPath;
        string conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionalSourceField); 
        SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);
        if (sourcePropertyValue != null) {
            for (int i = 0; i < condHAtt.HideWhenEqualToValues.Length; i++) {
                if (sourcePropertyValue.enumNames[sourcePropertyValue.enumValueIndex] == condHAtt.HideWhenEqualToValues[i]) {
                    return true;
                }
            }
        }
        return false;
    }
}

#endif