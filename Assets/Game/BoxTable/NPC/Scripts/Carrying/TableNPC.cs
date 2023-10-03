using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TableNPC : MonoBehaviour, IBoxOpener
{
    public TableCarrier tableCarrier;
    public new bool enabled = true;

    public bool IsOpening;
    public Slider ProgressSlider;
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
    public float wakingDistance = 10;

    private static readonly int ForwardSpeed = Animator.StringToHash("forwardSpeed");
    private Transform player;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
    }

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
        if(IsOpening)
           ProgressSlider.value += 1 * Time.deltaTime;
        if (Vector3.Distance(transform.GetChild(0).transform.position, player.position) <
            wakingDistance && IsSleeping)
            SetIsSleeping(false);
    }

    public void OnFinishedOpenBox()
    {
        Debug.Log("Giving box to targetCarrier");
        myAnimator.Play("NPC_Idle");

        CurrentItem = _itemsManager.GetRandomItemFromBox(CurrentBox.Type, null);
        _itemsManager.UnlockItem(CurrentItem);
        Debug.Log("Got new item: " + CurrentItem.name);

        ProgressSlider.value = ProgressSlider.minValue;
        IsOpening = false;

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
        box.CanBeOpened = false;
        Invoke(nameof(OnFinishedOpenBox), waitTime);
        ProgressSlider.maxValue = waitTime;
        ProgressSlider.value = ProgressSlider.minValue;
        IsOpening = true;
        return true;
    }

    private void SetIsSleeping(bool b)
    {
        IsSleeping = b;
        SleepPartical.SetActive(b);
        if (IsSleeping)
        {
            myAnimator.Play("NPC_Sleep_Idle");
        }
        else
        {
            myAnimator.Play("NPC_Idle");
        }
        Seconds = 0;
        Debug.Log("NPC of " + transform.parent.name + "is sleeping = " + b);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, wakingDistance);
    }
}