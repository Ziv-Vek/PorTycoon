using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1)]
public class UserDataManager : MonoBehaviour
{
    private const string FILE_NAME = "userData.json";
    private string _persistentDataPath;

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

        _persistentDataPath = Application.persistentDataPath;
        CheckFirstBuildRun();
        LoadUserData();
    }


    void DeletePersistentDataSaveFile()
    {
        string path = _persistentDataPath + FILE_NAME;
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    void CheckFirstBuildRun()
    {
        string currentVersion = Application.version;
        string savedVersion = PlayerPrefs.GetString("AppVersion", "");

        if (currentVersion != savedVersion)
        {
            Debug.Log("First run after install or update");
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetString("AppVersion", currentVersion);
            PlayerPrefs.Save();

            DeletePersistentDataSaveFile();
        }
    }

    private async Task SaveToFileAsync(UserData userData)
    {
        string json = JsonConvert.SerializeObject(userData);
        string path = _persistentDataPath + FILE_NAME;

        using (StreamWriter writer = new StreamWriter(path, false))
        {
            await writer.WriteAsync(json);
            Debug.Log("Saved data to: " + path);
        }
    }

    private void SaveToFile(UserData userData)
    {
        string json = JsonConvert.SerializeObject(userData);
        string path = _persistentDataPath + FILE_NAME;

        using StreamWriter writer = new StreamWriter(path, false);
        writer.Write(json);
        Debug.Log("Saved data to: " + path);
    }


    public Task SaveUserDataAsync()
    {
        var userData = new UserData();
        ItemsManager.Instance.SaveData(userData);
        GameManager.Instance.SaveData(userData);

        return Task.Run(() => SaveToFileAsync(userData));
    }

    public void SaveUserData()
    {
        var userData = new UserData();
        ItemsManager.Instance.SaveData(userData);
        GameManager.Instance.SaveData(userData);

        SaveToFile(userData);
    }

    private bool HasLoadData()
    {
        return File.Exists(_persistentDataPath + FILE_NAME);
    }

    private void LoadUserData()
    {
        UserData userData;
        if (!HasLoadData())
        {
            Debug.Log("No save data found");
            userData = new UserData();
            SaveToFile(userData);
        }
        else
        {
            string json = File.ReadAllText(_persistentDataPath + FILE_NAME);
            userData = JsonConvert.DeserializeObject<UserData>(json);

            if (!userData.GoneThroughTutorial)
            {
                userData = new UserData();
            }
        }

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

        var userData = new UserData();
        ItemsManager.Instance.ResetData();
        GameManager.Instance.ResetData();

        SaveToFile(userData);
        UIManager.Instance.UpdateUI();
        Debug.Log("reset UI called");
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