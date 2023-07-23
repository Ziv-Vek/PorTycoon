using System;
using System.Collections;
using System.Collections.Generic;
using ScratchCardAsset;
using UnityEngine;
using UnityEngine.Serialization;

public class ScratchBoard : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject scratchBoard;
    public float targetScratchProgress = 0.3f;
    public Item CurrentItem { get; set; }

    [SerializeField] private ScratchCardManager cardManager;
    [SerializeField] private PlayerMover playerMover;
    [SerializeField] private TableCarrier carrier;

    private bool _doneScratchCard = false;

    void Start()
    {
        // get box table of the parent prefab
        scratchBoard.SetActive(false);
    }

    void Update()
    {
        // Rotate the UI to face the camera
        var rotation = mainCamera.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.forward,
            rotation * Vector3.up);

        transform.position = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, mainCamera.nearClipPlane + 20));
    }

    public void Close()
    {
        playerMover.ToggleMovement(true);
        scratchBoard.SetActive(false);
    }

    public void Open()
    {
        if (!carrier.CheckCanGiveBoxes())
        {
            return;
        }

        playerMover.ToggleMovement(false);
        CurrentItem = GameManager.Instance.CurrentLevel.GetRandomItemForLevel();
        cardManager.Progress.OnProgress += OnScratchProgress;
        cardManager.ClearScratchCard();
        scratchBoard.SetActive(true);
    }

    private void OnFinishedScratching()
    {
        _doneScratchCard = false;
        carrier.GiveBox();
        if (carrier.CheckCanGiveBoxes())
        {
            CurrentItem = GameManager.Instance.CurrentLevel.GetRandomItemForLevel();
        }
        else
        {
            cardManager.Progress.OnProgress -= OnScratchProgress;
            Close();
        }
        // TODO: Here we should invoke money throw
    }

    private void OnScratchProgress(float progress)
    {
        if (progress >= targetScratchProgress && !_doneScratchCard)
        {
            _doneScratchCard = true;
            // apply function after X seconds
            cardManager.FillScratchCard();
            Invoke(nameof(OnFinishedScratching), 2f);

            Debug.Log($"User scratched {Math.Round(progress * 100f, 2)}% of surface");
        }
    }
}