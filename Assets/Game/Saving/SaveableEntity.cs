using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteAlways]
public class SaveableEntity : MonoBehaviour
{
    [SerializeField] private string uniqueID = "";
    static Dictionary<string, SaveableEntity> globalLookup = new Dictionary<string, SaveableEntity>();

    private void OnEnable()
    {
        ProcessUidSetup();
    }

    public string GetUID()
    {
        if (uniqueID == "")
            ProcessUidSetup();
        
        return uniqueID;
    }
    
    #if UNITY_EDITOR

    private void Update()
    {
        if (Application.IsPlaying(gameObject)) return;
        if (string.IsNullOrEmpty(gameObject.scene.path)) return;
        
        ProcessUidSetup();
    }

    #endif

    private void ProcessUidSetup()
    {
        SerializedObject serializedObject = new SerializedObject(this);
        SerializedProperty property = serializedObject.FindProperty("uniqueID");

        if (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
        {
            if (!Utils.Instance) gameObject.AddComponent<Utils>();
            
            property.stringValue = Utils.Instance.GetUniqueID();
            serializedObject.ApplyModifiedProperties();
        }

        globalLookup[property.stringValue] = this;
    }
    
    private bool IsUnique(string candidate)
    {
        if (!globalLookup.ContainsKey(candidate)) return true;

        if (globalLookup[candidate] == this) return true;

        if (globalLookup[candidate] == null)
        {
            globalLookup.Remove(candidate);
            return true;
        }

        if (globalLookup[candidate].GetUID() != candidate)
        {
            globalLookup.Remove(candidate);
            return true;
        }

        return false;
    }

}
