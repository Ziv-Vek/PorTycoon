using System;
using UnityEngine;
using UnityEngine.Serialization;


public class TableNPC : MonoBehaviour, IBoxOpener
{
    public TableCarrier tableCarrier;
    public new bool enabled = true;
    public MoneyPile moneyPile;
    [SerializeField] Animator myAnimator;
    public int waitTime = 2;
    private Item CurrentItem { get; set; }
    private PortBox CurrentBox { get; set; }

    private GameManager _gameManager;
    private Bank _bank;

    private static readonly int ForwardSpeed = Animator.StringToHash("forwardSpeed");

    private void Start()
    {
        // cache instances
        _gameManager = GameManager.Instance;
        _bank = Bank.Instance;

        tableCarrier.AddBoxOpener(this);
    }

    public void OnFinishedOpenBox()
    {
        Debug.Log("Giving box to targetCarrier");
        myAnimator.SetFloat(ForwardSpeed, 0);

        CurrentItem = _gameManager.CurrentLevel.GetRandomItemForLevel();
        _gameManager.UnlockItem(CurrentItem);
        Debug.Log("Got new item: " + CurrentItem.name);
        _bank.AddMoneyToPile(moneyPile);
        tableCarrier.RemoveBox(CurrentBox);
        tableCarrier.AddBoxOpener(this);
    }


    public bool OpenBox(PortBox box)
    {
        if (!enabled) return false; //TODO: Take enabled from global state
        Debug.Log("Taking box from targetCarrier");
        CurrentBox = box;
        myAnimator.SetFloat(ForwardSpeed, 100f);
        box.CanBeOpened = false;
        Invoke(nameof(OnFinishedOpenBox), waitTime);
        return true;
    }
}