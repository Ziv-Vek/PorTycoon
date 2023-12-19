using System;
using UnityEngine;
using System.Collections.Generic;

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
    public int experience { get; set; } = 1;
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

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                UserDataManager.Instance.ResetUserData();
                Debug.Log("Data has been reset");
            }
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
        LevelsData = userData.LevelsData;
        playerSpeedLevel = userData.playerSpeedLevel;
        playerBoxPlacesLevel = userData.playerBoxPlacesLevel;

        if (GoneThroughTutorial == false)
            UserDataManager.Instance.ResetUserData();
        if (experience < 1)
            experience = 1;
        GameObject.FindWithTag("Player").GetComponent<PlayerMover>().SpawnPlayer(CurrentLevel);

    }

    public void ResetData()
    {
        GoneThroughTutorial = false;
        money = 140;
        stars = 0;
        experience = 1;
        playerSpeedLevel = 1;
        playerBoxPlacesLevel = 1;
        level = 1;
        for (int i = 0; i < LevelsData.Count; i++)
        {
            LevelsData["Port" + (i + 1)].shipSpeedLevel = 1;
            LevelsData["Port" + (i + 1)].quantityLevel = 1;
            LevelsData["Port" + (i + 1)].qualityLevel = 1;
            LevelsData["Port" + (i + 1)].convayorSpeedLevel = 1;
            LevelsData["Port" + (i + 1)].scanningSpeedLevel = 1;
            LevelsData["Port" + (i + 1)].tableStackLevel = 1;
            LevelsData["Port" + (i + 1)].openBoxTimeNpc = 1;
            LevelsData["Port" + (i + 1)].awarenessTimeNpc = 1;
            LevelsData["Port" + (i + 1)].forklifSpeedLevel = 1;
            LevelsData["Port" + (i + 1)].forkliftBoxQuantityLevel = 1;
            LevelsData["Port" + (i + 1)].forkliftFuelTankLevel = 1;
            LevelsData["Port" + (i + 1)].scratchSizeScaleLevel = 1;
            LevelsData["Port" + (i + 1)].ForkliftIsEnabled = false;
            LevelsData["Port" + (i + 1)].HandyManNumber = 0;
            LevelsData["Port" + (i + 1)].ShipNumber = 0;
        }
    }

    public void SetCurrentLevel(int level)
    {
        Debug.Log("SetCurrentLevel: " + level);
        this.level = level;
        CurrentLevel = level;
    }
}