using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI starsText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateMoneyText(int money)
    {
        moneyText.text = money.ToString();
    }
    public void UpdateStarsText(int stars)
    {
        starsText.text = stars.ToString();
    }
}
