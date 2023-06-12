using System;
using UnityEngine;

public class Bank : MonoBehaviour
{
    public static Bank Instance;
    [SerializeField] float amountOfMoneyToThrow = 100;

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

    public float GetAmountOfMoneyToThrow()
    {
        return amountOfMoneyToThrow;
    }
}
