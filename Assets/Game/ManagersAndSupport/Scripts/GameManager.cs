using System;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int CurrentLevel { get; set; } = 1;

    // player settings
    public int money;
    public int stars;
    public int experience;
    public int currentLevel = 1;
    public int shipSpeedLevel = 1;
    public int quantityLevel = 1;
    public int qualityLevel = 1;

    public int convayorSpeedLevel = 1;
    public int scanningSpeedLevel = 1;
    public int tableStackLevel = 1;

    public int openBoxTimeNpc = 1;
    public int awarenessTimeNpc = 1;

    public int playerSpeedLevel = 1;
    public int playerBoxPlacesLevel = 1;

    public int forklifSpeedLevel = 1;
    public int forkliftBoxQuantityLevel = 1;
    public int forkliftFuelTankLevel = 1;

    public int scratchSizeScaleLevel = 1;

    public bool ForkliftIsEnabled = false;
    public int HandyManNumber = 0;
    public int ShipNumber = 1;

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
        userData.money = money;
        userData.stars = stars; 
        userData.playerSpeedLevel = playerSpeedLevel;
        userData.playerBoxPlacesLevel = playerBoxPlacesLevel;

        userData.experience = experience;
        userData.shipSpeedLevel = shipSpeedLevel;
        userData.quantityLevel = quantityLevel;
        userData.qualityLevel = qualityLevel;
        userData.convayorSpeedLevel = convayorSpeedLevel;
        userData.scanningSpeedLevel = scanningSpeedLevel;
        userData.tableStackLevel = tableStackLevel;
        userData.openBoxTimeNpc = openBoxTimeNpc;
        userData.awarenessTimeNpc = awarenessTimeNpc;
        userData.forklifSpeedLevel = forklifSpeedLevel;
        userData.forkliftBoxQuantityLevel = forkliftBoxQuantityLevel;
        userData.forkliftFuelTankLevel = forkliftFuelTankLevel;
        userData.scratchSizeScaleLevel = scratchSizeScaleLevel;
        userData.ForkliftIsEnabled = ForkliftIsEnabled;
        userData.HandyManNumber = HandyManNumber;
        userData.ShipNumber = ShipNumber;
    }

    public void LoadData(UserData userData)
    {
        money = userData.money;
        stars = userData.stars;
        playerSpeedLevel = userData.playerSpeedLevel;
        playerBoxPlacesLevel = userData.playerBoxPlacesLevel;

        experience = userData.experience;
        shipSpeedLevel = userData.shipSpeedLevel;
        quantityLevel = userData.quantityLevel;
        qualityLevel = userData.qualityLevel;
        convayorSpeedLevel = userData.convayorSpeedLevel;
        scanningSpeedLevel = userData.scanningSpeedLevel;
        tableStackLevel = userData.tableStackLevel;
        openBoxTimeNpc = userData.openBoxTimeNpc;
        awarenessTimeNpc = userData.awarenessTimeNpc;
        forklifSpeedLevel = userData.forklifSpeedLevel;
        forkliftBoxQuantityLevel = userData.forkliftBoxQuantityLevel;
        forkliftFuelTankLevel = userData.forkliftFuelTankLevel;
        scratchSizeScaleLevel = userData.scratchSizeScaleLevel;
        ForkliftIsEnabled = userData.ForkliftIsEnabled;
        HandyManNumber = userData.HandyManNumber;
        ShipNumber = userData.ShipNumber;
    }

    public void ResetData()
    {
        money = 0;
        stars = 0;
        experience = 0;
        shipSpeedLevel = 1;
        quantityLevel = 1;
        qualityLevel = 1;
        convayorSpeedLevel = 1;
        scanningSpeedLevel = 1;
        tableStackLevel = 1;
        openBoxTimeNpc = 1;
        awarenessTimeNpc = 1;
        playerSpeedLevel = 1;
        playerBoxPlacesLevel = 1;
        forklifSpeedLevel = 1;
        forkliftBoxQuantityLevel = 1;
        forkliftFuelTankLevel = 1;
        scratchSizeScaleLevel = 1;
        ForkliftIsEnabled = false;
        HandyManNumber = 0;
        ShipNumber = 0;
    }
}