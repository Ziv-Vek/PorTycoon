using System;
using System.Collections;
using System.Collections.Generic;
using ScratchCardAsset;
using UnityEngine;

public class ScratchBoard : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject scratchBoard;
    public float targetScratchProgress = 0.3f;
    [SerializeField] private ScratchCardManager cardManager;
    [SerializeField] private PlayerMover playerMover;

    void Start()
    {
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
        playerMover.ToggleMovement(false);
        cardManager.Progress.OnProgress += OnScratchProgress;
        cardManager.ClearScratchCard();
        scratchBoard.SetActive(true);
    }

    private void OnFinishedScratching()
    {
        cardManager.Progress.OnProgress -= OnScratchProgress;
        // TODO: Here we should invoke money throw
        Close();
    }

    private void OnScratchProgress(float progress)
    {
        if (progress >= targetScratchProgress)
        {
            // apply function after X seconds
            cardManager.FillScratchCard();
            Invoke(nameof(OnFinishedScratching), 2f);

            Debug.Log($"User scratched {Math.Round(progress * 100f, 2)}% of surface");
        }
    }
}