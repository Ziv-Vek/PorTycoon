using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.InputSystem;

public class TableNPC : MonoBehaviour, IBoxOpener
{
    public TableCarrier tableCarrier;
    public new bool enabled = true;

    public bool IsSleeping = false;
    [SerializeField] public int AwarenessSeconds = 70;
    [SerializeField] float Seconds = 0;
    public GameObject SleepPartical;

    public MoneyPile moneyPile;
    [SerializeField] Animator myAnimator;
    public int waitTime = 2;
    private Item CurrentItem { get; set; }
    private PortBox CurrentBox { get; set; }

    private ItemsManager _itemsManager;
    private Bank _bank;

    private static readonly int ForwardSpeed = Animator.StringToHash("forwardSpeed");

    private void Start()
    {
        // cache instances
        _itemsManager = ItemsManager.Instance;
        _bank = Bank.Instance;
        SetIsSleeping(false);

        if (!IsSleeping && enabled)
            tableCarrier.AddBoxOpener(this);
    }

    public void Update()
    {
        Seconds += 1 * Time.deltaTime;
        if (Vector3.Distance(transform.GetChild(0).transform.position, GameObject.Find("Player").transform.position) <
            6 && IsSleeping)
            SetIsSleeping(false);
    }

    public void OnFinishedOpenBox()
    {
        Debug.Log("Giving box to targetCarrier");
        myAnimator.SetFloat(ForwardSpeed, 0);

        CurrentItem = _itemsManager.GetRandomItemFromBox(CurrentBox.Type, null);
        _itemsManager.UnlockItem(CurrentItem);
        Debug.Log("Got new item: " + CurrentItem.name);


        _bank.AddMoneyToPile(moneyPile, "Scratch");
        tableCarrier.RemoveBox(CurrentBox);
        tableCarrier.AddBoxOpener(this);

        if (Seconds >= AwarenessSeconds)
        {
            SetIsSleeping(true);
        }
    }


    public bool OpenBox(PortBox box)
    {
        if (!enabled || IsSleeping) return false; //TODO: Take enabled from global state
        Debug.Log("Taking box from targetCarrier");
        CurrentBox = box;
        myAnimator.SetFloat(ForwardSpeed, 100f);
        box.CanBeOpened = false;
        Invoke(nameof(OnFinishedOpenBox), waitTime);
        return true;
    }

    private void SetIsSleeping(bool b)
    {
        IsSleeping = b;
        SleepPartical.SetActive(b);
        Seconds = 0;
        Debug.Log("NPC of " + transform.parent.name + "is sleeping = " + b);
    }
}