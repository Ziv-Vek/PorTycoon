using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[DefaultExecutionOrder(-4)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int CurrentLevel { get; set; } = 1;

    // player settings
    public bool GoneThroughTutorial;
    public bool Sound = true;
    public bool Music = true;
    public int money;
    public int stars;
    public int experience = 1;
    public int playerSpeedLevel = 1;
    public int playerBoxPlacesLevel = 1;
    [SerializeField] public int level = 1;

    public int AmountOfLevels;

    public Dictionary<string, LevelData> LevelsData { get; private set; } = new Dictionary<string, LevelData>();

    public bool Vibration = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        level = 1;
    }

    public void SaveData(UserData userData)
    {
        userData.experience = experience;
        userData.GoneThroughTutorial = GoneThroughTutorial;
        userData.Sound = Sound;
        userData.Music = Music;
        userData.Vibration = Vibration;
        userData.money = money;
        userData.stars = stars;
        userData.LevelsData = LevelsData;
        userData.playerSpeedLevel = playerSpeedLevel;
        userData.playerBoxPlacesLevel = playerBoxPlacesLevel;
        userData.currentLevel = level;
    }

    public void LoadData(UserData userData)
    {
        experience = userData.experience;
        GoneThroughTutorial = userData.GoneThroughTutorial;
        Sound = userData.Sound;
        Music = userData.Music;
        Vibration = userData.Vibration;
        money = userData.money;
        stars = userData.stars;
        LevelsData = userData.LevelsData ?? new Dictionary<string, LevelData>();
        playerSpeedLevel = userData.playerSpeedLevel;
        playerBoxPlacesLevel = userData.playerBoxPlacesLevel;

        Debug.Log("load called, level: " + userData.currentLevel + "GoneThroughTutorial: " + GoneThroughTutorial);
        if (experience < 1)
            experience = 1;
        GameObject.FindWithTag("Player").GetComponent<PlayerMover>().SpawnPlayer(CurrentLevel);
    }

    public void ResetData()
    {
        UserData userData = new UserData();
        Sound = userData.Sound;
        Music = userData.Music;
        GoneThroughTutorial =  userData.GoneThroughTutorial;
        money = userData.money;
        stars = userData.stars;
        experience = userData.experience;
        playerSpeedLevel = userData.playerSpeedLevel;
        playerBoxPlacesLevel = userData.playerBoxPlacesLevel;
        level = userData.currentLevel;
        foreach (var key in LevelsData.Keys.ToList())
        {
            LevelsData[key] = new LevelData();
        }
    }

    public void SetCurrentLevel(int level)
    {
        Debug.Log("SetCurrentLevel: " + level);
        this.level = level;
        CurrentLevel = level;
    }
}