using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[DefaultExecutionOrder(-4)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int _currentLevel = 1;

    public int CurrentLevel
    {
        get => _currentLevel;
        set
        {
            YsoCorp.GameUtils.YCManager.instance.OnGameStarted(_currentLevel);
            if (_currentLevel != value)
            {
                _currentLevel = value;
            }
        }
    }


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

    public Dictionary<string, LevelData> LevelsData { get; private set; } = new();

    public bool Vibration = true;

    public bool ThereUIActive;

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
        userData.currentLevel = CurrentLevel;
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
        CurrentLevel = userData.currentLevel;
        this.level = CurrentLevel;
        Debug.Log("load called, level: " + userData.currentLevel + "GoneThroughTutorial: " + GoneThroughTutorial);
        if (experience < 1)
            experience = 1;
        if (experience != CurrentLevel)
            experience = CurrentLevel;
        GameObject.FindWithTag("Player").GetComponent<PlayerMover>().SpawnPlayer(CurrentLevel);
    }

    public void ResetData()
    {
        UserData userData = new UserData();
        userData.Sound = true;
        userData.Music = true;
        Sound = userData.Sound;
        Music = userData.Music;
        GoneThroughTutorial = userData.GoneThroughTutorial;
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

        if (PlayerPrefs.HasKey("CooldownEndTime"))
            PlayerPrefs.SetFloat("CooldownEndTime", 0);
    }
}