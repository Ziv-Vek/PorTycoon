using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class UserDataManager : MonoBehaviour
{
    private const string FILE_NAME = "userData.json";

    public static UserDataManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensure the manager persists between scenes
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }

        LoadUserData();
    }

    private void SaveToFile(UserData userData)
    {
        string json = JsonConvert.SerializeObject(userData);
        File.WriteAllText(Application.persistentDataPath + FILE_NAME, json);
        Debug.Log(Application.persistentDataPath + FILE_NAME);
    }

    public void SaveUserData()
    {
        Debug.Log("save called");
        var userData = new UserData();

        ItemsManager.Instance.SaveData(userData);
        GameManager.Instance.SaveData(userData);

        SaveToFile(userData);
    }

    public bool HasLoadData()
    {
        return File.Exists(Application.persistentDataPath + FILE_NAME);
    }

    public void LoadUserData()
    {
        if (!HasLoadData())
        {
            return;
        }

        string json = File.ReadAllText(Application.persistentDataPath + FILE_NAME);
        var userData = JsonConvert.DeserializeObject<UserData>(json);

        ItemsManager.Instance.LoadData(userData);
        GameManager.Instance.LoadData(userData);
    }

    public IEnumerator SaveUserDataWithDelay()
    {
        yield return null;
        SaveUserData();
    }

    public void ResetUserData()
    {
        Debug.Log("ResetUserData");
        ItemsManager.Instance.ResetData();
        GameManager.Instance.ResetData();

        SaveUserData();
        //  UIManager.Instance.UpdateUI();
        FindAnyObjectByType<UIManager>().UpdateUI();
    }

    public void OnApplicationPause(bool pauseStatus)
    {
        SaveUserData();
    }

    public void OnApplicationQuit()
    {
        SaveUserData();
    }
}