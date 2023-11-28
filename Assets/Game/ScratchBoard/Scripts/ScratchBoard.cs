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

    [SerializeField] private PlayerMover playerMover;
    [SerializeField] private TableCarrier tableCarrier;
    [SerializeField] private ScratchItemModel scratchItemModel;
    [SerializeField] private PlayerCarrier playerCarrier;
    [SerializeField] private Button throwButton;
    [SerializeField] private MoneyPile moneyPile;

    [SerializeField] private GameObject EndButtons;

    [Header("Settings")] public float targetScratchProgress = 0.3f;

    private bool _isScratching;
    private Item CurrentItem { get; set; }
    private PortBox CurrentBox { get; set; }

    private void Start()
    {
        gameObject.SetActive(false);
        throwButton.onClick.AddListener(OnThrowBox);
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
        Destroy(scratchItemModel.transform.GetChild(0).gameObject);
        if (CurrentBox.isPurchasedBox)
        {
            UIManager.Instance.OpenCollectionCanvas();
            return;
        }
        PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        playerMover.ToggleMovement(true);
        playerMover.ShowJoystick();

        playerMover.ShowJoystick();
        tableCarrier.SetPlayer(playerCarrier);
    }

    public void Open(PortBox box)
    {
        PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        playerMover.ToggleMovement(false);
        playerMover.HideJoystick();

        playerMover.ToggleMovement(false);
        playerMover.HideJoystick();

        CurrentBox = box;
        CurrentBox.CanBeOpened = false;

        tableCarrier.RemovePlayer();


        NextItem();

        cardManager.Progress.OnProgress += OnScratchProgress;
        gameObject.SetActive(true);
    }

    private void OnFinishedScratching()
    {
        _isScratching = false;

        if (!CurrentBox.isPurchasedBox)
        {
            tableCarrier.RemoveBox(CurrentBox);
        }

        //PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        //playerMover.ToggleMovement(true);
        //playerMover.ShowJoystick();

        ItemsManager.Instance.UnlockItem(CurrentItem);

        cardManager.Progress.OnProgress -= OnScratchProgress;
        // Close();
        if (!CurrentBox.isPurchasedBox)
        {
            EndButtons.SetActive(true);
        }
        else
            Close();

        throwButton.gameObject.SetActive(false);
        GetComponent<PanelTouchHandler>().CanScratch = false;
        GetComponent<PanelTouchHandler>().ScratchPartical.Pause();

        Bank.Instance.AddMoneyToPile(moneyPile, "Scratch");
    }

    private void OnScratchProgress(float progress)
    {
        if (progress >= targetScratchProgress && !_isScratching)
        {
            _isScratching = true;
            cardManager.FillScratchCard();
            Invoke(nameof(OnFinishedScratching), 1f);
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
    public void BackToPort()
    {
        Close();
        tableCarrier.RemovePlayer();
    }
}