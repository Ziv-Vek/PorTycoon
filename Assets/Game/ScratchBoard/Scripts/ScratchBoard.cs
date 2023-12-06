using System;
using ScratchCardAsset;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScratchBoard : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private ScratchCardManager cardManager;

    [SerializeField] private Conveyor conveyorBelt;
    [SerializeField] private PlayerMover playerMover;
    [SerializeField] private TableCarrier tableCarrier;
    [SerializeField] private ScratchItemModel scratchItemModel;
    [SerializeField] private PlayerCarrier playerCarrier;
    [SerializeField] private Button throwButton;
    [SerializeField] private Button ExitButton;
    [SerializeField] private MoneyPile moneyPile;

    [SerializeField] private GameObject EndButtons;

    [Header("Settings")] public float targetScratchProgress = 0.3f;

    private bool _isScratching;
    private Item CurrentItem { get; set; }
    private PortBox CurrentBox { get; set; }

    private void Start()
    {
        throwButton.onClick.AddListener(OnThrowBox);
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        throwButton.gameObject.SetActive(true);
        GetComponent<PanelTouchHandler>().CanScratch = true;
    }
    private void NextItem()
    {
        CurrentItem = ItemsManager.Instance.GetRandomItemFromBox(CurrentBox.Type, null);
        scratchItemModel.ChangeModel(CurrentItem.imagePath);
        cardManager.ClearScratchCard();
    }

    private void Close()
    {
        gameObject.SetActive(false);
        EndButtons.SetActive(false);
        EndButtons.transform.Find("NextBoxButton").gameObject.SetActive(false);
        Destroy(scratchItemModel.transform.GetChild(0).gameObject);
      
        tableCarrier.SetPlayer(playerCarrier);
        if (CurrentBox.isPurchasedBox)
        {
            UIManager.Instance.OpenCollectionCanvas();
            tableCarrier.RemovePlayer();
            return;
        }
        else
        {
            PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
            playerMover.ToggleMovement(true);
            playerMover.ShowJoystick();
            playerMover.joystick.DeactivateJoystick();
        }
        if (!GameManager.Instance.GoneThroughTutorial)
            FindAnyObjectByType<TutorialM>().ClickOn_CollectionPanel();
    }

    public void Open(PortBox box)
    {
        PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        playerMover.ToggleMovement(false);
        playerMover.HideJoystick();

        CurrentBox = box;
        CurrentBox.CanBeOpened = false;

        tableCarrier.RemovePlayer();
        NextItem();

        cardManager.Progress.OnProgress += OnScratchProgress;
        gameObject.SetActive(true);
        if (!CurrentBox.isPurchasedBox)
        {
            throwButton.gameObject.SetActive(true);
        }
        else
            throwButton.gameObject.SetActive(false);
    }

    private void OnFinishedScratching()
    {
        _isScratching = false;

        if (!CurrentBox.isPurchasedBox)
        {
            tableCarrier.RemoveBox(CurrentBox);
        }

        ItemsManager.Instance.UnlockItem(CurrentItem);

        cardManager.Progress.OnProgress -= OnScratchProgress;
        if (!CurrentBox.isPurchasedBox)
        {
            EndButtons.SetActive(true);
            if (!tableCarrier.CheckIfBoxesEmpty() || !conveyorBelt.CheckIfBoxesEmpty())
                EndButtons.transform.Find("NextBoxButton").gameObject.SetActive(true);
            ExitButton.gameObject.SetActive(false);
        }
        else
            Close();

        throwButton.gameObject.SetActive(false);
        GetComponent<PanelTouchHandler>().CanScratch = false;
        GetComponent<PanelTouchHandler>().ScratchPartical.Stop();

        Bank.Instance.AddMoneyToPile(moneyPile, "Scratch");
    }

    private void OnScratchProgress(float progress)
    {
        if (progress >= targetScratchProgress && !_isScratching)
        {
            _isScratching = true;
            cardManager.FillScratchCard();
            Invoke(nameof(OnFinishedScratching), 1f);
            if(ItemsManager.Instance.UnlockedItems.ContainsKey(CurrentItem.id))
                AudioManager.inctece.play("Item Scratched");
            else
                AudioManager.inctece.play("Item Scratched New Item");
        }
    }

    private void OnThrowBox()
    {
        _isScratching = false;

        if (!CurrentBox.isPurchasedBox)
        {
            tableCarrier.RemoveBox(CurrentBox);
        }

        Bank.Instance.AddMoneyToPile(moneyPile, "Scratch");

        cardManager.Progress.OnProgress -= OnScratchProgress;
        Close();
    }
    public void NextBoxToOpen()
    {
        Close();
        //PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        //playerMover.ToggleMovement(false);
        //playerMover.HideJoystick();
    }
    public void ExitDuringScratch()
    {
        Close();
        tableCarrier.RemovePlayer();
    }
    public void BackToPort()
    {
        Close();
        if(!CurrentBox.isPurchasedBox)
           tableCarrier.RemovePlayer();
    }
}