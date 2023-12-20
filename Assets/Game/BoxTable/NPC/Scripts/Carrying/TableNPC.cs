using System;
using UnityEngine;
using UnityEngine.UI;

public class TableNPC : MonoBehaviour, IBoxOpener
{
    public TableCarrier tableCarrier;
    public new bool enabled = true;

    public bool IsOpening = false;
    public Slider ProgressSlider;
    public bool IsSleeping;
    [SerializeField] public int AwarenessSeconds = 70;
    [SerializeField] float Seconds = 0;
    public GameObject SleepPartical;
    [SerializeField] ParticleSystem wrenchParticleSystem;

    public MoneyPile moneyPile;
    [SerializeField] Animator myAnimator;
    public int waitTime = 2;
    private Item CurrentItem { get; set; }
    private PortBox CurrentBox { get; set; }

    private ItemsManager _itemsManager;
    private Bank _bank;
    public float wakingDistance = 10;

    private Transform player;

    [SerializeField] private ParticleSystem PunchEffect;
    [SerializeField] private AudioClip PunchSound;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && IsSleeping)
        {
            SetIsSleeping(false);
            PunchEffect.Play();
            GetComponent<AudioSource>().clip = PunchSound;
            GetComponent<AudioSource>().Play();
        }
    }

    public void Update()
    {
        Seconds += 1 * Time.deltaTime;
        if (IsOpening)
            ProgressSlider.value += 1 * Time.deltaTime;
    }

    public void OnFinishedOpenBox()
    {
        Debug.Log("Giving box to targetCarrier");
        myAnimator.Play("NPC_Idle");
        wrenchParticleSystem.Stop();

        CurrentItem = _itemsManager.GetRandomItemFromBox(CurrentBox.Type, transform.parent.parent.GetComponent<PortLoader>().PortLevel);
        _itemsManager.UnlockItem(CurrentItem);
        Debug.Log("Got new item: " + CurrentItem.name);

        ProgressSlider.value = ProgressSlider.minValue;
        IsOpening = false;

        _bank.AddMoneyToPile(moneyPile, "Scratch");
        // tableCarrier.RemoveBox(CurrentBox);
        Destroy(CurrentBox.transform.gameObject);
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
        myAnimator.Play("Worker_Reach_Box");
        wrenchParticleSystem.Play();
        CurrentBox = box;
        box.CanBeOpened = false;
        Invoke(nameof(OnFinishedOpenBox), waitTime);
        ProgressSlider.maxValue = waitTime;
        ProgressSlider.value = ProgressSlider.minValue;
        IsOpening = true;
        box.gameObject.transform.position = transform.Find("BoxPlace").position;
        box.gameObject.transform.rotation = transform.Find("BoxPlace").rotation;
        int index = Array.FindIndex(tableCarrier.boxes, i => i == box);
        tableCarrier.boxes[index] = null;
        tableCarrier.boxesPlaces[index].Find("Place Visual").gameObject.SetActive(true);
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
}