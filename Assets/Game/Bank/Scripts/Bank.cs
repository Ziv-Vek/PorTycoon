using System;
using UnityEngine;

public class Bank : MonoBehaviour
{
    public static Bank Instance;
    [SerializeField] int amountOfMoneyToThrow = 100;
    public int CargoMoneyAmount = 5;
    public int ScretchMoneyAmount = 5;
    [SerializeField] GameObject star;
    [SerializeField] GameObject Player;

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
    }
    public void AddMoneyToPile(MoneyPile pile)
    {
        if(pile.gameObject.name == "CargoMoneyPile")
            pile.AddMoney(CargoMoneyAmount);
        if (pile.gameObject.name == "ScretchMoneyPile")
            pile.AddMoney(ScretchMoneyAmount);
    }
    public void SpawnStar()
    {
      
        Instantiate(star, new Vector3(Player.transform.position.x, 8.4f, GameObject.Find("Player").transform.position.z),Quaternion.identity);
        
    }
}
