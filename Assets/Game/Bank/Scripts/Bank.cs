using System;
using UnityEngine;

public class Bank : MonoBehaviour
{
    public static Bank Instance;
    [SerializeField] int amountOfMoneyToThrow = 100;

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
        GameManager.Instance.SaveData();
    }
}
