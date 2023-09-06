using System.IO;
using Newtonsoft.Json;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class UserDataManager: MonoBehaviour
{
    private const string FILE_NAME = "userData.json";

    public static UserDataManager Instance;

    private UserData UserData { get; set; } = new();
    
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

    private void SaveToFile()
    {
        string json = JsonConvert.SerializeObject(UserData);
        File.WriteAllText(Application.persistentDataPath + FILE_NAME, json);
    }

    public void SaveUserData()
    {
        ItemsManager.Instance.SaveData(UserData);
        GameManager.Instance.SaveData(UserData);

        SaveToFile();
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
        UserData = JsonConvert.DeserializeObject<UserData>(json);


        ItemsManager.Instance.LoadData(UserData);
        GameManager.Instance.LoadData(UserData);
    }
    
    public void OnApplicationQuit()
    {
        SaveUserData();
    }
}