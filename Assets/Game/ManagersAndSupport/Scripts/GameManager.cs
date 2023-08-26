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
    public int shipSpeedLevel = 1;
    public int quantityLevel = 1;
    public int qualityLevel = 1;

    public int convayorSpeedLevel = 1;
    public int scanningSpeedLevel = 1;
    public int tableStackLevel = 1;

    public int openBoxTimeNpc = 1;
    public int awarenessTimeNpc = 1;

    public int playerSpeedLevel = 1;
    public int playerBoxPlacesLevel;

    public int forkliftBoxQuantityLevel = 1;
    public int forkliftFuelTankLevel = 1;

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

    public bool CheckAllLevelItemsCollected()
    {
        return true;
    }
}