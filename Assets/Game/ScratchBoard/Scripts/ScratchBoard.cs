using System;
using ScratchCardAsset;
using UnityEngine;
using UnityEngine.UI;

public class ScratchBoard : MonoBehaviour
{
    public float targetScratchProgress = 0.3f;
    private Item CurrentItem { get; set; }
    private PortBox CurrentBox { get; set; }


    [SerializeField] private Camera scratchCamera;
    [SerializeField] private ScratchCardManager cardManager;
    [SerializeField] private PlayerMover playerMover;
    [SerializeField] private TableCarrier tableCarrier;
    [SerializeField] private ScratchItemImage scratchItemImage;
    [SerializeField] private PlayerCarrier playerCarrier;
    [SerializeField] private Button throwButton;
    [SerializeField] MoneyPile moneyPile;

    // internal indicator if the scratch card is done
    private bool _isScratching;

    void Start()
    {
        // get box table of the parent prefab
        gameObject.SetActive(false);

        // Handle throw button
        throwButton.onClick.AddListener(OnThrowBox);
    }

    void Update()
    {
        // Rotate the UI to face the camera
        var rotation = scratchCamera.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.forward,
            rotation * Vector3.up);

        transform.position = scratchCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, scratchCamera.nearClipPlane + 20));
    }

    private void NextItem()
    {
        CurrentItem = ItemsManager.Instance.GetRandomItemFromBox(CurrentBox.Type, null);
        scratchItemImage.ChangeImage(CurrentItem.imagePath);
        cardManager.ClearScratchCard();
    }

    private void Close()
    {
        playerMover.ToggleMovement(true);
        playerMover.ShowJoystick();
        gameObject.SetActive(false);

        if (!CurrentBox.isPurchasedBox)
            // Add player back to carrier
            tableCarrier.SetPlayer(playerCarrier);
    }

    public void Open(PortBox box)
    {
        // Handle Movement
        playerMover.ToggleMovement(false);
        playerMover.HideJoystick();

        // Handle box
        CurrentBox = box;
        CurrentBox.CanBeOpened = false;

        if (!CurrentBox.isPurchasedBox)
            // Remove player from carrier
            tableCarrier.RemovePlayer();

        NextItem();

        cardManager.Progress.OnProgress += OnScratchProgress;
        gameObject.SetActive(true);
    }

    private void OnFinishedScratching()
    {
        _isScratching = false;


        if (!CurrentBox.isPurchasedBox)
            tableCarrier.RemoveBox(CurrentBox);

        ItemsManager.Instance.UnlockItem(CurrentItem);

        cardManager.Progress.OnProgress -= OnScratchProgress;
        Close();


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
            tableCarrier.RemoveBox(CurrentBox);
        Bank.Instance.AddMoneyToPile(moneyPile, "Scratch");

        cardManager.Progress.OnProgress -= OnScratchProgress;
        Close();
    }
}