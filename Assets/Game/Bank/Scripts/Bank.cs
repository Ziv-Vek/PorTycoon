using System;
using UnityEngine;

public class Bank : MonoBehaviour
{
    public static Bank Instance;
    [SerializeField] int amountOfMoneyToThrow = 100;
    public int CargoMoneyAmount = 5;
    public int ScretchMoneyAmount = 5;
    [SerializeField] GameObject star;
    [SerializeField] Transform player;

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
        UIManager.Instance.UpdateMoneyText(GameManager.Instance.money);
    }

    public float GetAmountOfMoneyToThrow()
    {
        return amountOfMoneyToThrow;
    }

    public void DepositMoney(int moneyAmount)
    {
        int currentMoney = GameManager.Instance.money;
        currentMoney += moneyAmount;
        GameManager.Instance.money = currentMoney;
        UIManager.Instance.UpdateMoneyText(currentMoney);
    }

    public void DepositStars(int StarsAmount)
    {
        int currentStars = GameManager.Instance.stars;
        currentStars += StarsAmount;
        GameManager.Instance.stars = currentStars;
        UIManager.Instance.UpdateStarsText(currentStars);
        UserDataManager.Instance.SaveUserDataAsync();
    }

    public void AddMoneyToPile(MoneyPile pile, String s)
    {
        if (s == "Cargo")
            pile.AddMoney(CargoMoneyAmount);
        if (s == "Scratch")
            pile.AddMoney(ScretchMoneyAmount);
        if (s == "Win")
            pile.AddMoney(ScretchMoneyAmount);
    }

    public void SpawnStar()
    {
        Instantiate(star, new Vector3(player.position.x, 8.4f, player.position.z), Quaternion.identity);
    }
}