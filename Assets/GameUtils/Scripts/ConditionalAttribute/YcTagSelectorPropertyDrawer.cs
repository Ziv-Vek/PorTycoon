using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

//Original by DYLAN ENGELMAN http://jupiterlighthousestudio.com/custom-inspectors-unity/

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(YcTagSelectorAttribute))]
public class YcTagSelectorPropertyDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        if (property.propertyType == SerializedPropertyType.String) {
            EditorGUI.BeginProperty(position, label, property);

            var attrib = this.attribute as YcTagSelectorAttribute;

            if (attrib.UseDefaultTagFieldDrawer) {
                property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
            } else {
                List<string> tagList = new List<string>();
                tagList.Add("<NoTag>");
                tagList.AddRange(UnityEditorInternal.InternalEditorUtility.tags);
                string propertyString = property.stringValue;
                int index = -1;
                if (propertyString == "") {
                    index = 0;
                } else {
                    for (int i = 1; i < tagList.Count; i++) {
                        if (tagList[i] == propertyString) {
                            index = i;
                            break;
                        }
                    }
                }

                index = EditorGUI.Popup(position, label.text, index, tagList.ToArray());

                if (index >= 1) {
                    property.stringValue = tagList[index];
                } else {
                    property.stringValue = "";
                }
            }
            EditorGUI.EndProperty();
        } else {
            Debug.LogError("Property " + property.name + " is not a string. TagSelector attribute cannot be applied");
            EditorGUI.PropertyField(position, property, label);
        }
    }
}

#endif