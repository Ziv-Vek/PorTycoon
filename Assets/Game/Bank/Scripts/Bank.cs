using System;
using UnityEngine;

public class Bank : MonoBehaviour
{
    public static Bank Instance;
    [SerializeField] int amountOfMoneyToThrow = 100;
    public int CargoMoneyAmount = 5;
    public int ScretchMoneyAmount = 5;

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
        int currentStars = GameManager.Instance.money;
        currentStars += StarsAmount;
        GameManager.Instance.money = currentStars;
        UIManager.Instance.UpdateStarsText(currentStars);
    }
    public void AddMoneyToPile(MoneyPile pile)
    {
        if(pile.gameObject.name == "CargoMoneyPile")
            pile.AddMoney(CargoMoneyAmount);
        if (pile.gameObject.name == "ScretchMoneyPile")
            pile.AddMoney(ScretchMoneyAmount);
    }
}
