using System;
using UnityEngine;

public class MoneyPile : MonoBehaviour
{
    [SerializeField] private int moneyAmount = 10;

    private void OnTriggerEnter(Collider other)
    {
        Bank.Instance.DepositMoney(moneyAmount);
        Destroy(gameObject);
    }
}
