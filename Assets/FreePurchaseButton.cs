using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreePurchaseButton : MonoBehaviour
{
    private bool canFreePurchase;
    public bool CanFreePurchase
    {
        get => canFreePurchase;
        private set => canFreePurchase = value;
    }

    private void Awake()
    {
        CanFreePurchase = false;
    }
}
