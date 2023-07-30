using System;
using System.Collections;
using System.Collections.Generic;
using ScratchCardAsset;
using UnityEngine;
using UnityEngine.Serialization;

public class ScratchBoard : MonoBehaviour
{
    public Camera mainCamera;
    public float targetScratchProgress = 0.3f;
    public Item CurrentItem { get; set; }

    [SerializeField] private ScratchCardManager cardManager;
    [SerializeField] private PlayerMover playerMover;
    [SerializeField] private TableCarrier carrier;
    [SerializeField] private ScratchItemImage scratchItemImage;

    [SerializeField] MoneyPile moneyPile;

    // internal indicator if the scratch card is done
    private bool _doneScratchCard = false;

    void Start()
    {
        // get box table of the parent prefab
        gameObject.SetActive(false);
    }

    void Update()
    {
        // Rotate the UI to face the camera
        var rotation = mainCamera.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.forward,
            rotation * Vector3.up);

        transform.position = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, mainCamera.nearClipPlane + 20));
    }

    private void NextItem()
    {
        carrier.GiveBox();
        CurrentItem = GameManager.Instance.CurrentLevel.GetRandomItemForLevel();
        scratchItemImage.ChangeImage(CurrentItem.image);
        cardManager.ClearScratchCard();
    }

    public void Close()
    {
        playerMover.ToggleMovement(true);
        playerMover.ShowJoystick();
        gameObject.SetActive(false);
    }

    public void Open()
    {
        if (!carrier.CheckCanGiveBoxes())
        {
            return;
        }

        playerMover.ToggleMovement(false);
        playerMover.HideJoystick();
        NextItem();
        cardManager.Progress.OnProgress += OnScratchProgress;
        gameObject.SetActive(true);
    }

    private void OnFinishedScratching()
    {
        _doneScratchCard = false;
        GameManager.Instance.UnlockItem(CurrentItem);
        Debug.Log("unlocked item: " + CurrentItem.name);
        Debug.Log("Probability: " + CurrentItem.Probability);

        if (carrier.CheckCanGiveBoxes())
        {
            NextItem();
        }
        else
        {
            cardManager.Progress.OnProgress -= OnScratchProgress;
            Close();
        }

        Bank.Instance.AddMoneyToPile(moneyPile);
        // TODO: Here we should invoke money throw
    }

    private void OnScratchProgress(float progress)
    {
        if (progress >= targetScratchProgress && !_doneScratchCard)
        {
            _doneScratchCard = true;
            // apply function after X seconds
            cardManager.FillScratchCard();
            Invoke(nameof(OnFinishedScratching), 1f);

            Debug.Log($"User scratched {Math.Round(progress * 100f, 2)}% of surface");
        }
    }
}